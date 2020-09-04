using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Globals.inputDisabled = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Globals.inputDisabled = false;
    }
}
