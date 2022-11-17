using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToggleColor : MonoBehaviour
{
    public Color mainColorOn;
    public Color HoverColorOn;
    public Color mainColorOff;
    public Color HoverColorOff;

    [System.NonSerialized]
    public Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    public void ToggleColorChanged(bool isOn)
    {
        //Debug.Log("Is on: " + isOn);
        ColorBlock cb = toggle.colors;
        if (isOn)
        {
            cb.normalColor = mainColorOn;
            cb.highlightedColor = HoverColorOn;
            cb.pressedColor = HoverColorOn;
            cb.selectedColor = mainColorOn;
        }
        else
        {
            cb.normalColor = mainColorOff;
            cb.highlightedColor = HoverColorOff;
            cb.pressedColor = HoverColorOff;
            cb.selectedColor = mainColorOff;
        }
        toggle.colors = cb;
    }
}

