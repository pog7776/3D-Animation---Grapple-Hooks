using UnityEngine;
using System.Collections;

/// <summary>
/// Class used primarily by the SkyscraperRandomizer. Holds data for a base shape,
/// for choosing between
/// </summary>
[System.Serializable]
public class PossibleBase
{
    public string name;
    public bool selected = true;
    public Vector3[] baseVerts;

    public PossibleBase(string name, Vector3[] baseVerts)
    {
        this.name = name;
        this.baseVerts = baseVerts;
    }
}
