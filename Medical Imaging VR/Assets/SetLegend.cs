using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/**
 * This class sets the legend data tex from a prefab asset file.
 */
public class SetLegend : MonoBehaviour {

    public int legendID;

	// Use this for initialization
	void Start () {
        Texture3D legendVolume = Resources.Load("Legend Assets/Legend_" + legendID, typeof(Texture3D)) as Texture3D;

        if (legendVolume == null) //If the asset file can't be found, then throw an exception.
            throw new FileNotFoundException();

        //Set the shader property.
        GetComponent<Renderer>().material.SetTexture("Legend_Data", legendVolume);
	}
}
