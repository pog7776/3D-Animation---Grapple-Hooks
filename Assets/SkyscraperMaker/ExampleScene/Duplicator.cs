using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Simple script for creating clones of an object. Uses Skyscraper Maker to
/// demonstrate the randomizer. DO NOT put this on the same object as the one
/// you are duplicating. It you do, each duplication with also make its own
/// duplicates, getting caught in endless recursion.
[AddComponentMenu("")]
public class Duplicator : MonoBehaviour {

    public float xSpace = 20;
    public float zSpace = 20;
    public int xCount = 4;
    public int zCount = 4;

    public GameObject target;
    public GameObject loadingScreen;
    public Slider slider;
    public GameObject generateBtn;
    public CameraMove camMove;

    private List<SkyscraperRandomizer> skyscrapers;


	/// <summary>
    /// Create a number of rows and columns of the object
    /// </summary>
	void Start () {
        StartCoroutine(InstantiateAsync());
	}

    public void Regenerate()
    {
        StartCoroutine(RegenerateAsync());
    }

    IEnumerator InstantiateAsync()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
        if (generateBtn != null)
            generateBtn.SetActive(false);
        if (camMove != null)
            camMove.enabled = false;
        skyscrapers = new List<SkyscraperRandomizer>();
        skyscrapers.Add(target.GetComponent<SkyscraperRandomizer>());
        target.GetComponent<SkyscraperRandomizer>().GenerateRandomly();
        int total = xCount * zCount - 2;
        int current = 1;
        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < zCount; j++)
            {
                if (i != 0 || j != 0)
                {
                    var ob = Instantiate(target, new Vector3(
                        target.transform.position.x + i * xSpace,
                        target.transform.position.y,
                        target.transform.position.z + j * zSpace), Quaternion.identity);
                    skyscrapers.Add(ob.GetComponent<SkyscraperRandomizer>());
                    ob.GetComponent<SkyscraperRandomizer>().GenerateRandomly();
                    slider.value = ((float)current) / total;
                    current++;
                    yield return null;
                }
            }
        }
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
        if (generateBtn != null)
            generateBtn.SetActive(true);
        if (camMove != null)
            camMove.enabled = true;
    }

    IEnumerator RegenerateAsync()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
        if (generateBtn != null)
            generateBtn.SetActive(false);
        if (camMove != null)
            camMove.enabled = false;
        for (int i = 0; i < skyscrapers.Count; i++)
        {
            skyscrapers[i].GenerateRandomly();
            slider.value = ((float) i) / (skyscrapers.Count - 2);
            yield return null;
        }
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
        if (generateBtn != null)
            generateBtn.SetActive(true);
        if (camMove != null)
            camMove.enabled = true;
    }
}
