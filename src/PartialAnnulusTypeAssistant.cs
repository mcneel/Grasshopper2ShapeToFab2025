using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper2.Data;
using Grasshopper2.Display;
using Grasshopper2.Types.Assistant;
using Grasshopper2.Types.Conversion;

namespace S2FDemo
{
  public sealed class AnnulusConversions : ConversionRepository
  {
    //ConversionDelegate<PartialAnnulus, Point3d>

    public static Merit AnnulusToPoint(PartialAnnulus annulus, out Point3d point, out string message)
    {
      point = annulus.Plane.Origin;
      message = "Point is placed at annulus centre";
      return Merit.Fair;
    }
    // public static Merit PointToAnnulus(Point3d point, out Rectangle3d rec, out string message)
    // {
    //   rec = new Rectangle3d(new Plane(point, Vector3d.XAxis, Vector3d.YAxis), 0.1, 0.2);
    //   message = "C'mon...";
    //   return Merit.Weird;
    // }
  }

  public sealed class PartialAnnulusTypeAssistant : TypeAssistant<PartialAnnulus>
  {
    public PartialAnnulusTypeAssistant() : base("Partial Annulus") { }

    public override PartialAnnulus Copy(PartialAnnulus instance)
    {
      return instance;
    }
    public override int Sort(PartialAnnulus a, PartialAnnulus b)
    {
      return a.Length.CompareTo(b.Length);
    }

    public override string DescribePrimary(Pear<PartialAnnulus> pear)
    {
      return $"Annulus {pear.Item.InnerRadius:0.00##}, {pear.Item.OuterRadius:0.00##}";
    }
    public override string DescribeSecondary(Pear<PartialAnnulus> pear)
    {
      return $"{pear.Item.SweepFactor * 100:0}%";
    }

    public override bool Area(PartialAnnulus instance, out double area)
    {
      area = instance.Area;
      return !double.IsNaN(area);
    }
    public override bool Length(PartialAnnulus instance, out double length)
    {
      length = instance.Length;
      return !double.IsNaN(length);
    }

    public override BoundingBox BoundingBox(PartialAnnulus instance)
    {
      return instance.ToRhinoCurve().GetBoundingBox(true);
    }
    public override BoundingBox TransformBox(PartialAnnulus instance, Transform transformation)
    {
      return instance.ToRhinoCurve().GetBoundingBox(transformation);
    }

    public override object Transform(PartialAnnulus instance, Transform matrix)
    {
      var result = instance.Transform(matrix);
      if (result != null)
        return result;

      var curve = instance.ToRhinoCurve().DuplicateCurve();
      curve.MakeDeformable();
      curve.Transform(matrix);
      return curve;
    }
    public override object Deform(PartialAnnulus instance, SpaceMorph morph)
    {
      var curve = instance.ToRhinoCurve().DuplicateCurve();
      morph.Morph(curve);
      return curve;
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

  public sealed class PartialAnnulusCurveAssistant : CurveAssistant<PartialAnnulus>
  {
    public override Curve ConvertToRhinoCurve(PartialAnnulus instance)
    {
      return instance.ToRhinoCurve();
    }
  }
}