using Eto.Forms;
using Grasshopper2.Components;
using Grasshopper2.Types.Fields;
using Grasshopper2.UI;
using GrasshopperIO;
using Rhino.Geometry;

namespace S2FDemo
{
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