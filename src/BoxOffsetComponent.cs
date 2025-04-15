using System.Collections.Generic;
using Eto.Drawing;
using Grasshopper2.Components;
using Grasshopper2.Data;
using Grasshopper2.Parameters;
using Grasshopper2.Types.Fields.Standard;
using Grasshopper2.UI;
using GrasshopperIO;
using Rhino.Geometry;

namespace S2FDemo
{
  [IoId("ea4cedae-2e7c-4dd4-80be-8b8d6bf5f3fe")]
  public sealed class BoxOffsetComponent : Component
  {
    public BoxOffsetComponent() : base(new Nomen("Box Offset", "Offset one side of a box.", "S2F", "S2F")) { }
    public BoxOffsetComponent(IReader reader) : base(reader) { }

    protected override void AddInputs(InputAdder inputs)
    {
      var box = new Box(Plane.WorldXY,
                        new Interval(-1, +1),
                        new Interval(-2, +2),
                        new Interval(0, 1));
      inputs.AddBox("Box", "Bx", "Box to offset.").Set(box);
      inputs.AddEnum("Face Indices", "Fi", "Faces of box sequence to offset.", BoxFace.XMin, Access.Twig);
      inputs.AddNumber("Depth", "Dp", "Optional depth.", requirement: Requirement.MayBeMissing);

      // 1. Add a gap spacing as an input
      // 2. Make the depth input a Twig as well
      // 3. Add an integer input for repeated offset
    }

    protected override void AddOutputs(OutputAdder outputs)
    {
      outputs.AddBox("Box", "Bx", "Modified box.", Access.Twig);
    }

    protected override void Process(IDataAccess access)
    {
      access.GetItem(0, out Box box);
      access.GetTwig(1, out Twig<BoxFace> faces);
      if (!access.GetItem(2, out double depth))
        depth = double.NaN;

      var boxes = new List<Pear<Box>>();
      for (int i = 0; i < faces.LeafCount; i++)
      {
        if (faces.NullAt(i))
          boxes.Add(default);
        else
        {
          box = OffsetFace(box, faces.ItemAt(i), depth);
          boxes.Add(Garden.Pear(box));
        }
      }

      access.SetTwig(0, Garden.TwigFromPears(boxes));
    }

    public enum BoxFace
    {
      [UiName("Left"), UiInfo("dlfkhvkdfhv"), UiTint("Red9")]
      XMin,
      [UiName("Right"), UiInfo("dlfkhvkdfhv"), UiTint("Red6")]
      XMax,
      [UiName("Front"), UiInfo("dlfkhvkdfhv"), UiTint("Green9")]
      YMin,
      [UiName("Back"), UiInfo("dlfkhvkdfhv"), UiTint("Green6")]
      YMax,
      [UiName("Bottom"), UiInfo("dlfkhvkdfhv"), UiTint("Blue9")]
      ZMin,
      [UiName("Top"), UiInfo("dlfkhvkdfhv"), UiTint("Blue6")]
      ZMax
    }

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