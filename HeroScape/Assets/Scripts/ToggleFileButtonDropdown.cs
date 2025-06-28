using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleFileButtonDropdown : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject fileDropdown;

    void Start()
    {
        fileDropdown.SetActive(false);
    }

    public void ToggleMenu()
    {
        // Toggle the menu on/off
        fileDropdown.SetActive(!fileDropdown.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {
        if (fileDropdown.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject()) // Check if clicking UI
            {
                fileDropdown.SetActive(false);
            }
        }
    }
}
