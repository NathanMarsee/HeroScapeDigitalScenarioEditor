using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PieceImage : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Piece.PieceSize pieceSize;
    public Piece.TerrainType terrainType;
    private DragAndDropController dragAndDropController;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!dragAndDropController.dragging)
        {
            dragAndDropController.dragging = true;
            dragAndDropController.heldPieceImage = this;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragAndDropController.dragging)
        {
            dragAndDropController.dragging = false;
            dragAndDropController.heldPieceImage = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        /*if (dragAndDropController.dragging)
        {
            dragAndDropController.dragging = false;
            dragAndDropController.heldPieceImage = null;
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        dragAndDropController = FindObjectOfType<DragAndDropController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
