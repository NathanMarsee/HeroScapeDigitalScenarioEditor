using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using System.Runtime.InteropServices;
using static Piece;
using System;

public class SaveLoadManager : MonoBehaviour
{
    [Serializable]
    private class Scenario
    {
        public string name;
        public string description;
        public string goal;
        public string setup;
        public string victory;
        public string specialRules;
        public List<KeyValuePair<Vector3Int, SimplePiece>> terrainPieces;
        //Dictionary<Vector3Int, SimplePiece> terrainPieces = new Dictionary<Vector3Int, SimplePiece>();

        public Scenario() 
        {
            name = "";
            description = "";
            goal = "";
            setup = "";
            victory = "";
            specialRules = "";
            terrainPieces = new List<KeyValuePair<Vector3Int, SimplePiece>>();
        }
    }
    [Serializable]
    private class SimplePiece
    {
        public TerrainType terrainType;
        public PieceSize pieceSize;
        public int rotations;

        public SimplePiece()
        {
            rotations = 0;
        }
    }


    public EditorController editor;
    public InputField nameText;
    public InputField descriptionText;
    public InputField goalText;
    public InputField setupText;
    public InputField victoryText;
    public InputField specialRulesText;
    public List<GameObject> prefabs = new List<GameObject>();
    public string fileName;
    public string fileContent;


    // Start is called before the first frame update
    void Start()
    {
        GameObject[] tileObjects = Resources.LoadAll<GameObject>("HexTilePrefabs");
        prefabs = tileObjects.ToList();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NewScenario()
    {
        nameText.text = "";
        descriptionText.text = "";
        goalText.text = "";
        setupText.text = "";
        victoryText.text = "";
        specialRulesText.text = "";
        editor.ClearAll();
    }

    public void BuildJson()
    {
        Scenario scenario = new Scenario
        {
            name = nameText.text,
            description = descriptionText.text,
            goal = goalText.text,
            setup = setupText.text,
            victory = victoryText.text,
            specialRules = specialRulesText.text,
            terrainPieces = new List<KeyValuePair<Vector3Int, SimplePiece>>()
    };
        //scenario.terrainPieces = editor.terrainPieces;
        foreach (var item in editor.terrainPieces)
        {
            //scenario.terrainPieces.ElementAt<Vector3Int>(item.Key)
            SimplePiece piece = new SimplePiece();
            piece.terrainType = item.Value.terrainType;
            piece.pieceSize = item.Value.size;
            piece.rotations = item.Value.rotations;
            scenario.terrainPieces.Add(new KeyValuePair<Vector3Int, SimplePiece>(item.Key, piece));
        }

        StringBuilder sb = new StringBuilder();
        foreach (char c in nameText.text)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == ' '  || c == '_')
            {
                sb.Append(c);
            }
        }
        fileName = sb.ToString();
        //string path = Application.dataPath + "/" + fileName + ".json";

        fileContent = JsonConvert.SerializeObject(scenario, Newtonsoft.Json.Formatting.Indented);
        //File.WriteAllText(path, content);
        //DownloadJsonToFile(fileName, content);
    }

    public void LoadFromJson()
    {
        editor.ClearAll();
        //string fileName;
        /*StringBuilder sb = new StringBuilder();
        foreach (char c in nameText.text)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == ' ' || c == '_')
            {
                sb.Append(c);
            }
        }*/
        //fileName = sb.ToString();
        //string path = Application.dataPath + "/" + fileName + ".json";
        //string content = File.ReadAllText(path);
        //UploadFileToJson();
        Scenario scenario = JsonConvert.DeserializeObject<Scenario>(fileContent);
        nameText.text = scenario.name;
        descriptionText.text = scenario.description;
        goalText.text = scenario.goal;
        setupText.text = scenario.setup;
        victoryText.text = scenario.victory;
        specialRulesText.text = scenario.specialRules;
        StringBuilder sb = new StringBuilder();
        foreach (char c in nameText.text)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == ' ' || c == '_')
            {
                sb.Append(c);
            }
        }
        fileName = sb.ToString();

        foreach (var item in scenario.terrainPieces)
        {
            foreach (GameObject prefab in prefabs)
            {
                if (prefab.name.Equals(item.Value.pieceSize.ToString() + item.Value.terrainType.ToString()))
                {
                    GameObject newPiece = Instantiate(prefab);
                    editor.activelayer = item.Key.y;
                    while (editor.grid.gridHexXZLayers.Count <= item.Key.y)
                    {
                        editor.grid.AddLayer();
                    }
                    print(item.Key.y / 5f);
                    newPiece.transform.position = editor.grid.gridHexXZLayers[item.Key.y].GetWorldPosition(item.Key.x, item.Key.z) + new Vector3(0, item.Key.y / 5f, 0);
                    //item.Key;
                    newPiece.GetComponent<Piece>().rotations = item.Value.rotations;
                    newPiece.transform.eulerAngles = new Vector3(0, 60 * item.Value.rotations, 0);
                    newPiece.GetComponent<Piece>().gridPosition = item.Key;
                    editor.ClickedOnPiece(newPiece.GetComponent<Piece>());
                    editor.ClickedOnPiece(newPiece.GetComponent<Piece>());
                    editor.LetGoOfPiece();
                    break;
                }
            }
        }
    }
}