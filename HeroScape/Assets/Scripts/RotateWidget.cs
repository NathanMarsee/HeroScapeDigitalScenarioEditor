using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateWidget : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject rotationGuide;
    private float startAngle = 0;
    private bool dragging = false;
    private float angleDifference = 0;
    private EditorController editor;
    public Material pink;
    public Material pinkGlow;

    /*public void OnBeginDrag(PointerEventData eventData)
    {
        print("dragStart");
        dragging = true;
        startAngle = rotationGuide.transform.rotation.y;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print("dragEnd");
        dragging = false;
    }*/
    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        startAngle = rotationGuide.transform.rotation.eulerAngles.y;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        foreach (Transform child in transform)
        {
            //if (child.GetComponent<Renderer>().material == pinkGlow)
            {
                child.GetComponent<Renderer>().material = pink;
            }
        }
        dragging = false;
    }

    private static float GetAngleDifference(float angle1, float angle2)
    {
        float difference = angle2 - angle1;
        while (difference < -180)
        {
            difference += 360;
        }
        while (difference > 180)
        {
            difference -= 360;
        }
        return difference;
    }

    // Start is called before the first frame update
    void Start()
    {
        editor = FindObjectOfType<EditorController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            angleDifference = GetAngleDifference(startAngle, rotationGuide.transform.rotation.eulerAngles.y);
            if (angleDifference > 60) 
            {
                editor.rotatePiece(true);
                startAngle += 60;
            }
            else if (angleDifference < -60)
            {
                editor.rotatePiece(false);
                startAngle -= 60;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (Transform child in transform)
        {
            //if (child.GetComponent<Renderer>().material == pink)
            {
                child.GetComponent<Renderer>().material = pinkGlow;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!dragging) 
        { 
            foreach (Transform child in transform)
            {
                //if (child.GetComponent<Renderer>().material == pinkGlow)
                {
                    child.GetComponent<Renderer>().material = pink;
                }
            }
        }
    }
}
