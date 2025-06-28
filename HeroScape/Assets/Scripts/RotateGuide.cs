using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGuide : MonoBehaviour
{
    private EditorController editor;

    // Start is called before the first frame update
    void Start()
    {
        editor = FindObjectOfType<EditorController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(editor.mouse.transform.position);
    }
}
