using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DescriptionButtonClick : MonoBehaviour
{
    // Start is called before the first frame update
    public Button descButton;
    public GameObject descTextBox;

    void Start()
    {
        descTextBox.SetActive(false);
    }

    public void ToggleTextBox()
    {
        descTextBox.SetActive(!descTextBox.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {
        if (descTextBox.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject()) // Check if clicking UI
            {
                descTextBox.SetActive(false);
            }
        }
    }
}
