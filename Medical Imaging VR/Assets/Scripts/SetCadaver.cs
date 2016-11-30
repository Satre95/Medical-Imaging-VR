using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SetCadaver : MonoBehaviour {


    public DisplayBody bodyDisplayer;
    // Use this for initialization
    void Start()
    {

        string fname = constructAssetFilename();
        Texture3D cadaverVolume = Resources.Load("Cadaver Assets/" + fname, typeof(Texture3D)) as Texture3D;
        if (cadaverVolume == null)
            throw new FileNotFoundException();

        //set the shader property.
        GetComponent<Renderer>().material.SetTexture("Cadaver_Data", cadaverVolume);
    }

    string constructAssetFilename()
    {
        GameObject displayBodyComponent = GameObject.Find("SceneControl");
        if (displayBodyComponent == null)
        {
            //TODO Figure out how to get gameobject variable from other scene
            //Debug.LogError("failed to find object :(");
            this.enabled = false;
        }
        bodyDisplayer = displayBodyComponent.GetComponent<DisplayBody>();
        return (bodyDisplayer.DisplayId + "-Asset");
    }
}
