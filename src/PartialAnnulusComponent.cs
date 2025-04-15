using System;
using Grasshopper2;
using Grasshopper2.Components;
using Grasshopper2.Extensions;
using Grasshopper2.Types.Numeric;
using Grasshopper2.UI;
using GrasshopperIO;
using Rhino.Geometry;

namespace S2FDemo
{
  [IoId("d603ad2a-26c6-4232-aedc-c6455fca6be0")]
  public sealed class PartialAnnulus : IStorable
  {
    #region constructor
    public PartialAnnulus(Plane plane, double inner, double outer, double sweep)
    {
      inner = Math.Max(0, inner);
      outer = Math.Max(0, outer);
      Maths.Sort(ref inner, ref outer);

      Plane = plane;
      InnerRadius = inner;
      OuterRadius = outer;
      SweepFactor = sweep.Clip(0, 1);
      SweepAngle = ComputeEndAngle(inner, outer, SweepFactor);
    }
    private static double ComputeEndAngle(double inner, double outer, double sweep)
    {
      var centralCircle = new Circle(Plane.WorldXY, 0.5 * inner + 0.5 * outer);
      var cappingCircle = new Circle(centralCircle.PointAt(0), outer - inner);
      Rhino.Geometry.Intersect.Intersection.CircleCircle(centralCircle, cappingCircle, out var p0, out var p1);
      if (p0.Y > 0)
        p0 = p1;

      if (!centralCircle.ClosestParameter(p0, out var angle))
        return Maths.OnePi;

      return angle * sweep;
    }

    public PartialAnnulus(IReader reader)
    {
      Plane = reader.Plane(nameof(Plane));
      InnerRadius = reader.Number64(nameof(InnerRadius));
      OuterRadius = reader.Number64(nameof(OuterRadius));
      SweepFactor = reader.Number64(nameof(SweepFactor));
      SweepAngle = ComputeEndAngle(InnerRadius, OuterRadius, SweepFactor);
    }
    void IStorable.Store(IWriter writer)
    {
      writer.Plane(nameof(Plane), Plane);
      writer.Number64(nameof(InnerRadius), InnerRadius);
      writer.Number64(nameof(OuterRadius), OuterRadius);
      writer.Number64(nameof(SweepFactor), SweepFactor);
    }
    #endregion

    #region properties
    public Plane Plane { get; }
    public double InnerRadius { get; }
    public double OuterRadius { get; }
    public double SweepFactor { get; }
    public double SweepAngle { get; }

    public double Length
    {
      get
      {
        var length = 0.0;
        length += InnerRadius * SweepAngle;
        length += OuterRadius * SweepAngle;
        length += Maths.OnePi * (OuterRadius - InnerRadius);
        return length;
      }
    }
    public double Area
    {
      get
      {
        var annulusRadius = OuterRadius * OuterRadius - InnerRadius * InnerRadius;
        var capRadius = 0.5 * (OuterRadius - InnerRadius);

        var area = 0.0;
        area += annulusRadius * SweepAngle / 2;
        area += Maths.OnePi * capRadius * capRadius;
        return area;
      }
    }

    private Arc InnerArc
    {
      get
      {
        return new Arc(new Circle(Plane, InnerRadius), new Interval(0, SweepAngle));
      }
    }
    private Arc OuterArc
    {
      get
      {
        return new Arc(new Circle(Plane, OuterRadius), new Interval(SweepAngle, 0));
      }
    }
    private Arc CapArc0
    {
      get
      {
        return new Arc(Plane.PointAt(OuterRadius, 0), -Plane.YAxis, Plane.PointAt(InnerRadius, 0));
      }
    }
    private Arc CapArc1
    {
      get
      {
        if (SweepFactor.Equals(0.0))
          return new Arc(Plane.PointAt(InnerRadius, 0), Plane.YAxis, Plane.PointAt(OuterRadius, 0));

        var inner = InnerArc;
        var outer = OuterArc;

        if (inner.IsValid)
          return new Arc(inner.EndPoint, inner.TangentAt(inner.EndAngle), outer.StartPoint);
        else
          return new Arc(Plane.Origin, -outer.TangentAt(outer.StartAngle), outer.StartPoint);
      }
    }
    #endregion

    #region methods
    private PolyCurve _curve;
    public Curve ToRhinoCurve()
    {
      var curve = _curve;
      if (curve is null)
      {
        curve = new PolyCurve();
        if (InnerRadius.Equals(OuterRadius))
        {
          if (InnerRadius.Equals(0.0) || SweepFactor.Equals(0.0))
            return default;

          curve.Append(new Arc(new Circle(Plane, InnerRadius), SweepFactor * Maths.TwoPi));
        }
        else
        {
          curve.Append(CapArc0);

          if (SweepFactor > 0 && InnerRadius > 0)
            curve.Append(InnerArc);

          curve.Append(CapArc1);

          if (SweepFactor > 0)
            curve.Append(OuterArc);
        }

        _curve = curve;
      }
      return curve;
    }

    /// <summary>
    /// Apply a shape preserving transformation to this curve.
    /// If the transformed result cannot be represented by a partial annulus,
    /// null will be returned.
    /// </summary>
    public PartialAnnulus Transform(Transform transform)
    {
      switch (transform.DecomposeSimilarity(out var translation, out var dilation, out var rotation, 1e-6))
      {
        case TransformSimilarityType.OrientationPreserving:
          var plane = Plane;
          plane.Transform(rotation);
          plane.Origin = dilation * plane.Origin;
          plane.Origin += translation;

          var inner = dilation * InnerRadius;
          var outer = dilation * OuterRadius;
          return new PartialAnnulus(plane, inner, outer, SweepFactor);

        default:
          return default;
      }
    }

    public override string ToString()
    {
      var intent = NumberFormat.Honest.Modify(maximumDecimals: 4);
      return $"radii:{intent.Format(InnerRadius)}, {intent.Format(OuterRadius)} sweep:{SweepFactor * 100:0.0}%";
    }
    #endregion
  }

  [IoId("06141644-93bd-4518-8006-4580033e0727")]
  public sealed class AnnulusComponent : Component
  {
    public AnnulusComponent() : base(new Nomen("Partial Annulus", "Creater a partial annulus curve from plane and radii.", "S2F", "S2F")) { }
    public AnnulusComponent(IReader reader) : base(reader) { }

    protected override void AddInputs(InputAdder inputs)
    {

    }
    protected override void AddOutputs(OutputAdder outputs)
    {

    }

    protected override void Process(IDataAccess access)
    {

    }
  }
}