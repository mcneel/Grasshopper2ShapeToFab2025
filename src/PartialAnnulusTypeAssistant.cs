using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper2.Data;
using Grasshopper2.Display;
using Grasshopper2.Types.Assistant;

namespace S2FDemo
{
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

    // ClosestPoint
    // FurthestPoint

    // Transform
    // Deform

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