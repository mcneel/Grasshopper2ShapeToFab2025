# Grasshopper 2 ShapeToFab 2025

The public repo for the Shape To Fabrication 2025 plug-in developer workshop.

Please ensure you can access this repo and compile the C# project therein prior to the workshop. 
Debugging should start a new instance of Rhinoceros 8.

## Shape To Fabrication Dev Workshop Schedule:

1. Creating and Loading plug-ins
  1a. Setting up a C# plug-in project.
  1b. Deriving from the Grasshopper2.Framework.Plugin class.
  1c. Installing plug-ins with the _G2PluginViewer command.
  1d. Inspecting loaded plug-ins with the viewer.

2. Creating Simple Components
  2a. Deriving from the Grasshopper2.Components.Component class.
  2b. Implementing abstract methods.
  2c. Implementing the default constructor.
  2d. Inspecting invalid plug-ins with the viewer.
  2e. Implementing the deserialisation constructor.
  2f. Attaching the GrasshoperIO.IoIdAttribute.
  2g. Naming and default conventions for input parameters.
  2h. Getting and setting values during Process().

3. Creating Intermediate Components
  3a. Setting default values on inputs.
  3b. Setting requirements on inputs.
  3b. Setting presets on inputs.
  3c. Validating input values in the Process() method.
  3d. Styling presets using UiName, UiInfo, and UiTint attributes.
  3e. Proper Process() method design.
  3f. Switching to twig access.

4. Icon Design
  4a. Icon naming rules.
  4b. Creating new icons using Rhino and the G2Icon panel.
  4c. Using the icon colour space.
  4d. Using dots to place primitive shapes in icons.
  4e. Exporting *.ghicon files.

5. Custom Type Development
  5a. Look at the PartialAnnulus type.
  5b. Create a component for making new annulus values.
  5c. Inspect type support using panels, tooltips and components.
  5d. Implement a basic TypeAssistant<PartialAnnulus>.
  5e. Implement more advanced type assistant methods, such as: Draw, DescribePrimary, DescribeSecondary, BoundingBox, TransformBox, Transform, Deform, ToGeometry, Length, Area, and ClosestPoint.
  5f. Implement a basic CurveAssistant<PartialAnnulus>.
  5g. Change output parameter to curve type.
  5h. Add gradient pin support and output meta data.

6. Documentation
  6a. Using the _G2DocAuthoring command to set up authoring tools.
  6b. Creating example files for component specs.
  6c. Creating new specs.
  6d. Creating new glossary terms.
