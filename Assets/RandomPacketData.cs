using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomPacketData : MonoBehaviour
{
    public int ID;
    public long numberFromBits; // all shapes
    public int shapesAmount;
    public int waveAmount;
    public int firstWave;

    public string shapesText;
    public string wavesText;
    public string currentText;

    public Text ShapesText;
    public Text WavesInput;
    public Text CurrentText;

    void Start()
    {
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;

        ShapesText.text = shapesText;
        WavesInput.text = wavesText;
    }

}
