using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualInventoryUI : MonoBehaviour
{
    // Start is called before the first frame update

    public InventoryManager inventoryManager;

    // Pieces
    public Dropdown piecesDropdown;
    public InputField piecesQuantity;
    public Button addPieces;

    // Kits
    public Dropdown kitsDropdown;
    public InputField kitsQuantity;
    public Button addKits;



    void Start()
    {
        addPieces.onClick.AddListener(OnAddPiece);
        addKits.onClick.AddListener(OnAddKit);
    }

    void OnAddPiece()
    {

    }

    void OnAddKit()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
