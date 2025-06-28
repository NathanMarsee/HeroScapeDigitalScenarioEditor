using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static Piece;

public class DragAndDropController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool mouseInUI = false;
    public bool dragging = false;
    public PieceImage heldPieceImage = null;
    private EditorController editor = null;

    public List<GameObject> prefabs = new List<GameObject>();

    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseInUI = true;
        if (editor.holdingPiece && dragging)
        {
            editor.holdingPiece = false;
            editor.DeletePiece();
            dragging = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseInUI = false;
        if (dragging)
        {
            foreach (GameObject prefab in prefabs)
            {
                if (prefab.name.Equals(heldPieceImage.pieceSize.ToString() + heldPieceImage.terrainType.ToString()))
                {
                    if(editor.pieceSelected)
                        editor.ClickedOffPiece();
                    GameObject newPiece = Instantiate(prefab);
                    editor.ClickedOnPiece(newPiece.GetComponent<Piece>());
                    editor.ClickedOnPiece(newPiece.GetComponent<Piece>());
                    break;
                }
            }
            //dragging = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        editor = FindObjectOfType<EditorController>();
        GameObject[] tileObjects = Resources.LoadAll<GameObject>("HexTilePrefabs");
        prefabs = tileObjects.ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
