using Grasshopper2.Components;
using Grasshopper2.UI;
using GrasshopperIO;

namespace S2FDemo
{
  [IoId("72ebb162-5d40-48a3-9d42-e4626a3a3650")]
  public sealed class IntegerAdditionComponent : Component
  {
    public IntegerAdditionComponent()
    : base(new Nomen("Integer Addition", "Add two integer numbers together.", "S2F", "S2F")) { }

    public IntegerAdditionComponent(IReader reader) : base(reader) { }

    protected override void AddInputs(InputAdder inputs)
    {
      inputs.AddInteger("First Number", "A", "First number to add.").Set(1);
      inputs.AddInteger("Second Number", "B", "Second number to add.").Set(1);
      inputs.AddBoolean("Accept Overflow", "Of", "Toggle for overflow results.").Set(true);
    }
    protected override void AddOutputs(OutputAdder outputs)
    {
      outputs.AddInteger("Result", "R", "Result of A+B.");
    }

    protected override void Process(IDataAccess access)
    {
      access.GetItem(0, out int A);
      access.GetItem(1, out int B);
      access.GetItem(2, out bool overflow);

      if (overflow)
        unchecked
        {
          access.SetItem(0, A + B);
        }
      else
      {
        long aa = A;
        long bb = B;
        var result = aa + bb;
        if (result < int.MinValue || result > int.MaxValue)
        {
          access.AddError("Overflow Computation", "These integers are too big to be added together.");
          return;
        }
        access.SetItem(0, (int)result);
      }
    }
  }
}