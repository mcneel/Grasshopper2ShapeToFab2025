using System;
using Grasshopper2.Components;
using Grasshopper2.Data;
using Grasshopper2.Display;
using Grasshopper2.Extensions;
using Grasshopper2.Types.Assistant;
using Grasshopper2.UI;
using GrasshopperIO;
using Rhino.Geometry;

namespace S2FDemo
{
  [IoId("bb9e9d9d-9b08-4ab0-96f7-0cfdd4d42630")]
  public sealed class FrameCurveComponent : Component
  {
    public FrameCurveComponent() : base(new Nomen("Frame From Values", "Create a frame curve from basic values.", "Demo", "Demo")) { }
    public FrameCurveComponent(IReader reader) : base(reader) { }

    protected override void AddInputs(InputAdder inputs)
    {
      inputs.AddRectangle("Rectangle", "Rc", "Underlying frame rectangle.").Set(new Rectangle3d(Plane.WorldXY, 10, 5));
      inputs.AddNumber("Corner Size", "Cs", "Size of frame corners.").Set(1);
      inputs.AddEnum("Corner Type", "Ct", "Type of frame corners.", FrameCorner.Square);
    }
    protected override void AddOutputs(OutputAdder outputs)
    {
      outputs.AddCurve("Frame", "Fr", "Frame curve.");
    }

    protected override void Process(IDataAccess access)
    {
      access.GetItem(0, out Rectangle3d rec);
      access.GetItem(1, out double size);
      access.GetItem(2, out FrameCorner corner);

      var frame = new FrameCurve(rec, corner, size);
      if (frame.CornerSize != size)
        access.AddWarning("Invalid Corner Size", "The corner size was adjusted to fall within the valid range.");

      access.SetItem(0, frame);
    }
  }

  // Can't make a curve type. CurveAssistant is WAY too hard to implement.
  // Maybe 3D Text?
  // "Hello World" @ x,y,z

  public enum FrameCorner
  {
    Sharp,
    Chamfer,
    Fillet,
    Square,
  }

  [IoId("d64a9d52-64db-40d9-a179-763bcdd8db3c")]
  public sealed class FrameCurve : IStorable
  {
    public FrameCurve(Rectangle3d rectangle, FrameCorner corner, double size)
    {
      Rectangle = rectangle;
      Corner = corner;
      CornerSize = size.Clip(0, 0.25 * Math.Min(Math.Abs(rectangle.Width), Math.Abs(rectangle.Height)));
    }
    public FrameCurve(IReader reader)
    {
      Rectangle = reader.Rectangle(nameof(Rectangle));
      Corner = (FrameCorner)reader.Integer32(nameof(Corner));
      CornerSize = reader.Number64(nameof(CornerSize));
    }
    public void Store(IWriter writer)
    {
      writer.Rectangle(nameof(Rectangle), Rectangle);
      writer.Integer32(nameof(Corner), (int)Corner);
      writer.Number64(nameof(CornerSize), CornerSize);
    }

    public Rectangle3d Rectangle { get; }
    public FrameCorner Corner { get; }
    public double CornerSize { get; }

    public double Length
    {
      get
      {
        if (double.IsNaN(_length))
          _length = ToCurve().GetLength();
        return _length;
      }
    }
    private double _length = double.NaN;

    public Curve ToCurve()
    {
      var curve = _curve;
      if (curve is null)
      {
        if (CornerSize <= 0.0 || Corner == FrameCorner.Sharp)
          curve = new PolylineCurve(Rectangle.ToPolyline());
        else
        {
          var x = Rectangle.Plane.XAxis * CornerSize;
          var y = Rectangle.Plane.YAxis * CornerSize;
          switch (Corner)
          {
            case FrameCorner.Chamfer:
              var chamfer = new Polyline
              {
                Rectangle.Corner(0) + x,
                Rectangle.Corner(1) - x,
                Rectangle.Corner(1) + y,
                Rectangle.Corner(2) - y,
                Rectangle.Corner(2) - x,
                Rectangle.Corner(3) + x,
                Rectangle.Corner(3) - y,
                Rectangle.Corner(0) + y,
                Rectangle.Corner(0) + x
              };
              curve = new PolylineCurve(chamfer);
              break;

            case FrameCorner.Square:
              var square = new Polyline
              {
                Rectangle.Corner(0) + x + y,
                Rectangle.Corner(0) + x,
                Rectangle.Corner(1) - x,
                Rectangle.Corner(1) - x + y,
                Rectangle.Corner(1) + y,
                Rectangle.Corner(2) - y,
                Rectangle.Corner(2) - y - x,
                Rectangle.Corner(2) - x,
                Rectangle.Corner(3) + x,
                Rectangle.Corner(3) + x - y,
                Rectangle.Corner(3) - y,
                Rectangle.Corner(0) + y,
                Rectangle.Corner(0) + x + y,
              };
              curve = new PolylineCurve(square);
              break;

            case FrameCorner.Fillet:
              var fillet = new PolyCurve();
              fillet.Append(new Arc(Rectangle.Corner(0) - y, -y, Rectangle.Corner(0) + x));
              fillet.Append(new Line(Rectangle.Corner(0) + x, Rectangle.Corner(1) - x));
              fillet.Append(new Arc(Rectangle.Corner(1) - x, x, Rectangle.Corner(1) + y));
              fillet.Append(new Line(Rectangle.Corner(1) + y, Rectangle.Corner(2) - y));
              fillet.Append(new Arc(Rectangle.Corner(2) - y, y, Rectangle.Corner(2) - x));
              fillet.Append(new Line(Rectangle.Corner(2) - x, Rectangle.Corner(3) + x));
              fillet.Append(new Arc(Rectangle.Corner(3) + x, -x, Rectangle.Corner(3) - y));
              fillet.Append(new Line(Rectangle.Corner(3) - y, Rectangle.Corner(0) + y));
              fillet.MakeClosed(1e-6);
              curve = fillet;
              break;

            default:
              throw new ArgumentException("Unrecognised corner type: " + Corner);
          }
        }
        _curve = curve;
      }
      return curve;
    }
    private Curve? _curve;
  }

  public sealed class FrameCurveTypeAssistant : TypeAssistant<FrameCurve>
  {
    public FrameCurveTypeAssistant() : base("Frame Curve") { }

    public override FrameCurve Copy(FrameCurve instance)
    {
      return instance;
    }
    public override bool Test(FrameCurve instance, out string? invalidReason)
    {
      if (!instance.Rectangle.IsValid)
      {
        invalidReason = "Invalid underlying rectangle";
        return false;
      }

      if (double.IsNaN(instance.CornerSize))
      {
        invalidReason = "Invalid corner size";
        return false;
      }

      invalidReason = default;
      return true;
    }
    public override int Sort(FrameCurve a, FrameCurve b)
    {
      return a.Length.CompareTo(b.Length);
    }

    public override bool Length(FrameCurve instance, out double length)
    {
      length = instance.Length;
      return true;
    }
    public override bool Area(FrameCurve instance, out double area)
    {
      area = AreaMassProperties.Compute(instance.ToCurve()).Area;
      return true;
    }
    public override bool AreaCentroid(FrameCurve instance, out double area, out Point3d centroid)
    {
      var am = AreaMassProperties.Compute(instance.ToCurve());
      area = am.Area;
      centroid = instance.Rectangle.PointAt(0.5, 0.5);
      return true;
    }

    public override BoundingBox BoundingBox(FrameCurve instance)
    {
      return instance.ToCurve().GetBoundingBox(true);
    }
    public override BoundingBox TransformBox(FrameCurve instance, Transform transformation)
    {
      var curve = instance.ToCurve();
      return curve.GetBoundingBox(transformation);
    }

    public override string DescribePrimary(Pear<FrameCurve> pear)
    {
      return $"w:{pear.Item.Rectangle.Width:0.000} h:{pear.Item.Rectangle.Height:0.000} c:{pear.Item.CornerSize:0.00}";
    }
    public override string DescribeSecondary(Pear<FrameCurve> pear)
    {
      return pear.Item.Corner.ToString();
    }

    public override void Draw(Pear<FrameCurve> pear, DisplayBag bag)
    {
      var curve = new DisplayCurve(pear.Item.ToCurve(), CurveKind.Curve, pear.Meta);
      bag.AddCurve(curve);
    }
  }
}