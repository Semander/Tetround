using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAllShapes : MonoBehaviour
{
    [SerializeField]
    public GameObject allShapes;
    // Start is called before the first frame update
    public void Show()
    {
        allShapes.SetActive(true);
    }

    // Update is called once per frame
    public void Close()
    {
        allShapes.SetActive(false);
    }
}
