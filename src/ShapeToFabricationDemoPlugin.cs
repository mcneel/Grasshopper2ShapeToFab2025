using System;
using Grasshopper2.UI;

namespace S2FDemo
{
  public sealed class Shape2FabricationDemoPlugin : Grasshopper2.Framework.Plugin
  {
    public Shape2FabricationDemoPlugin()
      : base(new Guid("620fedcb-0e32-4f7b-8f6f-40a4a5a208ad"),
             new Nomen("Shape To Fabrication Demo", "An example plug-in used for the Grasshopper 2 plug-in developer workshop."),
             new Version(1, 0, 0))
    { }

    public override string Author
    {
      get { return "Robert McNeel & Associates"; }
    }
    public override Grasshopper2.UI.Icon.IIcon Icon
    {
      get
      {
        return Grasshopper2.UI.Icon.AbstractIcon.FromResource(
                "ShapeToFabricationPlugin.ghicon",
                typeof(Shape2FabricationDemoPlugin));
      }
    }

    public override string LicenceDescription => "MIT";
    public override string LicenceAgreement => "https://opensource.org/license/mit";
  }
}