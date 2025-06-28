using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotationSpeed = 250f;
    public GameObject gridObject;
    public float zoomSpeed = 10f;
    public float distance = 10f;
    public float maxZoom = 25f;
    public float minZoom = 5f;

    private Vector3 offset;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Quaternion targetRotation;

    public float panSpeed = 10f;

    public GraphicRaycaster uiRaycaster;
    public EventSystem eventSystem;

    public RectTransform scrollViewport;
    public RectTransform descMenuViewport;

    public InputField searchBar;

    private bool isIsometric = false;

    void Start()
    {

        if(gridObject != null)
        {
            offset = transform.position - gridObject.transform.position;
            distance = offset.magnitude;
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }

    }

    // Update is called once per frame
    void Update()
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        if(selectedObj != null)
        {
            if(selectedObj.GetComponent<UnityEngine.UI.InputField>() != null)
            {
                return;
            }
        }

        Rotation();
        zooming();
        panChangeLocation();
        viewSnapping();
    }

    void Rotation()
    {
        if (Input.GetMouseButton(1))
        {
            float horizontalInput = Input.GetAxis("Mouse X");
            float verticalInput = -1 * Input.GetAxis("Mouse Y");

            transform.RotateAround(gridObject.transform.position, Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
            transform.RotateAround(gridObject.transform.position, transform.right, verticalInput * rotationSpeed * Time.deltaTime);

            offset = transform.position - gridObject.transform.position;
        }
    }

    void zooming()
    {
        if (descMenuViewport != null || scrollViewport != null)
        {
            Vector2 mousePos = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(scrollViewport, mousePos))
            {
                return;
            } else if(RectTransformUtility.RectangleContainsScreenPoint(descMenuViewport, mousePos) && descMenuViewport.gameObject.activeSelf)
            {
                return;
            }
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if(scrollInput != 0)
        {
            distance -= scrollInput * zoomSpeed;
            distance = Mathf.Clamp(distance, minZoom, maxZoom);

            transform.position = gridObject.transform.position + offset.normalized * distance;

            offset = transform.position - gridObject.transform.position;
        }
        
    }

    void panChangeLocation()
    {
        // Old panning system (single axis)
        //if (Input.GetMouseButton(2))
        //{
        //    float verticalInput = Input.GetAxis("Mouse Y");
        //    float horizontalInput = Input.GetAxis("Mouse X");
        //    if (Input.GetAxis("Mouse X")<0)
        //    {
        //        Vector3 newPos = transform.position - Vector3.left * horizontalInput * Time.deltaTime * panSpeed;
        //        transform.position = newPos;
        //        print("mouse moved left");
        //    } else if(Input.GetAxis("Mouse X")>0)
        //    {
        //        Vector3 newPos = transform.position + Vector3.right * horizontalInput * Time.deltaTime * panSpeed;
        //        transform.position = newPos;
        //        print("mouse moved right");
        //    } else if(Input.GetAxis("Mouse Y") < 0)
        //    {
        //        Vector3 newPos = transform.position - Vector3.down * verticalInput * Time.deltaTime * panSpeed;
        //        transform.position = newPos;
        //        print("mouse moved down");
        //    } else if(Input.GetAxis("Mouse Y") > 0)
        //    {
        //        //transform.Translate(0, panSpeed * Time.deltaTime, 0);
        //        Vector3 newPos = transform.position + Vector3.up * verticalInput * Time.deltaTime * panSpeed;
        //        transform.position = newPos;
        //        print("mouse moved up");
        //    }
            
        //}
        if(Input.GetMouseButton(2))
        {
            float horizontalInput = Input.GetAxis("Mouse X");
            float verticalInput = Input.GetAxis("Mouse Y");

            Vector3 right = transform.right;
            Vector3 up = transform.up;

            Vector3 move = (-right * horizontalInput + -up * verticalInput) * panSpeed * Time.deltaTime;
            transform.position += move;
            gridObject.transform.position += move;

            offset = transform.position - gridObject.transform.position;
        }
    }

    void isometricView()
    {
        isIsometric = !isIsometric;

        StopAllCoroutines();

        Camera cam = GetComponent<Camera>();

        if(isIsometric)
        {

            if(cam != null)
            {
                cam.orthographic = true;
            }

            Vector3 isoOffset = new Vector3(1,1,-1).normalized * distance;
            Vector3 targetPos = gridObject.transform.position + isoOffset;
            Quaternion targetRotation = Quaternion.LookRotation(gridObject.transform.position - targetPos);
            StartCoroutine(SmoothMoveAndRotate(targetPos, targetRotation));
        } else
        {
            if(cam != null)
            {
                cam.orthographic = false;
            }
            StartCoroutine(SmoothMoveAndRotate(initialPosition, initialRotation));
        }
    }

    void snapToAngle(Vector3 direction)
    {
        Vector3 targetPosition = gridObject.transform.position + direction.normalized * distance;
        Quaternion targetRotation = Quaternion.LookRotation(gridObject.transform.position - targetPosition);

        StopAllCoroutines();
        StartCoroutine(SmoothMoveAndRotate(targetPosition, targetRotation));
    }

    IEnumerator SmoothMoveAndRotate(Vector3 targetPosition, Quaternion targetRotation)
    {
        float duration = 1f;
        float elapsed = 0f;

        Vector3 startingPos = transform.position;
        Quaternion startingRot = transform.rotation;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);

            transform.position = Vector3.Lerp(startingPos, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startingRot, targetRotation, t);

            yield return null;
        }
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    void resetCamera()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothMoveAndRotate(initialPosition, initialRotation));
    }

    void viewSnapping()
    {
            if (Input.GetKey("1"))
            {
                snapToAngle(Vector3.forward);
            }
            else if (Input.GetKey("2"))
            {
                snapToAngle(Vector3.back);
            }
            else if (Input.GetKey("3"))
            {
                snapToAngle(Vector3.left);
            }
            else if (Input.GetKey("4"))
            {
                snapToAngle(Vector3.right);
            }
            else if (Input.GetKey("5"))
            {
                snapToAngle(Vector3.up);
            }
            else if (Input.GetKey("6"))
            {
                resetCamera();
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                isometricView();
            }
        
    }
}
