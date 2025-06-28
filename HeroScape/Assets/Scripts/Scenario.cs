using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Scenario : MonoBehaviour
{
    // Start is called before the first frame update
    public string Title;
    public string generalDescription;
    public string goal;
    public string setup;
    public string victory;
    public string specialRules;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Scenario (string title)
    {
        Title = title;
        generalDescription = "";
        goal = "";
        setup = "";
        victory = "";
        specialRules = "";
    }

}
