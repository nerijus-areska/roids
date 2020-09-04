using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Roid : MonoBehaviour
{

    public float oreAmount;
    public float initialOreAmount;
    public GameObject textComponent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void refreshOreAmount() {
        String[] suffixes = {"K", "M", "B", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "De"};
        String suffix = "";
        int i = 0;
        var tmpAmount = oreAmount;
        while(i < 10) {
            if(tmpAmount > 1000) {
                tmpAmount = tmpAmount / 1000;
                suffix = suffixes[i];
            }
            i++;
        }
        long roundedAmount = (long)Math.Round(tmpAmount, 0);
        
        textComponent.GetComponent<TextMeshProUGUI>().SetText(roundedAmount + suffix);
    }
}
