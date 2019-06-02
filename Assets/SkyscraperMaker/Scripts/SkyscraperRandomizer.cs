using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Works off the SKyscraper script in order to design a skyscraper
/// based on given value ranges.
/// </summary>
[RequireComponent(typeof(Skyscraper))]
[AddComponentMenu("Skyscraper Maker/Skyscraper Randomizer")]
public class SkyscraperRandomizer : MonoBehaviour
{
    //Whether it should generate when initialize or if we should wait until
    //it is called.
    public bool generateOnStart = true;

    //Ranges for many of the Skyscraper values. The script will
    //chosose a float between each min and max. 
    public float minHeight = 10;
    public float maxHeight = 30;

    public float minWidth = 5;
    public float maxWidth = 20;

    public float minLength = 5;
    public float maxLength = 20;

    public float minWindowWidth = .5f;
    public float maxWindowWidth = 1f;

    public float minWindowHeight = .5f;
    public float maxWindowHeight = 1.5f;

    public float minWindowGap = .08f;
    public float maxWindowGap = .2f;

    public float minStoryGap = .05f;
    public float maxStoryGap = .2f;

    //Default options for build shapes. More can be added, but this can
    //give you a starting point.
    public PossibleBase[] standardBases = new PossibleBase[] {
        new PossibleBase("Square", new Vector3[]
        {
            new Vector3(.5f, 0, -.5f),
            new Vector3(.5f, 0, .5f),
            new Vector3(-.5f, 0, .5f),
            new Vector3(-.5f, 0, -.5f)
        }),
        new PossibleBase("Corner Cut", new Vector3[]
        {
            new Vector3(-.5f, 0, -.5f),
            new Vector3(.25f, 0, -.5f),
            new Vector3(.25f, 0, -.25f),
            new Vector3(.5f, 0, -.25f),
            new Vector3(.5f, 0, .5f),
            new Vector3(-.5f, 0, .5f)
        }),
        new PossibleBase("Hexagon", new Vector3[]
        {
            new Vector3(0, 0, -.5f),
            new Vector3(.5f, 0, -.25f),
            new Vector3(.5f, 0, .25f),
            new Vector3(0, 0, .5f),
            new Vector3(-.5f, 0, .25f),
            new Vector3(-.5f, 0, -.25f)
        }),
        new PossibleBase("Trapezoid", new Vector3[]
        {
            new Vector3(-.5f, 0, -.5f),
            new Vector3(.5f, 0, -.5f),
            new Vector3(.25f, 0, .5f),
            new Vector3(-.25f, 0, .5f)
        }),
        new PossibleBase("Heart", new Vector3[]
        {
            new Vector3(-.5f, 0, -.5f),
            new Vector3(-.25f, 0, -.5f),
            new Vector3(0, 0, -.25f),
            new Vector3(.25f, 0, -.5f),
            new Vector3(.5f, 0, -.5f),
            new Vector3(.5f, 0, 0),
            new Vector3(0, 0, .5f),
            new Vector3(-.5f, 0, 0)
        }),
        new PossibleBase("Double Corner Cut", new Vector3[]
        {
            new Vector3(-.5f, 0, -.5f),
            new Vector3(.5f, 0, -.5f),
            new Vector3(.5f, 0, .25f),
            new Vector3(.25f, 0, .25f),
            new Vector3(.25f, 0, .5f),
            new Vector3(-.25f, 0, .5f),
            new Vector3(-.25f, 0, .25f),
            new Vector3(-.5f, 0, .25f)
        })
    };
    
    //Where all the user-defined shapes go
    [SerializeField]
    public List<PossibleBase> customBases = new List<PossibleBase>();

    //Keeps track of which checkboxes are selected
    public bool[] windowStyleOptions = new bool[3] { true, true, true };
    public bool[] roofStyleOptions = new bool[3] { true, true, true };

    //Used only if slanted roof is an option
    public float minSlant = .5f;
    public float maxSlant = 4f;
    public float minRoofBorder = .2f;
    public float maxRoofBorder = 1f;

    //Used only if indented roof is an option
    public float minIndentedRoofHeight = .2f;
    public float maxIndentedRoofHeight = 2f;
    public float minIndent = .1f;
    public float maxIndent = 1f;

    //Lists of colors to choose from for each part of the building
    public List<Color> slantWallColors = new List<Color>
    {
        new Color(.2f, .2f, .2f),
        new Color(.35f, .35f, .35f),
        Color.black
    };

    public List<Color> roofColors = new List<Color> {
        Color.black,
        new Color(.42f, .35f, .35f),
        new Color(.196f, .196f, .293f)
    };

    public List<Color> windowColors = new List<Color> {
        Color.black,
        new Color(.196f, .196f, .294f),
        new Color(.208f, .208f, .251f)
    };

    public List<Color> windowDividerColors = new List<Color>
    {
        Color.black,
        new Color(.1f,.1f,.1f),
        new Color(1f, 1f, 1f)
    };

    public List<Color> cornerColors = new List<Color> {
        Color.white,
        Color.gray,
        new Color(.522f, .459f, .333f)
    };

    public List<Color> storyDividerColors = new List<Color>
    {
        Color.white,
        Color.gray,
        new Color(.1f, .1f, .1f)
    };

    /// <summary>
    /// Generates on start only when told to do so
    /// </summary>
    void Start ()
    { 
        if (generateOnStart)
        {
            GenerateRandomly();
        }
	}

    /// <summary>
    /// Sets up the Skyscraper script and then calls its
    /// Generate() method.
    /// </summary>
    public void GenerateRandomly()
    {
        Skyscraper ss = GetComponent<Skyscraper>();
        SetFields(ss);
        ss.Generate();
    }

    /// <summary>
    /// Changes all the properties of the Skyscraper according to
    /// psuedorandomness.
    /// </summary>
    /// <param name="ss">The skyscraper to modify</param>
    public void SetFields(Skyscraper ss)
    {
        //Set random values based on given limits
        ss.height = Random.Range(minHeight, maxHeight);
        ss.windowWidth = Random.Range(minWindowWidth, maxWindowWidth);
        ss.windowHeight = Random.Range(minWindowHeight, maxWindowHeight);
        ss.floorSpace = Random.Range(minStoryGap, maxStoryGap);

        ss.roofColor = roofColors[Random.Range(0, roofColors.Count)];
        ss.windowColor = windowColors[Random.Range(0, windowColors.Count)];
        ss.cornerColor = cornerColors[Random.Range(0, cornerColors.Count)];
        ss.storyDividerColor = storyDividerColors[Random.Range(0, storyDividerColors.Count)];
        ss.windowDividerColor = windowDividerColors[Random.Range(0, windowDividerColors.Count)];
        ss.baseVertices = BuildBase(ss);

        int windowI = ChooseAmongTrue(windowStyleOptions);
        if (windowI >= 0)
            ss.windowType = (WindowType)windowI;
        else
            Debug.LogWarning("No window type selected. Defaulting to previous type.");

        int roofI = ChooseAmongTrue(roofStyleOptions);
        if (roofI >= 0)
            ss.roofType = (RoofType)roofI;
        else
            Debug.LogWarning("No roof type selected. Defaulting to previous type.");
        if (ss.roofType == RoofType.Slanted)
        {
            ss.roofSlant = Random.Range(minSlant, maxSlant);
            ss.slantRoofBorder = Random.Range(minRoofBorder, maxRoofBorder);
            ss.slantWallColor = slantWallColors[Random.Range(0, slantWallColors.Count)];
            ss.roofPeak = (Direction)Random.Range(0, 4);
        }
        else if (ss.roofType == RoofType.Indented)
        {
            ss.indentedRoofHeight = Random.Range(minIndentedRoofHeight, maxIndentedRoofHeight);
            ss.indent = Random.Range(minIndent, maxIndent);
        }
    }

    /// <summary>
    /// Finds a random index in options, giving equal weight to all
    /// true values and no weight to false values.
    /// </summary>
    /// <param name="options">possible values</param>
    /// <returns>index of a random true option, -1 if all options are false</returns>
    private int ChooseAmongTrue(bool[] options) {
        List<int> indices = new List<int>();
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i])
            {
                indices.Add(i);
            }
        }
        if (indices.Count > 0)
            return indices[Random.Range(0, indices.Count)];
        else
            return -1;
    }

    /// <summary>
    /// Designs a base by picking a base shape and scaling it according to
    /// some random values, and then rotating it randomly.
    /// </summary>
    /// <param name="ss">The skyscraper whose base is being built</param>
    /// <returns></returns>
    private Vector3[] BuildBase(Skyscraper ss)
    {
        float l = Random.Range(minLength, maxLength);
        float w = Random.Range(minWidth, maxWidth);
        List<PossibleBase> possibilities = new List<PossibleBase>();
        foreach(PossibleBase pBase in standardBases)
        {
            if (pBase.selected)
            {
                possibilities.Add(pBase);
            }
        }
        foreach (PossibleBase pBase in customBases)
        {
            if (pBase.selected)
            {
                possibilities.Add(pBase);
            }
        }
        if (possibilities.Count == 0)
        {
            Debug.LogWarning("No base shapes have been selected. Defaulting to the exisiting shape.");
            return ss.baseVertices;
        }

        int randomIndex = Random.Range(0, possibilities.Count);
        Vector3[] chosenBase = (Vector3[])possibilities[randomIndex].baseVerts.Clone();
        int rotationIndex = Random.Range(0, 4);
        for (int i = 0; i < chosenBase.Length; i++)
        {
            chosenBase[i].x *= w;
            chosenBase[i].z *= l;
            switch (rotationIndex) {
                case 0:
                    chosenBase[i] = Quaternion.Euler(0, -90, 0) * chosenBase[i];
                    break;
                case 1:
                    chosenBase[i] = Quaternion.Euler(0, -180, 0) * chosenBase[i];
                    break;
                case 2:
                    chosenBase[i] = Quaternion.Euler(0, -270, 0) * chosenBase[i];
                    break;
            }
        }

        return chosenBase;
    }

}