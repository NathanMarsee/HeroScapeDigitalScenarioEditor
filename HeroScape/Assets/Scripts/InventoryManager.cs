using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Kit> kits = new List<Kit>();
    public Text feedback;

    public Toggle manualToggle;
    public bool manualMode;

    public List<string> ownedKitNames = new List<string>();

    public List<string> neededKits = new List<string> { "Basic", "Lands of Valhalla Expansion" };

    public Dictionary<string, int> manualInventory = new Dictionary<string, int>();

    public Dropdown pieceNames;
    public Dropdown kitNameInputs;
    public InputField quantityInputPieces;
    public InputField quantityInputKits;
    public Button addPieceButton;
    public Button addKitButton;

    public Text inventoryDisplay;


    void Start()
    {
        LoadKits("kits");

        SetupUIListeners();

        if (!manualMode)
        {
            StartCoroutine(CheckKitMatch());
        }
        if(manualToggle != null)
        {
            manualToggle.onValueChanged.AddListener(OnManualToggleChanged);
            manualToggle.isOn = manualMode;
        }

        Debug.Log("Add Kit Button: " + (addKitButton != null ? "Assigned ✅" : "Missing ❌"));

    }

    void SetupUIListeners()
    {
        addPieceButton.onClick.AddListener(() =>
        {
            string piece = pieceNames.options[pieceNames.value].text;
            int quantity = int.Parse(quantityInputPieces.text);
            Debug.Log("Add Piece Button Clicked");

            if (manualInventory.ContainsKey(piece))
                manualInventory[piece] += quantity;
            else
                manualInventory[piece] = quantity;

            UpdateInventoryDisplay();

            if (manualMode)
            {
                FindNeededKitsFromManual(neededKits);
            }
        });

        addKitButton.onClick.AddListener(() =>
        {
            string kitName = kitNameInputs.options[kitNameInputs.value].text;
            Kit selectedKit = kits.FirstOrDefault(k => k.kitName == kitName);

            if (selectedKit != null)
            {
                foreach (var kvp in selectedKit.kitPieces)
                {
                    if (manualInventory.ContainsKey(kvp.Key))
                        manualInventory[kvp.Key] += kvp.Value;
                    else
                        manualInventory[kvp.Key] = kvp.Value;
                }
                UpdateInventoryDisplay();

                if (manualMode)
                {
                    FindNeededKitsFromManual(neededKits);
                }
            }
        });
    }

    void UpdateInventoryDisplay()
    {
        inventoryDisplay.text = "Manual Inventory:\n";

        foreach (var kvp in manualInventory)
        {
            inventoryDisplay.text += $"{kvp.Key}: {kvp.Value}\n";
        }

        inventoryDisplay.text += "\nOwned Kits:\n";
        foreach (string kitName in ownedKitNames)
        {
            inventoryDisplay.text += $"{kitName}\n";
        }
    }


    void OnManualToggleChanged(bool isManual)
    {
        manualMode = isManual;
        StopAllCoroutines();

        if(!manualMode)
        {
            StartCoroutine(CheckKitMatch());
            Debug.Log("✅ Switched to Automatic mode.");
        } else
        {
            Debug.Log("Switched to manual mode.");
        }
    }

    IEnumerator CheckKitMatch()
    {
        while (true)
        {
            FindClosestKitMatch();
            yield return new WaitForSeconds(5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(kits.Count > 0 && Input.GetKeyDown(KeyCode.C))
        {
            if(manualMode)
            {
                FindNeededKitsFromManual(neededKits);
            } else
            {
                FindClosestKitMatch();
            }
            
        }
    }

    void LoadKits(string fileName)
    {
        TextAsset file = Resources.Load<TextAsset>(fileName);
        if (file == null)
        {
            Debug.LogError("Kit file not found!");
            return;
        }

        string[] lines = file.text.Split('\n');
        Debug.Log("Kit file successfully loaded.");

        Kit currentKit = null;

        foreach (string line in lines)
        {
            string trimmed = line.Trim();

            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
            {
                continue;
            }
            if (trimmed.StartsWith("Kit:"))
            {
                if (currentKit != null)
                {
                    kits.Add(currentKit);
                    Debug.Log($"Kit loaded: {currentKit.kitName} with {currentKit.kitPieces.Count} pieces.");
                }
                currentKit = new Kit(trimmed.Substring(4).Trim());
            }
            else if (currentKit != null && line.Contains(":"))
            {
                string[] parts = line.Split(':');
                string pieceName = parts[0].Trim();
                int quantity = int.Parse(parts[1].Trim());
                currentKit.AddPiece(pieceName, quantity);
            }
        }
        if (currentKit != null)
        {
            kits.Add(currentKit);
            Debug.Log($"Kit loaded: {currentKit.kitName} | Total Pieces: {currentKit.TotalQuantity()}");
        }
        if (kitNameInputs != null)
        {
            kitNameInputs.ClearOptions();
            kitNameInputs.AddOptions(kits.Select(k => k.kitName).ToList());
        }
        if (pieceNames != null)
        {
            var allPieceNames = kits.SelectMany(k => k.kitPieces.Keys).Distinct().ToList();
            pieceNames.ClearOptions();
            pieceNames.AddOptions(allPieceNames);
        }



        Debug.Log($"Total kits loaded: {kits.Count}");
    }

    void compareSceneToKit(Kit kit)
    {
     
        Debug.Log($"--- Comparing scene to kit: {kit.kitName} ---");

        Dictionary<string, int> sceneCounts = new Dictionary<string, int>();

        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Count the occurrences of pieces in the scene
        foreach (GameObject obj in allObjects)
        {
            if (!obj.scene.IsValid()) continue;

            string objName = obj.name.Replace("(Clone)", "").Trim();

            if (kit.kitPieces.ContainsKey(objName))
            {
                if (sceneCounts.ContainsKey(objName))
                    sceneCounts[objName]++;
                else
                    sceneCounts[objName] = 1;
            }
        }

        int matchedPieces = 0;
        int totalExpected = 0;

        // Compare the scene counts with expected counts in the kit
        foreach (var piece in kit.kitPieces)
        {
            int expected = piece.Value;
            int inScene = sceneCounts.ContainsKey(piece.Key) ? sceneCounts[piece.Key] : 0;

            totalExpected += expected;
            matchedPieces += Mathf.Min(expected, inScene);

            string message = $"Piece: {piece.Key} | Expected: {expected} | In Scene: {inScene}";

            if (inScene == expected)
                Debug.Log($"✔ {message}");
            else
                Debug.LogWarning($"❌ {message}");
        }

        float similarity = (totalExpected > 0) ? (matchedPieces / (float)totalExpected) * 100f : 0f;
        Debug.Log($"🔍 Similarity to kit '{kit.kitName}': {similarity:F1}%");
    }

    void FindClosestKitMatch()
    {
        if (kits.Count == 0) return;

        Dictionary<string, int> sceneCounts = new Dictionary<string, int>();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (!obj.scene.IsValid()) continue;

            //string objName = obj.name.Replace("(Clone)", "").Trim();
            string objName = System.Text.RegularExpressions.Regex.Replace(obj.name, @"\s*\(\d+\)|\(Clone\)", "").Trim();

            if (sceneCounts.ContainsKey(objName))
                sceneCounts[objName]++;
            else
                sceneCounts[objName] = 1;
        }

        Kit bestMatch = null;
        float bestSimilarity = 0f;

        foreach (Kit kit in kits)
        {
            int matchedPieces = 0;
            int totalExpected = 0;

            foreach (var piece in kit.kitPieces)
            {
                int expected = piece.Value;
                int inScene = sceneCounts.ContainsKey(piece.Key) ? sceneCounts[piece.Key] : 0;

                matchedPieces += Mathf.Min(expected, inScene);
                totalExpected += expected;
            }

            float similarity = (totalExpected > 0) ? (matchedPieces / (float)totalExpected) * 100f : 0f;

            Debug.Log($"Kit '{kit.kitName}' similarity: {similarity:F1}%");

            if (similarity > bestSimilarity)
            {
                bestSimilarity = similarity;
                bestMatch = kit;
            }
        }

        if (bestMatch != null)
        {
            Debug.Log($"🏆 Best matching kit: {bestMatch.kitName} ({bestSimilarity:F1}% match)");
            compareSceneToKit(bestMatch); // Optionally show detailed comparison
        }
        else
        {
            Debug.Log("⚠ No matching kits found.");
        }

        if (bestSimilarity < 60f)
        {
            Debug.Log("No suitable kit.");
        } else
        {
            Debug.Log($"✅ Recommended Kit: <b>{bestMatch.kitName}</b> — Match Score: <b>{bestSimilarity:F1}%</b>");
            feedback.text = "";
            feedback.text += $"Recommended Kit: '{bestMatch.kitName}' -- similarity: {bestSimilarity:F1}%\n";
        }
    }

    //void FindBestMatchManual()
    //{
    //    if (kits.Count == 0 || manualInventory.Count == 0)
    //    {
    //        feedback.text = "⚠ No kits or manual inventory is empty.";
    //        return;
    //    }

    //    Kit bestMatch = null;

    //    foreach (Kit kit in kits)
    //    {
    //        bool canBuild = true;
    //        foreach (var piece in kit.kitPieces)
    //        {
    //            int owned = manualInventory.ContainsKey(piece.Key) ? manualInventory[piece.Key] : 0;
    //            if (owned < piece.Value)
    //            {
    //                canBuild = false;
    //                break;
    //            }
    //        }
    //        if (canBuild)
    //        {
    //            bestMatch = kit;
    //            break; // stop at first full match
    //        }
    //    }

    //    if (bestMatch != null)
    //    {
    //        // Remove pieces used from manual inventory
    //        foreach (var piece in bestMatch.kitPieces)
    //        {
    //            manualInventory[piece.Key] -= piece.Value;
    //            if (manualInventory[piece.Key] <= 0)
    //                manualInventory.Remove(piece.Key);
    //        }

    //        // Add kit to owned list if not already there
    //        if (!ownedKitNames.Contains(bestMatch.kitName))
    //        {
    //            ownedKitNames.Add(bestMatch.kitName);
    //        }

    //        feedback.text = $"✅ You can build kit: {bestMatch.kitName} (100% complete).";
    //        Debug.Log(feedback.text);

    //        UpdateInventoryDisplay();
    //    }
    //    else
    //    {
    //        feedback.text = "❌ No complete kit matches found in manual inventory.";
    //        Debug.Log(feedback.text);
    //    }
    //}

    bool CanBuildKit(Kit kit)
    {
        foreach (var piece in kit.kitPieces)
        {
            int owned = manualInventory.ContainsKey(piece.Key) ? manualInventory[piece.Key] : 0;
            if (owned < piece.Value) return false;
        }
        return true;
    }

    void BuildKit(Kit kit)
    {
        foreach (var piece in kit.kitPieces)
        {
            manualInventory[piece.Key] -= piece.Value;
            if (manualInventory[piece.Key] <= 0)
                manualInventory.Remove(piece.Key);
        }

        if (!ownedKitNames.Contains(kit.kitName))
            ownedKitNames.Add(kit.kitName);
    }

    

    void FindNeededKitsFromManual(List<string> neededKitNames)
    {
        if (kits.Count == 0 || manualInventory.Count == 0)
        {
            feedback.text = "⚠ No kits or manual inventory is empty.";
            return;
        }

        // Count how many kits are needed:
        Dictionary<string, int> neededCounts = new Dictionary<string, int>();
        foreach (var kitName in neededKitNames)
        {
            if (neededCounts.ContainsKey(kitName))
                neededCounts[kitName]++;
            else
                neededCounts[kitName] = 1;
        }

        // Count how many kits are already owned:
        Dictionary<string, int> ownedCounts = new Dictionary<string, int>();
        foreach (var kitName in ownedKitNames)
        {
            if (ownedCounts.ContainsKey(kitName))
                ownedCounts[kitName]++;
            else
                ownedCounts[kitName] = 1;
        }

        List<string> builtKits = new List<string>();

        foreach (var kvp in neededCounts)
        {
            string kitName = kvp.Key;
            int neededCount = kvp.Value;
            int ownedCount = ownedCounts.ContainsKey(kitName) ? ownedCounts[kitName] : 0;
            int toBuild = neededCount - ownedCount;

            if (toBuild <= 0) continue;

            Kit kit = kits.Find(k => k.kitName == kitName);
            if (kit == null) continue;

            for (int i = 0; i < toBuild; i++)
            {
                if (CanBuildKit(kit))
                {
                    BuildKit(kit);
                    builtKits.Add(kit.kitName);
                }
                else
                {
                    break; // Can't build more of this kit
                }
            }
        }

        if (builtKits.Count > 0)
        {
            feedback.text = $"✅ Built kits: {string.Join(", ", builtKits)}";
        }
        else
        {
            feedback.text = "❌ Cannot build any needed kits from inventory.";
        }

        UpdateInventoryDisplay();
    }

    public class Kit
    {
        public string kitName;
        public Dictionary<string, int> kitPieces = new Dictionary<string, int>();

        public Kit(string name)
        {
            kitName = name;
            kitPieces = new Dictionary<string, int>();
        }

        public void AddPiece(string pieceName, int quantity)
        {
            if (kitPieces.ContainsKey(pieceName))
            {
                kitPieces[pieceName] += quantity;
            }
            else
            {
                kitPieces[pieceName] = quantity;
            }
        }

        public int TotalQuantity()
        {
            int total = 0;
            foreach (var entry in kitPieces)
            {
                total += entry.Value;
            }
            return total;
        }
    }
}

