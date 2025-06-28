using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{


    // Start is called before the first frame update
    void Start()
    {
        editor = FindObjectOfType<EditorController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    [Serializable]
    public enum TerrainType { Grass, Sand, Stone, Water };
    [Serializable]
    public enum PieceSize { s1, s2, s3, s7, s24 };

    private EditorController editor;
    public TerrainType terrainType;
    public PieceSize size;
    public Material defaultMat;
    public Material highlightMat;
    public Material selectedMat;
    public Vector3Int gridPosition = new Vector3Int(0, 0, 0);
    public int rotations;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerId == -1 && !editor.holdingPiece)
        {
            editor.ClickedOnPiece(this);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        /*if (eventData.pointerId == -1 && editor.selectedPiece == this && !editor.holdingPiece)
        {
            editor.ClickedOnPiece(this);
        }*/
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!editor.holdingPiece && editor.selectedPiece != this)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("TileBase"))
                {
                    child.GetComponent<Renderer>().material = highlightMat;
                }
            }
            
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (editor.selectedPiece != this)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("TileBase"))
                {
                    child.GetComponent<Renderer>().material = defaultMat;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
