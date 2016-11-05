#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/**
 * This script is responsible for loading the legend dataset. 
 * It writes an ouput vol tex data file that can then be set in material.
 */
public class LoadLegend : MonoBehaviour
{

    public int legendID = 1;
    public int startIndex = 60;
    public int endIndex = 1010;
    public int indexIncrement = 5;

    private List<Color> imageColors;

    // Use this for initialization
    void Start()
    {
        imageColors = new List<Color>();

        string folderPath = findLegendFolderWithID(legendID);

        //Precalc the tex dimensions by loading the first image.
        Texture2D first = Resources.Load(folderPath + startIndex.ToString().PadLeft(4, '0')) as Texture2D;
        int width = (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(first.width) / System.Math.Log(2)));
        int height = (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(first.height) / System.Math.Log(2)));
        int numImages = (endIndex - startIndex + 1) / indexIncrement;

        for (int i = startIndex; i <= endIndex; i += indexIncrement)
        {
            Texture2D anImage = new Texture2D(width, height);
            Texture2D temp = Resources.Load(folderPath + i.ToString().PadLeft(4, '0')) as Texture2D;
            anImage.SetPixels(temp.GetPixels());
            addImageColorToList(anImage);
        }

        //Convert volTex dimensions to nearest power of two
        int numSlices = (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(numImages) / System.Math.Log(2)));

        //Need to pad the 3D tex along the z axis with empty data due to the round off.
        Texture2D emptyTex = new Texture2D(width, height);
        for(int j = numImages; j < numSlices; j++)
        {
            addImageColorToList(emptyTex);
        }
        
        //Allocate the texture memory.
        Texture3D legendVolume = new Texture3D(width, height, numSlices, TextureFormat.ARGB32, false);

        //Copy the data to the 3D texture.
        Color[] allColors = imageColors.ToArray();
        legendVolume.SetPixels(allColors);
        legendVolume.Apply();

        // assign it to the material of the parent object
        GetComponent<Renderer>().material.SetTexture("Legend_Data", legendVolume);

        //Update the shader slice axes;
        setShaderSliceAxes(numImages, numSlices);

        //As a sanity check, don't actually write out files if running from a compiled binary.
#if UNITY_EDITOR
        //Write out the loaded legend to an asset file for quick loading in the future.
        AssetDatabase.CreateAsset(legendVolume, "Assets/Resources/Legend Assets/Legend_" + legendID.ToString() + ".asset");
#endif
    }

    /*
     * Copy the pixel colors of the given image to the master array.
     */
    void addImageColorToList(Texture2D anImage)
    {
        Color[] tempColors = anImage.GetPixels();
        for (int i = 0; i < tempColors.Length; i++)
        {
            imageColors.Add(tempColors[i]);
        }
    }

    /**
     * Function to find the corresponding legend data folder for the given legend ID
     */
    string findLegendFolderWithID( int id )
    {
        string dataPath = Application.dataPath;
        string[] legendDirs = Directory.GetDirectories(dataPath + "/Resources/Legend/", "(" + id.ToString() + ")*");

        //Assuming unique dir and system ids, get the first.
        string folderPath = legendDirs[0];

        //Extract just the last directory.
        string[] folders = folderPath.Split('/');

        //The last element should be the folder name.
        return "Legend/" + folders[folders.Length - 1] + "/";
    }

    void setShaderSliceAxes(float numImages, float numSlices)
    {
        //get the material
        Material mat = GetComponent<Renderer>().material;

        //Set 2nd slice axis to numImages cutoff, as don't want to sample empty voxels.
        mat.SetFloat("_SliceAxis2Max",  numImages / numSlices);
        mat.SetFloat("_SliceAxis2Min", 0.0001f);
    }
}
