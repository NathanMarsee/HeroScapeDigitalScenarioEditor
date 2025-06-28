using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Piece;

public class ImageScrollList : MonoBehaviour
{
    public GameObject imagePrefab;
    public Transform contentPanel;
    public InputField searchField;

    private List<GameObject> imageObjects = new List<GameObject>();
    private List<Sprite> allImages = new List<Sprite>();

    [SerializeField]
    private Sprite defaultSprite;


    void Start()
    {
        LoadImages();
        searchField.onValueChanged.AddListener(FilterImages);
    }

    void LoadImages()
    {
        
        Sprite[] sprites = Resources.LoadAll<Sprite>("HeroscapePieces");
        allImages = sprites.ToList();

        /*foreach (Sprite sprite in allImages)
        {
            print(sprite.name);
            AddImageItem(sprite);
        }*/
        foreach (TerrainType terrainType in Enum.GetValues(typeof(TerrainType)))
        {
            foreach (PieceSize pieceSize in Enum.GetValues(typeof(PieceSize)))
            {
                if (terrainType != TerrainType.Water || pieceSize == PieceSize.s1) // only do water of size 1
                {
                    bool found = false;
                    foreach (Sprite sprite in allImages)
                    {
                        if (sprite.name.Equals(pieceSize.ToString() + terrainType.ToString()))
                        {
                            /*print(sprite.name.Substring(0, pieceSize.ToString().Length));
                            print(sprite.name);*/
                            AddImageItem(sprite, terrainType, pieceSize);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        AddImageItem(defaultSprite, terrainType, pieceSize);
                    }
                }
            }
        }
    }

    void AddImageItem(Sprite sprite, TerrainType terrainType, PieceSize pieceSize)
    {
        GameObject newItem = Instantiate(imagePrefab, contentPanel);
        newItem.GetComponent<Image>().sprite = sprite;
        newItem.GetComponent<PieceImage>().terrainType = terrainType;
        newItem.GetComponent<PieceImage>().pieceSize = pieceSize;
        imageObjects.Add(newItem);
    }

    void FilterImages(string query)
    {
        query = query.ToLower();

        /*for (int i = 0; i < allImages.Count; i++)
        {
            bool matchesSearch = allImages[i].name.ToLower().Contains(query);
            imageObjects[i].SetActive(matchesSearch);
        }*/
        foreach (GameObject image in imageObjects)
        {
            PieceImage pieceImage = image.GetComponent<PieceImage>();
            bool match = pieceImage.terrainType.ToString().ToLower().Contains(query) || pieceImage.pieceSize.ToString().ToLower().Substring(1).Contains(query) ||
                (pieceImage.terrainType.ToString().ToLower() + pieceImage.pieceSize.ToString().ToLower().Substring(1)).Contains(query) ||
                (pieceImage.pieceSize.ToString().ToLower() + pieceImage.terrainType.ToString().ToLower().Substring(1)).Contains(query);
            image.SetActive(match);
        }
    }
}

