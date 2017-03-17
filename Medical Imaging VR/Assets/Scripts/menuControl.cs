using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menuControl : MonoBehaviour
{
    public OVRInput.Controller controller;
    public GameObject displayPlane;

    //for ray cast
    private LineRenderer laser;
    private bool showUpper;
    int imageIndex;
    public const int imageSize = 8506;

    void Start()
    {
        laser = gameObject.AddComponent<LineRenderer>();
        laser.material = new Material(Shader.Find("Particles/Additive"));
        laser.startColor = Color.yellow;
        laser.endColor = Color.blue;
        laser.startWidth = 0.01f;
        laser.endWidth = 0.01f;
        laser.numPositions = 2;

        showUpper = true;

        imageIndex = 0;
    }

    void Update()
    {
        laser.SetPosition(0, this.transform.position);
       laser.SetPosition(1, this.transform.position + this.transform.forward * 10);

        //selection button is pressed
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, this.transform.forward, out hit))
            {
                laser.SetPosition(1, hit.point);
                GameObject target = hit.collider.gameObject;
                if (target.tag == "button")
                {
                    Button btn = target.GetComponent<Button>();
                    btn.onClick.Invoke();
                }
                
            }
        }

        if (OVRInput.Get(OVRInput.Button.One))
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, this.transform.forward, out hit))
            {
                laser.SetPosition(1, hit.point);
                GameObject target = hit.collider.gameObject;
                if (target.tag == "body")
                {

                    //ranges from 0 to 1
                    float targetPos = 0.5f + target.transform.InverseTransformPoint(hit.point).y;
                    if (showUpper)
                    {
                        imageIndex = 10 * (int)Math.Round((targetPos * imageSize / 2) / 10);
                    }
                    else
                    {
                        imageIndex = 10 * (int)Math.Round((targetPos * imageSize / 2 + imageSize / 2) / 10);
                    }

                    //use imageIndex to display 2d cadaver image
                    Texture2D image = Resources.Load("Cadaver_512/" + imageIndex.ToString().PadLeft(4, '0')) as Texture2D;
                    displayPlane.GetComponent<Renderer>().material.mainTexture = image;

                }
            }
        }

    }
    
                
       
}
