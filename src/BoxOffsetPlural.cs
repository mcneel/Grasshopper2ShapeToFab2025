using System;
using System.Collections.Generic;
using Grasshopper2.Components;
using Grasshopper2.Data;
using Grasshopper2.Extensions;
using Grasshopper2.Parameters;
using Grasshopper2.UI;
using GrasshopperIO;
using Rhino.Geometry;

namespace S2FDemo
{
  [IoId("0b110cbe-5cbf-4b8a-baed-134a9f814850")]
  public sealed class BoxOffsetPlural : Component
  {
    public BoxOffsetPlural() : base(new Nomen("Box Offset Plural", "Offset sides of a box.", "S2F", "S2F")) { }
    public BoxOffsetPlural(IReader reader) : base(reader) { }

    protected override void AddInputs(InputAdder inputs)
    {
      var box = new Box(Plane.WorldXY,
                        new Interval(-1, +1),
                        new Interval(-2, +2),
                        new Interval(0, 1));
      inputs.AddBox("Box", "Bx", "Box to offset.").Set(box);
      inputs.AddEnum("Face Indices", "Fi", "Faces of box sequence to offset.", BoxFace.XMin, Access.Twig);
      inputs.AddNumber("Depth", "Dp", "Optional depth.", Access.Twig);
    }

    protected override void AddOutputs(OutputAdder outputs)
    {
      outputs.AddBox("Box", "Bx", "Modified box.", Access.Twig);
    }

    protected override void Process(IDataAccess access)
    {
      access.GetItem(0, out Box box);
      access.GetTwig(1, out Twig<BoxFace> faces);
      access.GetTwig(2, out Twig<double> depths);
      access.VerifyEqualTwigLeafCount(faces, depths, "faces", "depths");

      var boxes = new List<Pear<Box>>();
      for (int i = 0; i < Math.Max(faces.LeafCount, depths.LeafCount); i++)
      {
        var f = i.Clip(0, faces.LeafCount - 1);
        var d = i.Clip(0, depths.LeafCount - 1);

        if (faces.NullAt(f) || depths.NullAt(d))
          boxes.Add(default);
        else
        {
          box = OffsetFace(box, faces.ItemAt(f), depths.ItemAt(d));
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