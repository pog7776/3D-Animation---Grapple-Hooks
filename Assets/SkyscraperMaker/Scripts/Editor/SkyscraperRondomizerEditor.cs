using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Allows the editing of ranges of possible values used by the
/// SkyscraperRandomizer script.
/// </summary>
[CustomEditor(typeof(SkyscraperRandomizer))]
public class SkyscraperRondomizerEditor : Editor
{
    //for the foldouts
    bool showRoofColors = false;
    bool showWindowColors = false;
    bool showWindowDividerColors = false;
    bool showCornerColors = false;
    bool showStoryDividerColors = false;
    bool showPossibleBases = false;
    bool showPossibleWindowTypes = false;
    bool showPossibleRoofTypes = false;
    bool showSlantWallColors = true;

    string newBaseName = "New Base Name";

    static string[] roofTypeNames = new string[3] { "Flat", "Slanted", "Indented" };
    static string[] windowTypeNames = new string[3] { "Individual", "Full Width", "Vertical Stripe" };

    /// <summary>
    /// Creates the inspector with lots of controls. for various ranges.
    /// </summary>
    public override void OnInspectorGUI()
    {
        SkyscraperRandomizer tar = (SkyscraperRandomizer)target;

        EditorGUI.BeginChangeCheck();
        tar.generateOnStart = EditorGUILayout.Toggle("Generate on Start", tar.generateOnStart);

        DisplayRangeControl("Height", ref tar.minHeight, ref tar.maxHeight);
        DisplayRangeControl("Width", ref tar.minWidth, ref tar.maxWidth);
        DisplayRangeControl("Length", ref tar.minLength, ref tar.maxLength);
        DisplayRangeControl("Window Width", ref tar.minWindowWidth, ref tar.maxWindowWidth);
        DisplayRangeControl("Window Height", ref tar.minWindowHeight, ref tar.maxWindowHeight);
        DisplayRangeControl("Window Spacing", ref tar.minWindowGap, ref tar.maxWindowGap);
        DisplayRangeControl("Gap Between Stories", ref tar.minStoryGap, ref tar.maxStoryGap);

        EditorGUILayout.Separator();
        showRoofColors = DisplayColorGroup("Roof Colors", tar.roofColors, showRoofColors);
        showWindowColors = DisplayColorGroup("Window Colors", tar.windowColors, showWindowColors);
        showWindowDividerColors = DisplayColorGroup("Window Divider Colors", tar.windowDividerColors, showWindowDividerColors);
        showCornerColors = DisplayColorGroup("Corner Colors", tar.cornerColors, showCornerColors);
        showStoryDividerColors = DisplayColorGroup("Story Divider Colors", tar.storyDividerColors, showStoryDividerColors);

        //Allows the editing of base shape options as well as the registering of new ones
        EditorGUILayout.Separator();
        showPossibleBases = EditorGUILayout.Foldout(showPossibleBases, "Base Shapes");
        if (showPossibleBases)
        {
            foreach (PossibleBase possibleBase in tar.standardBases)
            {
                possibleBase.selected = EditorGUILayout.Toggle(possibleBase.name, possibleBase.selected);
            }
            for (int i = 0; i < tar.customBases.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                tar.customBases[i].selected = EditorGUILayout.Toggle(tar.customBases[i].name, tar.customBases[i].selected);
                if (GUILayout.Button("Delete"))
                {
                    tar.customBases.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            newBaseName = EditorGUILayout.TextField(newBaseName);
            if (GUILayout.Button("Add Current Base Shape"))
            {
                Vector3[] verts = (Vector3[])tar.GetComponent<Skyscraper>().baseVertices.Clone();
                RescaleBase(verts);
                tar.customBases.Add(new PossibleBase(newBaseName, verts));
            }
        }

        //Allows you to pick and choose different possible window types
        EditorGUILayout.Separator();
        showPossibleWindowTypes = EditorGUILayout.Foldout(showPossibleWindowTypes, "Window Types");
        if (showPossibleWindowTypes)
        {
            for (int i = 0; i < tar.windowStyleOptions.Length; i++)
            {
                tar.windowStyleOptions[i] = EditorGUILayout.Toggle(windowTypeNames[i], tar.windowStyleOptions[i]);
            }
        }

        //Allows you to pick and choose different possible roof types
        showPossibleRoofTypes = EditorGUILayout.Foldout(showPossibleRoofTypes, "Roof Types");
        if (showPossibleRoofTypes)
        {
            for (int i = 0; i < tar.roofStyleOptions.Length; i++)
            {
                tar.roofStyleOptions[i] = EditorGUILayout.Toggle(roofTypeNames[i], tar.roofStyleOptions[i]);
            }
            if (tar.roofStyleOptions[(int)RoofType.Slanted])
            {
                DisplayRangeControl("Roof Slant", ref tar.minSlant, ref tar.maxSlant);
                DisplayRangeControl("Slant Roof Border", ref tar.minRoofBorder, ref tar.maxRoofBorder);
                showSlantWallColors = DisplayColorGroup("Slant Wall Colors", tar.slantWallColors, showSlantWallColors);
            }
            if (tar.roofStyleOptions[(int)RoofType.Indented])
            {
                DisplayRangeControl("Indented Roof Height", ref tar.minIndentedRoofHeight, ref tar.maxIndentedRoofHeight);
                DisplayRangeControl("Indentation", ref tar.minIndent, ref tar.maxIndent);
            }
        }

        //Button for randomizing in the Editor
        if (GUILayout.Button("Randomly Generate"))
        {
            Skyscraper ss = tar.GetComponent<Skyscraper>();
            tar.SetFields(ss);
            SkyscraperEditor.GenerateAndSave(ss);
            EditorUtility.SetDirty(target);
        }
        Undo.RecordObject(tar, "Modified Skyscraper Randomizer");
    }

    /// <summary>
    /// Gets the shape of verts and scales it so it has a width
    /// and length of one
    /// </summary>
    /// <param name="verts">Collection of polygon vertices</param>
    private void RescaleBase(Vector3[] verts)
    {
        float xMin = verts[0].x;
        float xMax = verts[0].x;
        float zMin = verts[0].z;
        float zMax = verts[0].z;
        foreach (Vector3 vert in verts)
        {
            if (vert.x < xMin)
            {
                xMin = vert.x;
            }
            else if (vert.x > xMax)
            {
                xMax = vert.x;
            }
            if (vert.z < zMin)
            {
                zMin = vert.z;
            }
            else if (vert.z > zMax)
            {
                zMax = vert.z;
            }
        }
        float xScale = xMax - xMin;
        float zScale = zMax - zMin;
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i].x /= xScale;
            verts[i].z /= zScale;
        }
    }

    /// <summary>
    /// Editor control for a min and max value
    /// </summary>
    /// <param name="label">Text to appear above the input group</param>
    /// <param name="min">minimum var of the range</param>
    /// <param name="max">maximum var of the range</param>
    private void DisplayRangeControl(string label, ref float min, ref float max)
    {
        float defaultLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 50;
        min = EditorGUILayout.FloatField("Min", min);
        max = EditorGUILayout.FloatField("Max", max);
        EditorGUIUtility.labelWidth = defaultLabelWidth;
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// An Editor control for modifying a list of colors
    /// </summary>
    /// <param name="label">Text to appear above the group</param>
    /// <param name="colorList">Holds the data for the colors</param>
    /// <param name="foldout">whether to display the list or just the label</param>
    /// <returns>new foldout value</returns>
    private bool DisplayColorGroup(string label, List<Color> colorList, bool foldout)
    {
        foldout = EditorGUILayout.Foldout(foldout, label);
        if (foldout)
        {
            for (int i = 0; i < colorList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                colorList[i] = EditorGUILayout.ColorField(colorList[i]);
                if (GUILayout.Button("Remove"))
                {
                    colorList.RemoveAt(i);
                    return foldout;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add Color"))
            {
                colorList.Add(Color.white);
            }
        }
        return foldout;
    }
}
