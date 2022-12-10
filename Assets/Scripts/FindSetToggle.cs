using UnityEngine;
using UnityEngine.UI;

public class FindSetToggle : MonoBehaviour
{
    public Toggle[] toggle;

    [System.NonSerialized]
    private int shapeAmount = 34;
    [System.NonSerialized]
    private bool[] shapes;

    public int symShapes;
    [System.NonSerialized]
    public int asymShapes; 

    private void Awake()
    {
        for (int i = 0; i < shapeAmount; i++)
        {
            shapes[i] = toggle[i].isOn;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
