using UnityEngine;
using System.Collections;

public class Control : MonoBehaviour
{

    // rotate
    public float rotSpeed = 4.0f;
    bool isRotating;
    Vector3 rotationAxisX;
    Vector3 rotationAxisY;
    Vector3 mouseOrigin;
    Vector3 angleDelta;
    GameObject rotationCentre;

    // translate
    public float panSpeed = 2.0f;
    bool isMovingLeft, isMovingRight;
    bool isMovingUp, isMovingDown;
    bool isMovingForward, isMovingBackward;
    Vector3 translationAxis;

    // scale
    public float zoomSpeed = 1.0f;
    private float scaleMin = 0.01f;
    private float scaleMax = 100f;
    bool isScaling;
    float scale; // same for the 3 axes

    //threshold control
    public Renderer rend;
    float threshold = 0.21f;
    bool threshold_up;
    bool threshold_down;

    void Awake()
    {
    }

    void Start()
    {
        scale = Mathf.Clamp(this.transform.localScale[0], scaleMin, scaleMax);
        rend = GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Custom/Cadaver");
    }

    void Update()
    {

        // rotate 

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            isRotating = true;
            if (Input.GetMouseButtonDown(0)) rotationCentre = this.gameObject;
            if (Input.GetMouseButtonDown(1)) rotationCentre = Camera.main.gameObject;
            mouseOrigin = Input.mousePosition;
        }

        if (isRotating)
        {
            rotationAxisX = Camera.main.transform.up;
            rotationAxisY = Camera.main.transform.right;
            angleDelta = (Input.mousePosition - mouseOrigin) / Screen.width;
            angleDelta *= rotSpeed;
            angleDelta.x *= -1;
            this.transform.RotateAround(rotationCentre.transform.position, rotationAxisX, angleDelta.x);
            this.transform.RotateAround(rotationCentre.transform.position, rotationAxisY, angleDelta.y);
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) isRotating = false;
        }

        // translate

        isMovingRight = Input.GetKey(KeyCode.RightArrow);
        isMovingLeft = Input.GetKey(KeyCode.LeftArrow);
        isMovingForward = Input.GetKey(KeyCode.UpArrow);
        isMovingBackward = Input.GetKey(KeyCode.DownArrow);

        if (isMovingRight || isMovingLeft)
        {
            translationAxis = Camera.main.transform.right;
            float distance = panSpeed * Time.deltaTime;
            if (isMovingRight) this.transform.position += translationAxis * distance;
            if (isMovingLeft) this.transform.position -= translationAxis * distance;
        }

        if (isMovingForward || isMovingBackward)
        {
            translationAxis = Camera.main.transform.forward;
            float distance = panSpeed * Time.deltaTime;
            if (isMovingForward) this.transform.position += translationAxis * distance;
            if (isMovingBackward) this.transform.position -= translationAxis * distance;
        }

        // scale

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        isScaling = scroll != 0;

        if (isScaling)
        {
            scale *= 1 + scroll * zoomSpeed;
            scale = Mathf.Clamp(scale, scaleMin, scaleMax);
            this.transform.localScale = new Vector3(scale, scale, scale);
            if (scroll == 0) isScaling = false;
        }

        //threshold control
        threshold_up = Input.GetKey(KeyCode.KeypadPlus);
        threshold_down = Input.GetKey(KeyCode.KeypadMinus);

        if (threshold_up)
        {
            threshold += 0.01f;
            if (threshold > 1.0f)
            {
                threshold = 1.0f;
            }
        }
        if (threshold_down)
        {
            threshold -= 0.01f;
            if (threshold < 0.0f)
            {
                threshold = 0.0f;
            }
        }
        rend.material.SetFloat("_DataMin", threshold);

    }
}
