using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SetCadaver : MonoBehaviour {


	// Use this for initialization
	void Start () {

        string fname = constructAssetFilename();
        Texture3D cadaverVolume = Resources.Load("Cadaver Assets/" + fname, typeof(Texture3D)) as Texture3D;
        if (cadaverVolume == null)
            throw new FileNotFoundException();

        //set the shader property.
        GetComponent<Renderer>().material.SetTexture("Cadaver_Data", cadaverVolume);
	}

    string constructAssetFilename()
    {
        SetLegend legendSetter = GetComponent<SetLegend>();
        //Get the IDs from legend setter.
        int systemID = legendSetter.systemID;
        int subsystemID = legendSetter.subsystemID;
        int bodyPartID = legendSetter.bodyPartID;

        return (systemID + "_" + subsystemID + "_" + bodyPartID + "-Asset");
    }
}
