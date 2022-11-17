using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShapeAmount : MonoBehaviour
{
    [System.NonSerialized]
    public int number = 0;

    [System.NonSerialized]
    public TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void ToggleToNumber(bool isOn)
    {
        if (isOn)
        {
            number++;
        }
        else
        {
            number--;
        }
        text.text = number.ToString();
    }
}
