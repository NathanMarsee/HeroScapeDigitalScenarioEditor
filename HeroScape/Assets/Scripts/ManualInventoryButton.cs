using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualInventoryButton : MonoBehaviour
{
    // Start is called before the first frame update

    public Toggle manual;
    public Button openInventoryButton;
    public GameObject inventory;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (manual.isOn)
        {
            openInventoryButton.gameObject.SetActive(true);
        }
        else
        {
            openInventoryButton.gameObject.SetActive(false);
        }
    }
}
