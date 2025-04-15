using System;
using System.Collections.Generic;
using Eto.Forms;
using Grasshopper2;
using Grasshopper2.Components;
using Grasshopper2.Data;
using Grasshopper2.Display;
using Grasshopper2.Extensions;
using Grasshopper2.Types.Assistant;
using Grasshopper2.Types.Fields;
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

  public sealed class PartialAnnulusTypeAssistant : TypeAssistant<PartialAnnulus>
  {
    public PartialAnnulusTypeAssistant() : base("Partial Annulus") { }

    public override PartialAnnulus Copy(PartialAnnulus instance)
    {
      return instance;
    }

    public override string DescribePrimary(Pear<PartialAnnulus> pear)
    {
      return pear.Item.ToString();
    }

    public override void Draw(Pear<PartialAnnulus> pear, DisplayBag bag)
    {
      var dc = new DisplayCurve(pear.Item.ToRhinoCurve(), CurveKind.Curve, pear.Meta);
      bag.AddCurve(dc);
    }
    public override IEnumerable<GeometryBase> ToGeometry(Pear<PartialAnnulus> pear)
    {
      yield return pear.Item.ToRhinoCurve();
    }
  }

  [IoId("06141644-93bd-4518-8006-4580033e0727")]
  public sealed class AnnulusComponent : Component
  {
    public AnnulusComponent() : base(new Nomen("Partial Annulus", "Creater a partial annulus curve from plane and radii.", "S2F", "S2F")) { }
    public AnnulusComponent(IReader reader) : base(reader) { }

    protected override void AddInputs(InputAdder inputs)
    {
      inputs.AddPlane("Plane", "Pl", "Annulus base plane.").Set(Plane.WorldXY);
      inputs.AddField("Inner Radius", "Ri", "Annulus inner radius.").Set(1);
      inputs.AddField("Outer Radius", "Ro", "Annulus outer radius.").Set(1.5);
      inputs.AddField("Sweep Factor", "Sf", "Annulus sweep factor.").Set(0.75);
    }
    protected override void AddOutputs(OutputAdder outputs)
    {
      outputs.AddGeneric("Annulus curve", "Ac", "Result");
    }

    protected override void Process(IDataAccess access)
    {
      access.GetItem(0, out Plane plane);
      access.GetItem(1, out Field inner);
      access.GetItem(2, out Field outer);
      access.GetItem(3, out Field sweep);

      var ri = inner.ScalarAt(plane.Origin);
      var ro = outer.ScalarAt(plane.Origin);
      var sw = sweep.ScalarAt(plane.Origin);

      access.RectifyPositive(ref ri, "inner radius");
      access.RectifyPositive(ref ro, "outer radius");
      access.RectifyDomain(ref sw, (0, 1), "sweep factor");

      var annulus = new PartialAnnulus(plane, ri, ro, sw);

      access.SetItem(0, annulus);
    }
  }
}