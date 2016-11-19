using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SetCadaver : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SetLegend legendSetter = GetComponent<SetLegend>();

        Texture3D cadaverVolume = Resources.Load("Cadaver Assets/Cadaver_" + legendSetter.legendID, typeof(Texture3D)) as Texture3D;
        if (cadaverVolume == null)
            throw new FileNotFoundException();

        //set the shader property.
        GetComponent<Renderer>().material.SetTexture("Cadaver_Data", cadaverVolume);
	}

}
