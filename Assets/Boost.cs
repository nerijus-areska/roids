using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boost : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public float diesAt;
    public bool hasMouseFocus = false;
    public Spaceship spaceshipScript;

    // Update is called once per frame
    void Update()
    {
        if(Time.time > diesAt) {
            if(hasMouseFocus) {
                Globals.inputDisabled = false;
            }
            Destroy(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Debug.Log("I was ENTERED");
        Globals.inputDisabled = true;
        hasMouseFocus = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Debug.Log("I was EXITED");
        Globals.inputDisabled = false;
        hasMouseFocus = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        //Debug.Log("I was CLICKED ON");
        //TODO: Randomize boost times
        
        spaceshipScript.speedBoostUntil = Time.time + 20;
        Destroy(gameObject);
        Globals.inputDisabled = false;
    }
}
