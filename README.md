# Grasshopper 2 ShapeToFab 2025

The public repo for the Shape To Fabrication 2025 plug-in developer workshop.

Please ensure you can access this repo and compile the C# project therein prior to the workshop. 
Debugging should start a new instance of Rhinoceros 8.

## Shape To Fabrication Dev Workshop Schedule:

1. Creating and Loading plug-ins
  - Setting up a C# plug-in project.
  - Deriving from the Grasshopper2.Framework.Plugin class.
  - Installing plug-ins with the _G2PluginViewer command.
  - Inspecting loaded plug-ins with the viewer.
2. Creating Simple Components
  - Deriving from the Grasshopper2.Components.Component class.
  - Implementing abstract methods.
  - Implementing the default constructor.
  - Inspecting invalid plug-ins with the viewer.
  - Implementing the deserialisation constructor.
  - Attaching the GrasshoperIO.IoIdAttribute.
  - Naming and default conventions for input parameters.
  - Getting and setting values during Process().
3. Creating Intermediate Components
  - Setting default values on inputs.
  - Setting requirements on inputs.
  - Setting presets on inputs.
  - Validating input values in the Process() method.
  - Styling presets using UiName, UiInfo, and UiTint attributes.
  - Proper Process() method design.
  - Switching to twig access.
4. Icon Design
  - Icon naming rules.
  - Creating new icons using Rhino and the G2Icon panel.
  - Using the icon colour space.
  - Using dots to place primitive shapes in icons.
  - Exporting *.ghicon files.
5. Custom Type Development
  - Look at the PartialAnnulus type.
  - Create a component for making new annulus values.
  - Inspect type support using panels, tooltips and components.
  - Implement a basic TypeAssistant<PartialAnnulus>.
  - Implement more advanced type assistant methods, such as: Draw, DescribePrimary, DescribeSecondary, BoundingBox, TransformBox, Transform, Deform, ToGeometry, Length, Area, and ClosestPoint.
  - Implement a basic CurveAssistant<PartialAnnulus>.
  - Change output parameter to curve type.
  - Add gradient pin support and output meta data.
6. Documentation
  - Using the _G2DocAuthoring command to set up authoring tools.
  - Creating example files for component specs.
  - Creating new specs.
  - Creating new glossary terms.
