using Grasshopper2.Components;
using Grasshopper2.UI;
using GrasshopperIO;
using Rhino.Geometry;

namespace S2FDemo
{
  [IoId("ea4cedae-2e7c-4dd4-80be-8b8d6bf5f3fe")]
  public sealed class BoxOffsetComponent : Component
  {
    public BoxOffsetComponent() : base(new Nomen("Box Offset", "Offset one side of a box.")) { }
    public BoxOffsetComponent(IReader reader) : base(reader) { }

    protected override void AddInputs(InputAdder inputs)
    {
      var box = new Box(Plane.WorldXY,
                        new Interval(-1, +1),
                        new Interval(-2, +2),
                        new Interval(0, 1));
      inputs.AddBox("Box", "Bx", "Box to offset.").Set(box);
      inputs.AddInteger("Face Index", "Fi", "Face of box to offset.").Set(0);
      inputs.AddNumber("Depth", "Dp", "Optional depth.", requirement: Grasshopper2.Parameters.Requirement.MayBeMissing);
    }

    protected override void AddOutputs(OutputAdder outputs)
    {
      outputs.AddBox("Box", "Bx", "Modified box.");
    }

    protected override void Process(IDataAccess access)
    {
      access.GetItem(0, out Box box);
      access.GetItem(1, out int face);
      if (!access.GetItem(2, out double depth))
        depth = double.NaN;

      box = OffsetFace(box, (BoxFace)face, depth);
      
      access.SetItem(0, box);
    }

    public enum BoxFace { XMin, XMax, YMin, YMax, ZMin, ZMax }

    public static Box OffsetFace(Box box, BoxFace face, double depth)
    {
      var x = box.X;
      var y = box.Y;
      var z = box.Z;
      switch (face)
      {
        case BoxFace.XMin:
          if (double.IsNaN(depth)) depth = x.Length;
          x = new Interval(x.T0 - depth, x.T0);
          break;

        case BoxFace.XMax:
          if (double.IsNaN(depth)) depth = x.Length;
          x = new Interval(x.T1, x.T1 + depth);
          break;

        case BoxFace.YMin:
          if (double.IsNaN(depth)) depth = y.Length;
          y = new Interval(y.T0 - depth, y.T0);
          break;

        case BoxFace.YMax:
          if (double.IsNaN(depth)) depth = y.Length;
          y = new Interval(y.T1, y.T1 + depth);
          break;

        case BoxFace.ZMin:
          if (double.IsNaN(depth)) depth = z.Length;
          z = new Interval(z.T0 - depth, z.T0);
          break;

        case BoxFace.ZMax:
          if (double.IsNaN(depth)) depth = z.Length;
          z = new Interval(z.T1, z.T1 + depth);
          break;

      }

      x.MakeIncreasing();
      y.MakeIncreasing();
      z.MakeIncreasing();
      return new Box(box.Plane, x, y, z);
    }
  }
}