using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OfficLevel : MonoBehaviour
{
    string SaveName;
    bool isCompleted;

    public int id;
    public TMP_Text idText;
    public TMP_Text nameText;
    public Image Completion;

    [NonSerialized] public HandleOfficLvls parentScript;

    private void Awake()
    {
        Completion.enabled = false;
    }

    public void UpdId(int ident)
    {
        id = ident;
        idText.text = (id + 1).ToString();
    }

    public void setLevelData(Official u)
    {
        SaveName = u.name;
        nameText.text = SaveName;
        isCompleted = u.isCompleted;

        Completion.enabled = isCompleted;
    }

    public void Play() // Switch to play
    {
        FileNameController.filePath = SaveName;
        SceneManager.LoadScene("Game Scene");
    }
}
