using System;
using System.Collections.Generic;
using UnityEngine;
using static HexGrid;

public class EditorController : MonoBehaviour
{
    public HexGrid grid;
    public Transform targetHex;
    public Piece selectedPiece;
    public int activelayer = 0;
    public Mouse3D mouse;
    public GameObject transformWidget;
    public Dictionary<Vector3Int, Piece> terrainPieces = new Dictionary<Vector3Int, Piece>();
    Dictionary<Vector3Int, ItemInGrid> hexContents = new Dictionary<Vector3Int, ItemInGrid>();

    public bool pieceSelected = false;
    public bool holdingPiece = false;

    public class ItemInGrid
    {
        public Piece parentPiece;
        public ItemInGrid (Piece piece)
        {
            parentPiece = piece;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (pieceSelected)
        {
            transformWidget.transform.position = selectedPiece.gameObject.transform.position;
        }
        activelayer = (int) Math.Round(mouse.transform.position.y * 5);
        if (grid.lastGridObject != null)
        {
            targetHex = grid.lastGridObject.visualTransform;
        }
        if (holdingPiece && Input.GetMouseButton(0))
        {
            selectedPiece.transform.position = targetHex.position;
            grid.gridHexXZLayers[activelayer].GetXZ(selectedPiece.transform.position, out int posX, out int posZ);
            selectedPiece.gridPosition = new Vector3Int(posX, activelayer, posZ);
            while (CheckForOverlap(selectedPiece))
            {
                activelayer++;
                if (activelayer >= grid.gridHexXZLayers.Count)
                {
                    grid.AddLayer();
                }
                //selectedPiece.transform.position += new Vector3(0, 0.2f, 0);
                selectedPiece.gridPosition.y++;
            }
        }
        else if (holdingPiece && !Input.GetMouseButton(0))
        {
            LetGoOfPiece();
        }
        else if (Input.GetKeyDown(KeyCode.Delete) && pieceSelected)
        {
            DeletePiece();
        }
        else if (Input.GetKeyDown(KeyCode.E) && pieceSelected)
        {
            rotatePiece(false);
            /*LiftPiece(selectedPiece);
            selectedPiece.transform.Rotate(0, -60, 0);
            if (!CheckForOverlap(selectedPiece))
            {
                bool successfulAdd = PlacePiece(selectedPiece);
                if (!successfulAdd)
                {
                    selectedPiece.transform.Rotate(0, 60, 0);
                    PlacePiece(selectedPiece);
                }
            } else 
            {
                selectedPiece.transform.Rotate(0, 60, 0);
                PlacePiece(selectedPiece);
            }*/
        }
        else if (Input.GetKeyDown(KeyCode.R) && pieceSelected)
        {
            rotatePiece(true);
            /*LiftPiece(selectedPiece);
            selectedPiece.transform.Rotate(0, 60, 0);
            if (!CheckForOverlap(selectedPiece))
            {
                bool successfulAdd = PlacePiece(selectedPiece);
                if (!successfulAdd)
                {
                    selectedPiece.transform.Rotate(0, -60, 0);
                    PlacePiece(selectedPiece);
                }
            }
            else
            {
                selectedPiece.transform.Rotate(0, -60, 0);
                PlacePiece(selectedPiece);
            }*/
        }
    }

    public void rotatePiece(bool right)
    {
        int direction = -1;
        if (right)
        {
            direction = 1;
        }
        LiftPiece(selectedPiece);
        selectedPiece.transform.Rotate(0, direction * 60, 0);
        if (!CheckForOverlap(selectedPiece))
        {
            bool successfulAdd = PlacePiece(selectedPiece);
            selectedPiece.rotations += direction;
            if(selectedPiece.rotations > 5)
            {
                selectedPiece.rotations = 0;
            }
            if (selectedPiece.rotations > 5)
            {
                selectedPiece.rotations = 0;
            }
            if (!successfulAdd)
            {
                selectedPiece.transform.Rotate(0, direction * -60, 0);
                PlacePiece(selectedPiece);
            }
        }
        else
        {
            selectedPiece.transform.Rotate(0, direction * -60, 0);
            PlacePiece(selectedPiece);
        }
    }

    public void ClickedOnPiece(Piece piece)
    {
        if (!holdingPiece && selectedPiece == piece)
        {
            holdingPiece = true;
            //selectedPiece = piece;
            LiftPiece(selectedPiece);
            terrainPieces.Remove(new Vector3Int(selectedPiece.gridPosition.x, selectedPiece.gridPosition.y, selectedPiece.gridPosition.z));
        }
        else if ((!pieceSelected) || piece != selectedPiece)
        {
            if (selectedPiece != null) 
            {
                ChangeBaseMaterial(FindChildrenWithTag(selectedPiece.gameObject, "TileBase"), selectedPiece.defaultMat);
            }
            selectedPiece = piece;
            ChangeBaseMaterial(FindChildrenWithTag(selectedPiece.gameObject, "TileBase"), selectedPiece.selectedMat);
            pieceSelected = true;
            transformWidget.gameObject.SetActive(true);
        }
    }

    public void LetGoOfPiece()
    {
        if (!CheckForOverlap(selectedPiece))
        {
            holdingPiece = false;
            PlacePiece(selectedPiece);
            //ChangeBaseMaterial(FindChildrenWithTag(selectedPiece.gameObject, "TileBase"), selectedPiece.highlightMat);
            int xPos = 0;
            int yPos = 0;
            grid.gridHexXZLayers[activelayer].GetXZ(selectedPiece.gameObject.transform.position, out xPos, out yPos);
            selectedPiece.gridPosition = new Vector3Int(xPos, activelayer, yPos);
            //print(new Vector3Int(selectedPiece.gridPosition.x, selectedPiece.gridPosition.y, selectedPiece.gridPosition.z) + selectedPiece.name);
            terrainPieces.Add(new Vector3Int(selectedPiece.gridPosition.x, selectedPiece.gridPosition.y, selectedPiece.gridPosition.z), selectedPiece);
            //selectedPiece = null;
        }
    }

    public void ClickedOffPiece()
    {
        pieceSelected = false;
        ChangeBaseMaterial(FindChildrenWithTag(selectedPiece.gameObject, "TileBase"), selectedPiece.defaultMat);
        selectedPiece = null;
        transformWidget.gameObject.SetActive(false);
    }

    List<GameObject> FindChildrenWithTag(GameObject parent, string tag)
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                children.Add(child.gameObject);
            }
            /*foreach (Transform childer in child.transform)
            {
                if (childer.CompareTag(tag))
                {
                    children.Add(childer.gameObject);
                }
            }*/
        }
        return children;
    }

    void ChangeBaseMaterial(List<GameObject> tileHexes, Material mat)
    {
        foreach (GameObject tile in tileHexes)
        {
            tile.GetComponent<Renderer>().material = mat;
        }
    }

    bool CheckForOverlap(Piece piece)
    {
        foreach (Transform child in piece.transform)
        {
            grid.gridHexXZLayers[activelayer].GetXZ(child.transform.position, out int childX, out int childZ);
            hexContents.TryGetValue(new Vector3Int(childX, piece.gridPosition.y, childZ), out ItemInGrid item);
            if (item != null)
            {
                return true;
            }
        }
        return false;
    }

    bool PlacePiece(Piece piece)
    {
        List<Vector3Int> addedHexes = new List<Vector3Int>();
        bool success = true;
        foreach (Transform child in selectedPiece.transform)
        {
            grid.gridHexXZLayers[activelayer].GetXZ(child.transform.position, out int childX, out int childZ);
            Vector3Int hexPos = new Vector3Int(childX, selectedPiece.gridPosition.y, childZ);
            try
            {
                hexContents.Add(hexPos, new ItemInGrid(selectedPiece));
                addedHexes.Add(hexPos);
                child.gameObject.layer = 3;
            } catch
            {
                print("fail");
                success = false;
                //addedHexes.Remove(hexPos);
                child.gameObject.layer = 0;
                break;
            }
            //print(hexPos + piece.name);
        }
        if (!success)
        {
            foreach (Vector3Int hex in addedHexes)
            {
                hexContents.Remove(hex);
            }
            foreach (Transform child in selectedPiece.transform)
            {
                child.gameObject.layer = 0;
            }
        }
        return success;
    }

    bool LiftPiece(Piece piece)
    {
        foreach (Transform child in selectedPiece.transform)
        {
            child.gameObject.layer = 0;
            grid.gridHexXZLayers[activelayer].GetXZ(child.transform.position, out int childX, out int childZ);
            hexContents.Remove(new Vector3Int(childX, selectedPiece.gridPosition.y, childZ));
        }
        return true;
    }

    public bool DeletePiece()
    {
        pieceSelected = false;
        terrainPieces.Remove(selectedPiece.gridPosition);
        LiftPiece(selectedPiece);
        Destroy(selectedPiece.gameObject);
        selectedPiece = null;
        transformWidget.gameObject.SetActive(false);
        return true;
    }

    public void ClearAll()
    {
        if(holdingPiece)
            DeletePiece();
        pieceSelected = false;
        selectedPiece = null;
        holdingPiece = false;
        foreach (var item in terrainPieces)
        {
            Destroy(item.Value.gameObject);
        }
        terrainPieces.Clear();
        hexContents.Clear();
    }
}
