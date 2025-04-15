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
      inputs.AddInteger("First Number", "A", "First number to add.");
      inputs.AddInteger("Second Number", "B", "Second number to add.");
    }
    protected override void AddOutputs(OutputAdder outputs)
    {
      outputs.AddInteger("Result", "R", "Result of A+B.");
    }

    protected override void Process(IDataAccess access)
    {
      access.GetItem(0, out int A);
      access.GetItem(1, out int B);
      access.SetItem(0, A + B);
    }
  }
}