using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Material highlightMat;
    public Material defaultMat;
    private EditorController editor;

    // Start is called before the first frame update
    void Start()
    {
        editor = FindObjectOfType<EditorController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!editor.holdingPiece)
            GetComponent<Renderer>().material = highlightMat;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!editor.holdingPiece)
            GetComponent<Renderer>().material = defaultMat;
    }

}
