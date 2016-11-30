using UnityEngine;
using System.Collections;

public class disableCanvas : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        this.GetComponent<Canvas>().enabled = false;
    }
}
