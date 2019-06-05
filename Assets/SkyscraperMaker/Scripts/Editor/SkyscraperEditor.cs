using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Skyscraper))]
public class SkyscraperEditor : Editor {

    /// <summary>
    /// Draws handles for each vertex in the base and draws lines between them. Also previews the height
    /// with additional lines.
    /// </summary>
    void OnSceneGUI()
    {
        if (Camera.current != null)
        {
            Handles.color = Color.red;
            Skyscraper tar = (Skyscraper)target;
            for (int i = 0; i < tar.baseVertices.Length; i++)
            {
                Vector3 pos = tar.baseVertices[i];
                if (MoveHandle(tar, ref pos))
                {
                    Undo.RecordObject(tar, "Move Vertex");
                    tar.baseVertices[i] = pos;
                    EditorUtility.SetDirty(tar);
                }
                Handles.DrawLine(tar.transform.TransformPoint(tar.baseVertices[i]),
                        tar.transform.TransformPoint(tar.baseVertices[i] + Vector3.up * tar.height));
                if (i > 0)
                {
                    Handles.DrawLine(tar.transform.TransformPoint(tar.baseVertices[i - 1] + Vector3.up * tar.height), 
                            tar.transform.TransformPoint(pos + Vector3.up * tar.height));
                    Handles.DrawLine(tar.transform.TransformPoint(tar.baseVertices[i - 1]),
                            tar.transform.TransformPoint(pos));
                }
                else
                {
                    Handles.DrawLine(tar.transform.TransformPoint(tar.baseVertices[tar.baseVertices.Length - 1] + Vector3.up * tar.height),
                            tar.transform.TransformPoint(pos + Vector3.up * tar.height));
                    Handles.DrawLine(tar.transform.TransformPoint(tar.baseVertices[tar.baseVertices.Length - 1]),
                            tar.transform.TransformPoint(pos));
                }
            }
        }
    }

    /// <summary>
    /// Takes care of vertex handle movement, locking them along the Y axis.
    /// </summary>
    /// <param name="tar">The skyscraper to modify</param>
    /// <param name="pos">Position of the handle</param>
    /// <returns></returns>
    public bool MoveHandle(Skyscraper tar, ref Vector3 pos)
    {
        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.FreeMoveHandle(tar.transform.TransformPoint(pos), Quaternion.identity, .5f, new Vector3(1f, 1f, 1f), Handles.CircleCap);

        //To allow rotations, everything will be done locally
        newPos = tar.transform.InverseTransformPoint(newPos);
        Vector3 camPos = tar.transform.InverseTransformPoint(Camera.current.transform.position);    
        
        Vector3 posDiff = camPos - newPos;
        float factor = camPos.y / posDiff.y;

        newPos = new Vector3(camPos.x - posDiff.x * factor,
                0,
                camPos.z - posDiff.z * factor);

        pos = newPos;
        return EditorGUI.EndChangeCheck();
    }

    /// <summary>
    /// Allows some additional controls to the Inspector, such as vertex manipulation
    /// control and and generation button. It also hides fields that are irrelevant
    /// because of other field selections.
    /// </summary>
    public override void OnInspectorGUI()
    {
        Skyscraper tar = (Skyscraper)target;

        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button("Reset Base"))
        {
            ResetBase(tar);
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Base Vertex"))
        {
            AddBaseVertex(tar);
        }
        if (tar.baseVertices.Length > 3 && GUILayout.Button("Remove Base Vertex"))
        {
            RemoveBaseVertex(tar);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        tar.resolution = EditorGUILayout.IntField("Texture Resolution", tar.resolution);
        tar.addCollider = EditorGUILayout.Toggle("Add Collider", tar.addCollider);

        tar.height = EditorGUILayout.FloatField("Height", tar.height);
        tar.windowWidth = EditorGUILayout.FloatField("Window Width", tar.windowWidth);
        tar.windowHeight = EditorGUILayout.FloatField("Window Height", tar.windowHeight);
        tar.floorSpace = EditorGUILayout.FloatField("Space Between Floors", tar.floorSpace);
        tar.windowSpace = EditorGUILayout.FloatField("Space Between Windows", tar.windowSpace);

        tar.windowType = (WindowType)EditorGUILayout.EnumPopup("Window Type", tar.windowType);
        tar.roofType = (RoofType)EditorGUILayout.EnumPopup("Roof Type", tar.roofType);
        if (tar.roofType == RoofType.Slanted)
        {
            tar.roofSlant = EditorGUILayout.FloatField("Roof Slant Height", tar.roofSlant);
            tar.roofPeak = (Direction)EditorGUILayout.EnumPopup("Roof Peak", tar.roofPeak);
            tar.slantRoofBorder = EditorGUILayout.FloatField("Slant Roof Border", tar.slantRoofBorder);
            tar.slantWallColor = EditorGUILayout.ColorField("Slant Wall Color", tar.slantWallColor);
        }
        else if (tar.roofType == RoofType.Indented)
        {
            tar.indentedRoofHeight = EditorGUILayout.FloatField("Indented Roof Height", tar.indentedRoofHeight);
            tar.indent = EditorGUILayout.FloatField("Indent", tar.indent);
        }

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);
        tar.windowColor = EditorGUILayout.ColorField("Windows", tar.windowColor);
        tar.roofColor = EditorGUILayout.ColorField("Roof", tar.roofColor);
        tar.windowDividerColor = EditorGUILayout.ColorField("Window Dividers", tar.windowDividerColor);
        tar.storyDividerColor = EditorGUILayout.ColorField("Story Dividers", tar.storyDividerColor);
        tar.cornerColor = EditorGUILayout.ColorField("Corners", tar.cornerColor);

        EditorGUI.EndChangeCheck();
        Undo.RecordObject(tar, "Modified Skyscraper");

        EditorGUILayout.Separator();
        if (GUILayout.Button("Generate"))
        {
            GenerateAndSave(tar);
        }
    }

    /// <summary>
    /// Rather than use Skyscraper's Generate() method, this goes
    /// through the process step by step in order to update a
    /// progress bar. Then it save the generated content as assets
    /// and assigns them back to the object.
    /// </summary>
    /// <param name="tar">The skyscraper to modify</param>
    public static void GenerateAndSave(Skyscraper tar)
    {
        EditorUtility.DisplayProgressBar("Generating Tower", "Generating Mesh", 0);
        MeshRenderer r = tar.GetComponent<MeshRenderer>();
        try
        {
            MeshFilter mf = tar.GetComponent<MeshFilter>();
            mf.mesh = tar.BuildMesh();

            EditorUtility.DisplayProgressBar("Generating Tower", "Generating Material", .33f);

            Texture2D genTex = tar.BuildTexture();
            tar.BuildMaterial(genTex);

            EditorUtility.DisplayProgressBar("Generating Tower", "Saving assets", .67f);
        }
        catch (UnityException ex)
        {
            Debug.LogError("An error occurs when generating the skyscraper: " + ex.ToString());
        }

        try
        {
            Texture2D tex = (Texture2D)tar.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
            string dir = "Assets/SkyscraperMaker/Skyscrapers/" + tar.name + tar.GetInstanceID();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(dir + "/" + tar.name + "_wall_texture.png", bytes);
            AssetDatabase.CreateAsset(tar.GetComponent<MeshFilter>().sharedMesh, dir + "/" + tar.name + "_mesh.asset");
            AssetDatabase.CreateAsset(r.sharedMaterial, dir + "/" + tar.name + "_material.mat");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //Now that everything exists as an asset, we make assignments to link everything together
            Material material = (Material)(AssetDatabase.LoadAssetAtPath(dir + "/" + tar.name + "_material.mat", typeof(Material)));
            Texture2D mainTex = (Texture2D)(AssetDatabase.LoadAssetAtPath(dir + "/" + tar.name + "_wall_texture.png", typeof(Texture2D)));
            material.mainTexture = mainTex;
            tar.BuildCollider(tar.GetComponent<MeshFilter>().sharedMesh);
            AssetDatabase.SaveAssets();
        }
        catch (UnityException ex)
        {
            Debug.LogError("Unable to save the generated assets at this time: " + ex.ToString());
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Undo.RecordObject(tar, "Built Skyscraper");
    }

    /// <summary>
    /// Returns the base back to the default shape and size
    /// </summary>
    /// <param name="tar">The skyscraper to modify</param>
    private void ResetBase(Skyscraper tar)
    {
        Vector3[] newBase = new Vector3[]
            {
                new Vector3(5f, 0, -5f),
                new Vector3(5f, 0, 5f),
                new Vector3(-5f, 0, 5f),
                new Vector3(-5f, 0, -5f)
            };
        tar.baseVertices = newBase;
        EditorUtility.SetDirty(target);
    }

    /// <summary>
    /// Places another vertex equidistant to the first and last
    /// existing vertices
    /// </summary>
    /// <param name="tar">The skyscraper to modify</param>
    private void AddBaseVertex(Skyscraper tar)
    {
        Vector3[] newBase = new Vector3[tar.baseVertices.Length + 1];
        for (int i = 0; i < tar.baseVertices.Length; i++)
        {
            newBase[i] = tar.baseVertices[i];
        }
        newBase[newBase.Length - 1] = new Vector3(
                (newBase[0].x + newBase[newBase.Length - 2].x) / 2,
                0,
                (newBase[0].z + newBase[newBase.Length - 2].z) / 2);
        tar.baseVertices = newBase;
        EditorUtility.SetDirty(target);
    }

    /// <summary>
    /// Removes the last vertex of the base
    /// </summary>
    /// <param name="tar">The skyscraper to modify</param>
    private void RemoveBaseVertex(Skyscraper tar)
    {
        Vector3[] newBase = new Vector3[tar.baseVertices.Length - 1];
        for (int i = 0; i < tar.baseVertices.Length - 1; i++)
        {
            newBase[i] = tar.baseVertices[i];
        }
        tar.baseVertices = newBase;
        EditorUtility.SetDirty(target);
    }
}
