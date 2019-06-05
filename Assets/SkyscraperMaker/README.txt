**Skyscraper Maker**

This is a tool for procedurally generating skyscrapers. There are two main behavior scripts in this package,
Skyscraper and SkyscraperRandomizer. To quickly get started, attach the Skyscraper script to an empty Game Object.
You will find the script under Component -> Skyscraper Maker -> Skyscraper. You can additionally add a randomizer
script at Component -> Skyscraper Maker -> Skyscraper Randomizer.

**Skyscraper script**

This is the main script for generating a single skyscraper according to a series of parameters. It can either be
generated at runtime by calling the Generate() method or in the Editor by pressing the Generate button in the
inspector. If generated in the Inspector, the assets will be saved to "Assets/SkyscraperMaker/{GameObjectName}-{UniqueID}".
In there, you would find a mesh asset, a texture, and a material. This way, you could modify them and add any details
after initial generation.

The first thing to do is design the base. In the Scene view, with the object selected, there should be a series of
red circles connected with lines. You can drag these points around. In the Inspector, there are buttons for additional
controls to add and remove points as well as reset it back to a simple circle. The placement of the points will
determine the shape of the building.

There are also other parameters that can be set in the Inspector to modify both the geometry and the texturing of the
skyscraper. Here are the parameters and what they do:

 Texture Resolution    -- The number of horizontal (or vertical) pixels per world unit in the generated texture. Note that
                          generating lots of skyscrapers at a high resolutions can take lots of time and eat lots of
						  resources. If you are generating at runtime (usually through the randomizer script), it may be
						  wise to sacrifice here a bit, especially if the buildings will only be seen from a distance.
 Add Collider          -- Attaches a MeshCollider with the generated mesh
 Height                -- The number of world units from the base to the lowest point on the roof.
 Window Width          -- World unit width of a single window. Only applicable with the Window Style of Individual
 Window Height         -- World unit height of each window
 Space Between Floors  -- World unit distance between the top of one window and the bottom of another
 Space Between Windows -- World unit distance between the left of one window and the right of another

 Window Type -- The style of window texturing. Currently there are two options:
    1. Full Width      -- Windows stretch across the entire wall of the building (excluding corners).
    2. Individual      -- Windows have a set width, and the number of windows changes base on the wall width
    3. Vertical Stripe -- The window dividers continue past the floor, stretching from roof to ground

Roof Type -- The shape and style of the roof. Currently there are two options:
    1. Flat    -- Boring flat roof
    2. Slanted -- The roof is angled. This style has a few parameters of its own:
        - Roof Slant Height -- The height of the peak of the roof. The lowest point of the roof is the regular height
        - Roof Peak         -- Direction that should have the highest point of the roof
        - Slant Roof Border -- Thickness of upper trim
        - Slant Wall Color  -- Color to appear above the windows but below the trim.
    3. Indented -- The roof follows the shape of the base, but is somewhat shrunk, creating a ledge around the perimeter
                   This style has its own parameters:
        - Indented Roof Height -- The distance in world units from the ledge to the top
        - Indent			   -- The the width in world units of the ledge

Colors:
    Windows         -- Enough said
    Roof            -- Color of the top face of the building along with trim at the top of the walls
    Window Dividers -- Color of the space between the right of one window and the left of another
    Story Dividers  -- Color of the space between the bottom of one row of windows and the bottom of another
    Corners         -- Color of the extra space to the left and right of window rows


**Skyscraper Randomizer**

This script uses the Skyscraper script but instead takes ranges of possible values and then designs a Skyscraper
by randomly choosing withing those ranges. Here are the parameters and what they do:

 Height -- It will have a random height between the given minimum and maximum.
 Width  -- It will be scaled along the X axis so that it will cover an X distance within this range
 Length -- It will be scaled along the Z axis so that it will cover an X distance within this range
 Window Width -- Range of possible values for the Window Width field
 Window Height -- Range of possible values for the Window Height field
 Window Spacing -- Range of possible values for the Sapce Between Windows field
 Gap Between Stories -- Range of possible values for the Space Between Floors field

 Colors -- Each of the Colors from the Skyscraper will be chosen randomly, but limited to just the colors
          selected here. That way you don't end up with all funky-colored buildings.

 Base Shapes -- Select from several options for building shapes. The default options are as follows:
    1. Square            -- Four walls with right angles
    2. Corner Cut        -- Six walls with right angles. One corner is concave.
    3. Hexagon           -- Six convex walls.
	4. Trapezoid         -- Four-walled. Two walls are parallel, with one larger than the other.
	5. Heart             -- None walls. An indent on one side and a pointed corner on the other.
	6. Double Corner Cut -- Nine walls with right angles. Two corners are concave.
 You can also declare custom shapes by modifying the vertices in the Scene view, typing in a name for this shape,
 and then clicking "Add Current Base Shape." This will take a snapshot of your arrangement and add it to the list
 of possible base shapes. Note that even these custom shapes will be scaled according to the ranges above.

 Window Types and Roof Types -- Check next to the different window and roof types that you would like to possibly
                               see. Checking some roof types will open access to additional range fields that
                               associate with the roof type.


**Example Scene**

The example scene is a small city. If you open it up, you will only see a single building. If you hit run, you should
see sixteen unique buildings, since there a script that duplicates the building, and each building uses the
SkyscraperRandomizer script to modify the Skyscraper parameters before generating. The resoultion is intentionally kept
fairly low on the Skyscraper script since the camera is set up a fair distance away, and generating lots of buildings
can be time-consuming.


**Other Scripts**

There are a few other helper scripts included. They are hidden from the Component menu, but might come in handy:
 - PossibleBase		  - Simple data structure used by the SkyscraperRandomizer. You probably will not use it directly
 - Duplicator		  -	Simple behavior script that spawns a handful of objects in a grid. Used in the demo to create a
                        small city.
 - PolygonDecomposer  -	A script that takes an array of Vector3 objects, and, ignoring the Y values, maps triangles
                        that would cover all the area created by connecting the original points. It does this using
                        the ear clipping algorithm.


**Contact**

To ask questions, report bugs, or share your awesome game that uses SkyscraperMaker, you can contact me by emailing
me at ty@openvisionlabs.com or by leaving a comment at http://gamedev.openvisionlabs.com/skyscraper-maker.