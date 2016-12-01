using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/**
 * This class sets the legend data tex from a prefab asset file.
 */
public class SetLegend : MonoBehaviour
{

    public DisplayBody bodyDisplayer;
    // Use this for initialization
    void Start()
    {

        string filename = constructAssetFilename();
        Texture3D legendVolume = Resources.Load("Legend Assets/" + filename, typeof(Texture3D)) as Texture3D;

        if (legendVolume == null) //If the asset file can't be found, then throw an exception.
            throw new FileNotFoundException();

        //Set the shader property.
        GetComponent<Renderer>().material.SetTexture("Legend_Data", legendVolume);
    }

    void Update()
    {

    }

    /**
     * Function to determine the file name from the chosen IDs
     */
    string constructAssetFilename()
    {
        GameObject displayBodyComponent = GameObject.Find("SceneControl");
        if (displayBodyComponent == null)
        {
            //TODO Figure out how to get gameobject variable from other scene
            Debug.LogError("failed to find object :(");
            this.enabled = false;
        }
        bodyDisplayer = displayBodyComponent.GetComponent<DisplayBody>();
        Destroy(displayBodyComponent);
        return (bodyDisplayer.DisplayId + "-Asset");
    }
}
