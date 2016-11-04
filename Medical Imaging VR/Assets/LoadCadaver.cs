﻿#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LoadCadaver : MonoBehaviour
{

    //public Texture3D volumeData;
    public List<Color> imageColors;
    // Use this for initialization
    void Start()
    {

        int numImages = 0;
        int width = 0;
        int height = 0;

        //need to correspond with legend's z-axis
        for (int i = LoadLegend.startNumber; i <= LoadLegend.finalLegendIndex; i+=5)
        {
            Texture2D anImage = Resources.Load("Cadaver/"+i.ToString().PadLeft(4, '0')) as Texture2D;

            width = anImage.width;
            height = anImage.height;
            addImageColorToList(anImage);
            numImages++;
        }

        Color[] allColors = imageColors.ToArray();

        numImages = (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(numImages) / System.Math.Log(2)));
        width = (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(width) / System.Math.Log(2)));
        height = (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log(height) / System.Math.Log(2)));
        int texSize = width * height * numImages;

        Color[] allColorsWithPadding = new Color[texSize];

        allColors.CopyTo(allColorsWithPadding, 0);
        for (int i = allColors.Length; i < allColorsWithPadding.Length; i++)
        {
            allColorsWithPadding[i] = new Color(0, 0, 0, 0);
        }

        Texture3D volumeData = new Texture3D(width, height, numImages, TextureFormat.ARGB32, false);

        volumeData.SetPixels(allColorsWithPadding);
        volumeData.Apply();

        // assign it to the material of the parent object
        GetComponent<Renderer>().material.SetTexture("Cadaver_Data", volumeData);
        // save it as an asset for re-use
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(volumeData, @"Assets/" + "Cadaver_400" + ".asset");
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