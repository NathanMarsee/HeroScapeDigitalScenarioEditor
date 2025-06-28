using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static QuestPDF.Helpers.Colors;

public class TranslateWidget : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public Material red;
    public Material green;
    public Material blue;
    public Material redGlow;
    public Material greenGlow;
    public Material blueGlow;
    private EditorController editor;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId == -1 && !editor.holdingPiece)
        {
            editor.ClickedOnPiece(editor.selectedPiece);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        foreach (Transform child in transform)
        {
            if (child.name == "red")
            {
                child.GetComponent<Renderer>().material = red;
            }
            if (child.name == "green")
            {
                child.GetComponent<Renderer>().material = green;
            }
            if (child.name == "blue")
            {
                child.GetComponent<Renderer>().material = blue;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (Transform child in transform)
        {
            if (child.name == "red")
            {
                child.GetComponent<Renderer>().material = redGlow;
            }
            if (child.name == "green")
            {
                child.GetComponent<Renderer>().material = greenGlow;
            }
            if (child.name == "blue")
            {
                child.GetComponent<Renderer>().material = blueGlow;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!editor.holdingPiece)
        {
            foreach (Transform child in transform)
            {
                if (child.name == "red")
                {
                    child.GetComponent<Renderer>().material = red;
                }
                if (child.name == "green")
                {
                    child.GetComponent<Renderer>().material = green;
                }
                if (child.name == "blue")
                {
                    child.GetComponent<Renderer>().material = blue;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        editor = FindObjectOfType<EditorController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
