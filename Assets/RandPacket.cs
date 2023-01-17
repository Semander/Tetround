using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RandPacket : MonoBehaviour
{
    int Ident;
    public TMP_Text ID;
    public int waveAmount = 0;
    public TMP_InputField input;
    public TMP_Text currentWave;

    public long toggleBits = 0;

    public Toggle[] toggles;

    [NonSerialized] public AddPacketToContent parentScript;

    public Wave wavePacket = new Wave();

    public void UpdWaveAmount(string waveAmountString)
    {
        try
        {
            waveAmount = int.Parse(waveAmountString);

            if (waveAmount < 0)
            {
                input.text = "0";
                waveAmount = 0;
            }
        }
        catch (FormatException)
        {
            input.text = "0";
            waveAmount = 0;
        }
        parentScript.UpdCurrWave();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public int UpdCurrentWave(int currWave)
    {
        int lastWave = currWave + waveAmount;
        if (waveAmount > 0)
        {
            currentWave.text = currWave.ToString() + "-" + (lastWave - 1).ToString();
        }
        else { currentWave.text = currWave.ToString(); }

        wavePacket.waveAmount = waveAmount;

        return lastWave;
    }

    public void UpdId(int id)
    {
        Ident = id;
        ID.text = (Ident + 1).ToString();
    }

    public Wave UpdBits()
    {
        toggleBits = 0;

        for (int i = toggles.Length - 1; i >= 0; i--)
        {
            toggleBits <<= 1;
            if (toggles[i].isOn)
            {
                toggleBits++;
            }
        }
        Debug.Log("bits: " + toggleBits);

        wavePacket.shapes = toggleBits;
        return wavePacket;
    }

    public void UpdWave(Wave wave)
    {
        long bits = wave.shapes;
        for (int i = 0; i < toggles.Length; i++)
        {
            if (bits%2 == 1)
            {
                toggles[i].isOn = true;
            }
            bits >>= 1;
        }
        waveAmount = wave.waveAmount;
        input.text = waveAmount.ToString();
    }

    public void Destr()
    {
        parentScript.DeleteRPackage(Ident);
    }

}
