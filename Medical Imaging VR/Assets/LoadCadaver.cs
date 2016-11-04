#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/**
 * This script is responsible for loading the cadaver dataset specified by the legend being loaded. 
 * It writes an ouput vol tex data file that can then be set in material.
 */
public class LoadCadaver : MonoBehaviour
{   
    private List<Color> imageColors;
    private static string folderPath = "Cadaver/";

    // Use this for initialization
    void Start()
    {
        //Get a ref to the legend loader script.
        LoadLegend legendLoader = GetComponent<LoadLegend>();
        imageColors = new List<Color>();

        int startIndex = legendLoader.startIndex;
        int endIndex = legendLoader.endIndex;
        int indexIncrement = legendLoader.indexIncrement;

        //Precalc the tex dimensions by loading the first image.
        Texture2D first = Resources.Load(folderPath + startIndex.ToString().PadLeft(4, '0')) as Texture2D;
        int width = (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(first.width) / System.Math.Log(2)));
        int height = (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(first.height) / System.Math.Log(2)));
        int numImages = (endIndex - startIndex + 1) / indexIncrement;

        //need to correspond with legend's z-axis
        for (int i = startIndex; i <= endIndex; i+=indexIncrement)
        {
            Texture2D anImage = new Texture2D(width, height);
            Texture2D temp = Resources.Load(folderPath + i.ToString().PadLeft(4, '0')) as Texture2D;
            anImage.SetPixels(temp.GetPixels());
            addImageColorToList(anImage);
        }
        int numSlices = (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(numImages) / System.Math.Log(2)));

        //Allocate texture memory.
        Texture3D volumeData = new Texture3D(width, height, numSlices, TextureFormat.ARGB32, false);

        //Pad the color array with empty data to fill in for missing images.
        Texture2D emptyTex = new Texture2D(width, height);
        for(int j = numImages; j < numSlices; j++)
        {
            addImageColorToList(emptyTex);
        }

        //Copy data to texture memory.
        Color[] allColors = imageColors.ToArray();
        volumeData.SetPixels(allColors);
        volumeData.Apply();

        // assign it to the material of the parent object
        GetComponent<Renderer>().material.SetTexture("Cadaver_Data", volumeData);
        // save it as an asset for re-use
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(volumeData, "Assets/Cadaver Assets/Cadaver_" + legendLoader.legendID.ToString() + ".asset");
#endif
    }

    void addImageColorToList(Texture2D anImage)
    {
        Color[] tempColors = anImage.GetPixels();
        for (int i = 0; i < tempColors.Length; i++)
        {
            imageColors.Add(tempColors[i]);
        }
    }

}
