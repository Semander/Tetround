using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandPacket : MonoBehaviour
{
    public TMP_Text ID;
    public int waveAmount = 0;
    public TMP_InputField input;
    public TMP_Text currentWave;

    public long toggleBits = 0;

    public Toggle[] toggle;

    [NonSerialized] public AddPacketToContent parentScript;

    public void UpdWaveAmount(string waveAmountString)
    {
        try
        {
            waveAmount = int.Parse(waveAmountString);
        }
        catch (FormatException)
        {
            input.text = "";
            waveAmount = 0;
        }
        parentScript.UpdCurrWave();
    }

    public int UpdCurrentWave(int currWave)
    {
        int lastWave = currWave + waveAmount;
        if (waveAmount > 0)
        {
            currentWave.text = currWave.ToString() + "-" + (lastWave - 1).ToString();
        }
        else { currentWave.text = currWave.ToString(); }

        return lastWave;
    }

    public void UpdId(int id)
    {
        ID.text = id.ToString();
    }

    public long UpdBits()
    {
        toggleBits = 0;

        for (int i = toggle.Length - 1; i >= 0; i--)
        {
            toggleBits <<= 1;
            if (toggle[i].isOn)
            {
                toggleBits++;
            }
        }
        Debug.Log("bits: " + toggleBits);
        return toggleBits;
    }
}
