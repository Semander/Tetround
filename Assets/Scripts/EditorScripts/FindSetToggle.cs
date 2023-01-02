using UnityEngine;
using UnityEngine.UI;

public class FindSetToggle : MonoBehaviour
{
    public Toggle[] toggle;

    [System.NonSerialized]
    private int shapeAmount; //= 34;
    [System.NonSerialized]
    private bool[] shapes;

    public int symShapes;
    [System.NonSerialized]
    public int asymShapes; 

    private void Awake()
    {
        for (int i = 0; i < toggle.Length; i++)
        {
            shapes[i] = toggle[i].isOn;
        }
    }

}
