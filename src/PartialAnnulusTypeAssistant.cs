using System.Collections.Generic;
using Grasshopper2.Data;
using Grasshopper2.Display;
using Grasshopper2.Types.Assistant;
using Rhino.Geometry;

namespace S2FDemo
{
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
}