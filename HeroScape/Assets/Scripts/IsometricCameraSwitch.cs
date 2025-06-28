using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCameraSwitch : MonoBehaviour
{
    public Camera isometricCamera;
    public Camera defaultCamera;
    public bool isIsometric;
    public float transitionSpeed = 0.2f;
    private float transitionTime = 0f;

    private Camera target;
    private Camera current;
 

    // Start is called before the first frame update
    void Start()
    {
        defaultCamera.enabled = true;
        isometricCamera.enabled = false;
        current = defaultCamera;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I) && !isIsometric)
        {
            target = (current == defaultCamera) ? isometricCamera : defaultCamera;
            isIsometric = true;
            transitionTime = 0f;
        }

        if(isIsometric)
        {
            float lerpProgress = transitionTime / transitionSpeed;
            current.transform.position = Vector3.Lerp(current.transform.position, target.transform.position, lerpProgress);
            current.transform.rotation = Quaternion.Slerp(current.transform.rotation, target.transform.rotation, lerpProgress);

            transitionTime += Time.deltaTime;

            if (transitionTime >= transitionSpeed)
            {
                isIsometric = false;
                current.enabled = false;  // Disable the current camera
                target.enabled = true;   // Enable the target camera
                current = target;  // Set the new active camera
            }

        }
        
    }
}
