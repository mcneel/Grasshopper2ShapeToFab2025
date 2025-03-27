## Icons

Any *.ghicon files in this folder will automatically be included  into the plug-in assembly as embedded resources. When the name of an icon equals the name of a component class, it will automatically be assigned to that component. Otherwise, these icons can be loaded from resources using static methods on the `Grasshopper2.UI.Icon.AbstractIcon` class:

```
Grasshopper2.UI.Icon.AbstractIcon.FromResource("Name.ghicon", yourPluginAssembly);
Grasshopper2.UI.Icon.AbstractIcon.FromResource("Name.ghicon", typeInYourPlugin);
```

To create *.ghicon files, start Rhino with Grasshopper 2 installed and open the `GH2 Icon` panel. Use the `Icon Setup` button in the top left of the panel to run the `_G2IconSetup` command and add icon data to the current 3dm file. Running this command will alter the C-Plane and Grid settings in the 3dm file so that snapping to pixel edges and centres becomes easier. While the panel is visible, and while the 3dm file has icon data, a red pixel frame will be drawn in the viewports showing the boundary of the current icon image.

Assign edge or fill properties to any curves which lie inside the icon frame to include them in the icon image. It is also possible to insert standard shapes into the icon using text dots with appropriate notation. The `Icon Symbol` button on the top right of the panel lists the various shapes and provides example of valid notations.

The z-depth of shapes in the icon format may be affected by moving shapes up or down along the world Z axis in Rhino, or by using the `_SendToBack` and `_BringToFront` commands in Rhino.

Once an icon image is satisfactory, click on the red button with the down arrow to export the icon shapes in the current 3dm file to a *.ghicon file.