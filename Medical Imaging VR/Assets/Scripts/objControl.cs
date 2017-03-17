using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objControl : MonoBehaviour {
    public OVRCameraRig camera;
    public OVRInput.Controller controller;
    public GameObject rightHand;
    public GameObject leftHand;
    public float degreeThreshold = 10.0f;
    float offsetIn;
    float offsetOut;
    float scaleOffset;
    float handDistPrev;
    float handDistCurr;
    Vector3 rtHandPosPrev;
    Vector3 rtHandPosCurr;
    Vector3 ltHandPosPrev;
    Vector3 ltHandPosCurr;
    Vector3 ltPrevUp;
    Vector3 rtPrevUp;
    Vector3 ltCurrUp;
    Vector3 rtCurrUp;

    private LineRenderer rotateBar;

    // Use this for initialization
    void Start () {
        offsetIn = 0.0f;
        offsetOut = 0.0f;
        scaleOffset = 0.1f;
        handDistPrev = 0.0f;
        handDistCurr = 0.0f;

        rotateBar = gameObject.AddComponent<LineRenderer>();
        rotateBar.material = new Material(Shader.Find("Particles/Additive"));
        rotateBar.startColor = Color.yellow;
        rotateBar.endColor = Color.blue;
        rotateBar.startWidth = 0.01f;
        rotateBar.endWidth = 0.01f;
        rotateBar.numPositions = 2;
    }
	
	// Update is called once per frame
	void Update () {
        //zoom in
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, controller) > 0.9 )
        {
            offsetIn += 0.01f;
            Vector3 objDirCam = Vector3.Normalize(gameObject.transform.position - camera.transform.position);
            camera.transform.Translate(objDirCam.x * offsetIn, objDirCam.y * offsetIn, objDirCam.z * offsetIn);
        }
        //zoom out
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, controller) > 0.9 && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller) < 0.1)
        {
            offsetOut += 0.01f;
            Vector3 objDirCam = Vector3.Normalize(gameObject.transform.position - camera.transform.position);
            camera.transform.Translate(-objDirCam.x * offsetOut, -objDirCam.y * offsetOut, -objDirCam.z * offsetOut);
        }
        //scaling shiz
        Vector3 rightHandPos = camera.transform.InverseTransformPoint(rightHand.transform.position);
        Vector3 leftHandPos = camera.transform.InverseTransformPoint(leftHand.transform.position);
        handDistCurr = Vector3.Distance(rightHandPos, leftHandPos);
        //scale x
        if (OVRInput.Get(OVRInput.Button.One, controller)&& OVRInput.Get(OVRInput.Button.Three, controller))
        {
            //scale down
            if (handDistCurr - handDistPrev < 0)
            {
                gameObject.transform.localScale -= new Vector3(0.1f, 0.0f, 0.0f);
            }
            else if(handDistCurr-handDistPrev >0)
            {
                gameObject.transform.localScale += new Vector3(0.1f, 0.0f, 0.0f);
            }
        }

        //scale y
        if (OVRInput.Get(OVRInput.Button.Two, controller) && OVRInput.Get(OVRInput.Button.Four, controller))
        {
            //scale down
            if (handDistCurr - handDistPrev < 0)
            {
                gameObject.transform.localScale -= new Vector3(0.0f, 0.1f, 0.0f);
            }
            else if (handDistCurr - handDistPrev > 0)
            {
                gameObject.transform.localScale += new Vector3(0.0f, 0.1f, 0.0f);
            }
        }

        //scale z
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick) && OVRInput.Get(OVRInput.Button.SecondaryThumbstick, controller))
        {
            //scale down
            if (handDistCurr - handDistPrev < 0)
            {
                gameObject.transform.localScale -= new Vector3(0.0f, 0.0f, 0.1f);
            }
            else if (handDistCurr - handDistPrev > 0)
            {
                gameObject.transform.localScale += new Vector3(0.0f, 0.0f, 0.1f);
            }
        }

        handDistPrev = handDistCurr;

        //rotate
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller) > 0.9 && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, controller) > 0.9)
        {
            DrawRotationBars();
            RotateWithHands();
        } else
        {
            Vector3[] zeros = {new Vector3(0, 0, 0), new Vector3(0,0,0) };
            rotateBar.SetPositions(zeros);
        }
        rtHandPosPrev = rtHandPosCurr;
        ltHandPosPrev = ltHandPosCurr;
        ltPrevUp = ltCurrUp;
        rtPrevUp = rtCurrUp;

    }

    void RotateWithHands()
    {
        rtHandPosCurr = rightHand.transform.position;
        ltHandPosCurr = leftHand.transform.position;

        Vector3 left2Right = rtHandPosCurr - ltHandPosCurr;
        Vector3 left2RightPrev = rtHandPosPrev - ltHandPosPrev;
        if (Vector3.Magnitude(left2Right) < 0.1f)
            return;

        //Calculate the project velocity of the right hand
        rtCurrUp = rightHand.transform.up;
        Vector3 rightVel = (rtHandPosCurr - rtHandPosPrev) / Time.deltaTime;
        Vector3 prevRightNorm = Vector3.Normalize(Vector3.Cross(rtPrevUp, left2RightPrev));
        float distFromPrevPlaneR = Vector3.Dot((rtHandPosCurr - rtHandPosPrev), prevRightNorm);
        Vector3 rightNorm;
        if(distFromPrevPlaneR < 0)
            rightNorm = Vector3.Normalize( Vector3.Cross( rightHand.transform.up, left2Right));
        else
            rightNorm = Vector3.Normalize(Vector3.Cross( left2Right, rightHand.transform.up));

        Vector3 projectedRightVel = Vector3.Dot(rightVel, rightNorm) * rightNorm;

        //Calculate the projected velocity of the left hand
        ltCurrUp = leftHand.transform.up;
        Vector3 leftVel = (ltHandPosCurr - ltHandPosPrev) / Time.deltaTime;
        Vector3 prevLeftNorm = Vector3.Normalize(Vector3.Cross(ltPrevUp, left2RightPrev));
        float distFromPrevPlaneL = Vector3.Dot((ltHandPosCurr - ltHandPosPrev), prevLeftNorm);
        Vector3 leftNorm;
        if(distFromPrevPlaneL < 0)
            leftNorm = Vector3.Normalize(Vector3.Cross(leftHand.transform.up, left2Right));
        else
            leftNorm = Vector3.Normalize(Vector3.Cross(left2Right, leftHand.transform.up));
        Vector3 projectedLeftVel = Vector3.Dot(leftVel, leftNorm) * leftNorm;

        //Calculate the rotaion amount
        float delta = (Vector3.Magnitude(projectedRightVel) + Vector3.Magnitude(projectedLeftVel)) / 2.0f;
        float theta = -0.5f * Mathf.Rad2Deg * Mathf.Asin(2.0f * delta / Vector3.Magnitude(left2Right));

        //Determine the rotation axis.
        Vector3 axis = Vector3.Normalize(Vector3.Cross(left2Right, left2RightPrev));
        print("Theta: " + theta);
        print("Axis Mag: " + Vector3.Magnitude(Vector3.Cross(left2Right, left2RightPrev)));
        print("Left to Right Mag: " + Vector3.Magnitude(left2Right));
        if(Mathf.Abs(theta) > degreeThreshold)
            gameObject.transform.Rotate(axis, theta);

    }

    void DrawRotationBars()
    {
        rotateBar.SetPosition(0, leftHand.transform.position);
        rotateBar.SetPosition(1, rightHand.transform.position);
    }
}
