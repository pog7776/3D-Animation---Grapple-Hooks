using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum WindowType : byte { Individual = 0, FullWidth, VerticalStripe };
public enum RoofType : byte { Flat = 0, Slanted, Indented };
public enum Direction : byte { North = 0, East, South, West };

/// <summary>
/// Main script for the SKyscraper Maker package. Allows generation of skycrapers
/// in the Editor or at runtime.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[AddComponentMenu("Skyscraper Maker/Skyscraper")]
public class Skyscraper : MonoBehaviour
{
    //World Y units from base to where the roof shape begins
    public float height = 30;
    //World units along the wall for adjusting the window size
    public float windowWidth = .75f;
    public float windowHeight = 1;
    //World unit space between the left of one window and the right of another
    public float windowSpace = .1f;
    //World unit space between the top of one window and the bottom of another
    public float floorSpace = .2f;

    //Counter-clockwise from the top, they determine the shape of the building
    public Vector3[] baseVertices = new Vector3[4] {
        new Vector3(-5f, 0, -5f),
        new Vector3(5f, 0, -5f),
        new Vector3(5f, 0, 5f),
        new Vector3(-5f, 0, 5f)
    };
    //Pixel per world unit
    public int resolution = 50;

    //Affects certain color choices in texture generation
    [SerializeField]
    public WindowType windowType = WindowType.Individual;

    //Determines geometry and some texture choices
    [SerializeField]
    public RoofType roofType = RoofType.Flat;

    //Only used for slanted roof
    public float roofSlant = 3f;
    public Direction roofPeak = Direction.North;
    public float slantRoofBorder = .5f;
    public Color slantWallColor = Color.black;

    //Only used for indented roof
    public float indentedRoofHeight = 1f;
    public float indent = .5f;

    //The color that the texture will draw at certain places on the walls and roof
    public Color storyDividerColor = Color.white;
    public Color cornerColor = Color.white;
    public Color roofColor = Color.black;
    public Color windowColor = Color.black;
    public Color windowDividerColor = new Color(.1f, .1f, .1f);
    public Color highStripe = new Color(.3f, .3f, .3f);

    public bool addCollider = true;

    //Used for UV mapping. Do not touch (hence the private)
    private float perimeter;
    private float[] uvBreaks;

    private Texture2D tex;
    private Mesh mesh;

    /// <summary>
    /// Creates a tower based on the given properties. It does not save the generated
    /// assets, though, so this is used for runtime generation
    /// </summary>
    public void Generate()
    {
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        mf.mesh = BuildMesh();
        
        tex = BuildTexture();
        BuildMaterial(tex);
        BuildCollider(mf.mesh);
    }

    /// <summary>
    /// Creates the geometry of the tower by manipulating lots of vertices.
    /// </summary>
    /// <returns></returns>
    public Mesh BuildMesh()
    {
        if (Application.isPlaying)
            Destroy(mesh);
        else
            DestroyImmediate(mesh);
        mesh = new Mesh();

        //Used only if the roof is Indented. Does nothing otherwise
        Vector3[] shrunkenBase = GetShrunkenBase();

        //The components needed to build the mesh
        Vector3[] verts;
        int[] tri;
        Vector3[] normals;
        Vector2[] uv;

        //Size up the base for later calculations
        float minX = baseVertices[0].x, maxX = baseVertices[0].x; //irrelevant for flat roof
        float minZ = baseVertices[0].z, maxZ = baseVertices[0].z;
        foreach (Vector3 vert in baseVertices)
        {
            if (vert.x < minX)
            {
                minX = vert.x;
            }
            if (vert.x > maxX)
            {
                maxX = vert.x;
            }
            if (vert.z < minZ)
            {
                minZ = vert.z;
            }
            if (vert.z > maxZ)
            {
                maxZ = vert.z;
            }
        }

        //Determine the number of vertices and triangle indices for each wall.
        //This way we generate everything wall by wall
        int sideVertLength, sideTriLength;
        if (roofType == RoofType.Flat)
        {
            sideVertLength = 4;
            sideTriLength = 6;
        }
        else if (roofType == RoofType.Slanted)
        {
            sideVertLength = 6;
            sideTriLength = 12;
        }
        else
        {
            sideVertLength = 12;
            sideTriLength = 18;
        }

        //Allocate according to our predicted number of verts and tris per wall
        verts = new Vector3[baseVertices.Length * (sideVertLength + 1)];
        tri = new int[baseVertices.Length * sideTriLength + ((baseVertices.Length - 2) * 3)];
        normals = new Vector3[verts.Length];
        uv = new Vector2[verts.Length];

        //Used for UV mapping. We can calculate this now, but we'll have to go back
        //UV mapping once these are known.
        perimeter = 0;
        uvBreaks = new float[baseVertices.Length];

        //Each loop will start us on the next wall. The vertex is at the bottom-left of
        //the active wall.
        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 nextVert;
            if (i == baseVertices.Length - 1)
            {
                nextVert = baseVertices[0];
            }
            else
            {
                nextVert = baseVertices[i + 1];
            }
            perimeter += Vector3.Distance(baseVertices[i], nextVert);

            //The starting index for our vert and tri mapping
            int vStart = i * sideVertLength;
            int tStart = i * sideTriLength;

            //Every wall will have at least these 4 verts and 2 tris (6 indices)
            verts[vStart] = new Vector3(baseVertices[i].x, 0, baseVertices[i].z);
            verts[vStart + 1] = new Vector3(nextVert.x, 0, nextVert.z);
            verts[vStart + 2] = new Vector3(baseVertices[i].x, height, baseVertices[i].z);
            verts[vStart + 3] = new Vector3(nextVert.x, height, nextVert.z);

            tri[tStart] = vStart;
            tri[tStart + 1] = vStart + 2;
            tri[tStart + 2] = vStart + 1;
            tri[tStart + 3] = vStart + 2;
            tri[tStart + 4] = vStart + 3;
            tri[tStart + 5] = vStart + 1;

            //The normals for each wall will by default by perpendicular to the wall surface
            Vector3 theNormal = Vector3.Cross(Vector3.up, nextVert - baseVertices[i]).normalized;
            for (int j = 0; j < sideVertLength; j++)
            {
                normals[vStart + j] = theNormal;
            }

            if (roofType == RoofType.Flat)
            {
                //Simply make verts and norms for the roof. The tris will come later
                verts[baseVertices.Length * sideVertLength + i] = new Vector3(baseVertices[i].x, height, baseVertices[i].z);
                normals[baseVertices.Length * sideVertLength + i] = Vector3.up;
            }
            else if (roofType == RoofType.Slanted)
            {
                //Get the height of a corner of the roof by finding how
                //far along a given direction the corner lies, and scaling
                //it accordingly
                float addedHeight;
                float nextAddedHeight;
                switch (roofPeak)
                {
                    case Direction.North:
                        addedHeight = (baseVertices[i].z - minZ) / (maxZ - minZ) * roofSlant;
                        nextAddedHeight = (nextVert.z - minZ) / (maxZ - minZ) * roofSlant;
                        break;
                    case Direction.East:
                        addedHeight = (baseVertices[i].x - minX) / (maxX - minX) * roofSlant;
                        nextAddedHeight = (nextVert.x - minX) / (maxX - minX) * roofSlant;
                        break;
                    case Direction.South:
                        addedHeight = (maxZ - baseVertices[i].z) / (maxZ - minZ) * roofSlant;
                        nextAddedHeight = (maxZ - nextVert.z) / (maxZ - minZ) * roofSlant;
                        break;
                    case Direction.West:
                    default:
                        addedHeight = (maxX - baseVertices[i].x) / (maxX - minX) * roofSlant;
                        nextAddedHeight = (maxX - nextVert.x) / (maxX - minX) * roofSlant;
                        break;
                }
                verts[vStart + 4] = new Vector3(baseVertices[i].x, height + addedHeight, baseVertices[i].z);
                verts[vStart + 5] = new Vector3(nextVert.x, height + nextAddedHeight, nextVert.z);

                tri[tStart + 6] = vStart + 2;
                tri[tStart + 7] = vStart + 4;
                tri[tStart + 8] = vStart + 3;
                tri[tStart + 9] = vStart + 4;
                tri[tStart + 10] = vStart + 5;
                tri[tStart + 11] = vStart + 3;

                //add the roof vertex. Will make the tris later
                verts[baseVertices.Length * sideVertLength + i] = new Vector3(baseVertices[i].x, height + addedHeight, baseVertices[i].z);
                normals[baseVertices.Length * sideVertLength + i] = Vector3.up;
            }
            else if (roofType == RoofType.Indented)
            {
                //Uses the shrunken base in order to create a ledge around the perimeter with
                //a smaller flat top above it.
                Vector3 nextShrunkenVert = shrunkenBase[mod(i + 1, shrunkenBase.Length)];

                verts[vStart + 4] = new Vector3(baseVertices[i].x, height, baseVertices[i].z);
                verts[vStart + 5] = new Vector3(nextVert.x, height, nextVert.z);
                verts[vStart + 6] = new Vector3(shrunkenBase[i].x, height, shrunkenBase[i].z);
                verts[vStart + 7] = new Vector3(nextShrunkenVert.x, height, nextShrunkenVert.z);
                for (int j = vStart + 4; j < vStart + 8; j++)
                {
                    normals[j] = Vector3.up;
                }

                verts[vStart + 8] = new Vector3(shrunkenBase[i].x, height, shrunkenBase[i].z);
                verts[vStart + 9] = new Vector3(nextShrunkenVert.x, height, nextShrunkenVert.z);
                verts[vStart + 10] = new Vector3(shrunkenBase[i].x, height + indentedRoofHeight, shrunkenBase[i].z);
                verts[vStart + 11] = new Vector3(nextShrunkenVert.x, height + indentedRoofHeight, nextShrunkenVert.z);

                tri[tStart + 6] = vStart + 4;
                tri[tStart + 7] = vStart + 6;
                tri[tStart + 8] = vStart + 5;
                tri[tStart + 9] = vStart + 6;
                tri[tStart + 10] = vStart + 7;
                tri[tStart + 11] = vStart + 5;

                tri[tStart + 12] = vStart + 8;
                tri[tStart + 13] = vStart + 10;
                tri[tStart + 14] = vStart + 9;
                tri[tStart + 15] = vStart + 10;
                tri[tStart + 16] = vStart + 11;
                tri[tStart + 17] = vStart + 9;

                //add the roof vertex. Will make the tris later
                verts[baseVertices.Length * sideVertLength + i] = new Vector3(shrunkenBase[i].x, height + indentedRoofHeight, shrunkenBase[i].z);
                normals[baseVertices.Length * sideVertLength + i] = Vector3.up;
            }
        }

        //build the roof
        int[] roofTris = roofType == RoofType.Indented ? GetRoofTris(shrunkenBase) : GetRoofTris(baseVertices);

        for (int i = 0; i < roofTris.Length; i++)
        {
            tri[baseVertices.Length * sideTriLength + i] = roofTris[i];
        }

        //The mesh gets all the generated data assigned back
        mesh.vertices = verts;
        mesh.triangles = tri;
        mesh.normals = normals;

        float uvH = 0;
        float roofStart = height / (height + roofSlant) - .005f;

        //make a second sweep for uvs, since the base perimeter is now known
        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 nextVert;
            if (i == baseVertices.Length - 1)
            {
                nextVert = baseVertices[0];
            }
            else
            {
                nextVert = baseVertices[i + 1];
            }
            //We know what percent of the perimeter each wall is, so we can map
            //UVs without distorting the texture, except the roof. The roof is a 
            //solid color, so we stretch the roof color we have at the top of the
            //texture to cover the whole roof.
            int uvStart = i * sideVertLength;
            float newUvH = uvH + Vector3.Distance(baseVertices[i], nextVert) / perimeter;
            uvBreaks[i] = newUvH;
            
            uv[baseVertices.Length * sideVertLength + i] = new Vector2(.001f, .999f);
            if (roofType == RoofType.Slanted)
            {
                uv[uvStart] = new Vector2(uvH, 0);
                uv[uvStart + 1] = new Vector2(newUvH, 0);
                uv[uvStart + 2] = new Vector2(uvH, roofStart);
                uv[uvStart + 3] = new Vector2(newUvH, roofStart);

                uv[uvStart + 4] = new Vector2(uvH, verts[uvStart + 4].y / (height + roofSlant) - .005f);
                uv[uvStart + 5] = new Vector2(newUvH, verts[uvStart + 5].y / (height + roofSlant) - .005f);
            }
            else if (roofType == RoofType.Indented)
            {
                uv[uvStart] = new Vector2(uvH, 0);
                uv[uvStart + 1] = new Vector2(newUvH, 0);
                uv[uvStart + 2] = new Vector2(uvH, .995f);
                uv[uvStart + 3] = new Vector2(newUvH, .995f);

                uv[uvStart + 4] = new Vector2(uvH, .996f);
                uv[uvStart + 5] = new Vector2(newUvH, .996f);
                uv[uvStart + 6] = new Vector2(uvH, .997f);
                uv[uvStart + 7] = new Vector2(newUvH, .997f);
                uv[uvStart + 8] = new Vector2(uvH, .998f);
                uv[uvStart + 9] = new Vector2(newUvH, .998f);
                uv[uvStart + 10] = new Vector2(uvH, .999f);
                uv[uvStart + 11] = new Vector2(newUvH, .999f);
            }
            else
            {
                uv[uvStart] = new Vector2(uvH, 0);
                uv[uvStart + 1] = new Vector2(newUvH, 0);
                uv[uvStart + 2] = new Vector2(uvH, .995f);
                uv[uvStart + 3] = new Vector2(newUvH, .995f);
            }
            uvH = newUvH;
        }
        mesh.uv = uv;
        return mesh;
    }

    /// <summary>
    /// Attaches a mesh collider if it is not already attached, and then assigns the mesh to it.
    /// If the addCollider field is not chosen, however, it will delete the mesh collider.
    /// </summary>
    /// <param name="mesh"></param>
    public void BuildCollider(Mesh mesh)
    {

        MeshCollider col = GetComponent<MeshCollider>();
        if (col == null)
        {
            if (!addCollider)
            {
                return;
            }
            col = gameObject.AddComponent<MeshCollider>();
        }
        else if (!addCollider)
        {
            DestroyImmediate(col, true);
            return;
        }
        col.sharedMesh = mesh;
    }

    /// <summary>
    /// Makes a Standard material with the given texture and assigns it to the skyscraper object
    /// </summary>
    /// <param name="tex">Main Texture</param>
    public void BuildMaterial(Texture2D tex)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.SetTexture("_MainTex", tex);
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    /// <summary>
    /// Uses all the properties relating to windows and borders in order to decorate the mesh
    /// with a texture that doesn't have windows hanging over corners.
    /// </summary>
    /// <returns>The generated texture</returns>
    public Texture2D BuildTexture()
    {
        if (Application.isPlaying)
            Destroy(tex);
        else
            DestroyImmediate(tex);
        Vector3[] verts = GetComponent<MeshFilter>().sharedMesh.vertices;
        if (roofType == RoofType.Slanted)
        {
            //Slanted roofs need more vertical space for the slant wall
            tex = new Texture2D((int)(perimeter * resolution), (int)((height + roofSlant) * resolution * 1.005f));
        }
        else
        {
            tex = new Texture2D((int)(perimeter * resolution), (int)(height * resolution * 1.001f));
        }

        //in-memory array changes faster than SetPixel
        Color[] pixels = tex.GetPixels();
        //Since pixels is flattened, we increment pixelI by one every inner loop
        int pixelI = 0;

        //Used to keep track of what part of the building is currently being drawn
        bool drawFloor = false;
        bool atTop = false;
        float bufferFloor = 0;

        for (int i = 0; i < tex.height; i++)
        {
            //Horizontal-dependant variables reset on a new row of pixels
            float bufferWidth = 0;
            bool drawWindow = false;
            int uvBreakI = 0;
            int wallPixWidthSum = 0;
            int cornerWidth = 0;
            int wallStart = 0;
            int wallEnd = 0;

            for (int j = 0; j < tex.width; j++)
            {
                //If a new wall has been reached, certain things must be reset, and other incremented
                if (j == 0 || j > wallEnd)
                {
                    int wallPixWidth = (int)(uvBreaks[uvBreakI] * tex.width) - wallPixWidthSum;
                    wallPixWidthSum = (int)(uvBreaks[uvBreakI] * tex.width);
                    int windowCount = (int)(wallPixWidth / ((windowWidth + windowSpace) * resolution));
                    int fullWindowWidth = (int)(windowCount * ((windowWidth + windowSpace) * resolution) - (windowSpace * resolution));
                    cornerWidth = (wallPixWidth - fullWindowWidth) / 2;
                    uvBreakI++;
                    if (uvBreakI >= uvBreaks.Length)
                    {
                        uvBreakI = 0;
                    }
                    wallStart = wallEnd;
                    wallEnd = wallPixWidthSum;
                    bufferWidth = 0;
                    drawWindow = true;
                }
                if (atTop)
                {
                    //This is near or on the roof. It should be a solid color unless the roof is slanted
                    if (roofType == RoofType.Slanted)
                    {
                        float h1;
                        float h2 = verts[baseVertices.Length * 6 + uvBreakI].y;
                        if (uvBreakI == 0)
                        {
                            h1 = verts[baseVertices.Length * 7 - 1].y;
                        }
                        else
                        {
                            h1 = verts[baseVertices.Length * 6 + uvBreakI - 1].y;
                        }
                        float wallFraction = (j - wallStart) / (float)(wallEnd - wallStart);
                        float roofHeight = h1 + (h2 - h1) * wallFraction - slantRoofBorder;
                        float roofPixHeight = roofHeight * resolution;
                        if (i > roofPixHeight)
                        {
                            pixels[pixelI] = roofColor;
                        }
                        else
                        {
                            if (j < wallStart + cornerWidth || j > wallEnd - cornerWidth)
                            {
                                pixels[pixelI] = cornerColor;
                            }
                            else
                            {
                                pixels[pixelI] = slantWallColor;
                            }
                        }
                    }
                    else
                    {
                        pixels[pixelI] = roofColor;
                    }
                }
                else if (j < wallStart + cornerWidth || j > wallEnd - cornerWidth)
                {
                    //Too close to a corner for windows. Draw a border instead.
                    pixels[pixelI] = cornerColor;
                }
                else if (drawFloor || windowType == WindowType.VerticalStripe)
                {
                    //Draw windows
                    if (drawWindow)
                    {
                        if (drawFloor)
                        {
                            pixels[pixelI] = windowColor;
                        }
                        else
                        {
                            pixels[pixelI] = storyDividerColor;
                        }
                    }
                    else
                    {
                        if (windowType != WindowType.FullWidth)
                        {
                            pixels[pixelI] = windowDividerColor;
                        }
                        else
                        {
                            pixels[pixelI] = windowColor;
                        }
                    }
                    bufferWidth += 1f / resolution;
                    if (drawWindow && bufferWidth >= windowWidth)
                    {
                        bufferWidth -= windowWidth;
                        drawWindow = false;
                    }
                    else if (!drawWindow && bufferWidth >= windowSpace)
                    {
                        bufferWidth -= windowSpace;
                        drawWindow = true;
                    }
                }
                else
                {
                    //In between floors. Do not draw windows
                    pixels[pixelI] = storyDividerColor;
                }
                pixelI++;
            }
            //Determine if you should toggle drawing windows or if
            //You are approaching the roof
            bufferFloor += 1f / resolution;
            if (drawFloor && bufferFloor >= windowHeight)
            {
                bufferFloor -= windowHeight;
                drawFloor = false;
            }
            else if (!drawFloor && bufferFloor >= floorSpace && !atTop)
            {
                float topSpace = windowHeight + floorSpace + height * .005f;
                topSpace = roofType == RoofType.Slanted ? topSpace + roofSlant : topSpace;
                if (i > tex.height - (topSpace) * resolution - 5)
                {
                    atTop = true;
                }
                else
                {
                    bufferFloor -= floorSpace;
                    drawFloor = true;
                }
            }
        }
        //All done. Build the texture
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// Decomposes the polygon base into triangles, and then
    /// adjusts it based on the number of vertices in the mesh
    /// </summary>
    /// <param name="source">Base vertices</param>
    /// <returns>indices to verts. Sets of three going clockwise</returns>
    private int[] GetRoofTris(Vector3[] source)
    {
        int[] tris = new PolygonDecomposer(source).Decompose();
        for (int i = 0; i < tris.Length; i++)
        {
            if (roofType == RoofType.Flat)
            {
                tris[i] += baseVertices.Length * 4;
            }
            else if (roofType == RoofType.Slanted)
            {
                tris[i] += baseVertices.Length * 6;
            } else if (roofType == RoofType.Indented)
            {
                tris[i] += baseVertices.Length * 12;
            }
        }
        return tris;
    }

    /// <summary>
    /// Moves all the vertices inward to find a similar but smaller shape
    /// </summary>
    /// <returns>the verts that are closer together</returns>
    private Vector3[] GetShrunkenBase()
    {
        Vector3[] result = new Vector3[baseVertices.Length];
        if (roofType == RoofType.Indented)
        {
            for (int i = 0; i < baseVertices.Length; i++)
            {
                //Get the new vertex by getting two line inside but parallel to the base lines
                Vector3 curVert = baseVertices[i];
                Vector3 prevVert = baseVertices[mod(i - 1, baseVertices.Length)];
                Vector3 nextVert = baseVertices[mod(i + 1, baseVertices.Length)];
                Vector3 d1 = nextVert - curVert;
                Vector3 d2 = curVert - prevVert;
                Vector3 p1 = curVert + new Vector3(-d1.z, 0, d1.x) / Mathf.Sqrt(d1.x * d1.x + d1.z * d1.z) * indent;
                Vector3 p2 = prevVert + new Vector3(-d2.z, 0, d2.x) / Mathf.Sqrt(d2.x * d2.x + d2.z * d2.z) * indent;
                Vector3 intersection;
                if (!GetIntersection(out intersection, p1, d1, p2, d2))
                {
                    //Lines are parallel, so we can just use this point
                    intersection = p1;
                }
                result[i] = intersection;
            }
        }
        return result;
    }

    /// <summary>
    /// Finds an intersection between two lines. Used by GetShrunkenBase()
    /// </summary>
    /// <param name="intersection">Where to store the new point</param>
    /// <param name="p1">Point on line 1</param>
    /// <param name="d1">Direction vector of line 1</param>
    /// <param name="p2">Point on line 2</param>
    /// <param name="d2">Direction vector of line 2</param>
    /// <returns>Whether an intersection exists</returns>
    private bool GetIntersection(out Vector3 intersection, Vector3 p1, Vector3 d1, Vector3 p2, Vector3 d2)
    {

        Vector3 d3 = p2 - p1;
        Vector3 crossd1_2 = Vector3.Cross(d1, d2);
        Vector3 crossd3_2 = Vector3.Cross(d3, d2);

        float planarFactor = Vector3.Dot(d3, crossd1_2);

        //is coplanar, and not parrallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossd1_2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossd3_2, crossd1_2) / crossd1_2.sqrMagnitude;
            intersection = p1 + (d1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    /// <summary>
    /// A true mod function with no negatives
    /// </summary>
    /// <param name="a">left side</param>
    /// <param name="b">right side</param>
    /// <returns>a % b, but positive</returns>
    int mod(int a, int b)
    {
        return (a % b + b) % b;
    }
}
