using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserLevel : MonoBehaviour
{
    public int SaveID;
    public string levelName;
    bool isCompleted;
    User user = new User();

    public int id;
    public TMP_Text idText;
    public TMP_InputField nameInput;
    public Image Completion;

    [NonSerialized] public HandleUserLvls parentScript;

    private void Start()
    {
        Completion.enabled = false;
        nameInput.onEndEdit.AddListener(UpdName);
    }

    public void UpdName(string name)
    {
        levelName = name;
        parentScript.UpdateName(id);
        parentScript.LoadToJson();
    }

    public void UpdId(int ident)
    {
        id = ident;
        idText.text = (id + 1).ToString();
    }

    public void UpdLevelName(string inpText)
    {
        Debug.Log("Setting name: " + inpText);
        idText.text = inpText;
        levelName = inpText;
    } 

    public void setLevelData(User u)
    {
        SaveID = u.id;
        levelName = u.name;
        isCompleted = u.isCompleted;


        nameInput.text = levelName;
        Completion.enabled = isCompleted;
    }

    public User saveData()
    {
        user.id = SaveID;
        user.name = levelName;
        user.isCompleted = isCompleted;

        Debug.Log("id: " + SaveID);

        return user;
    }

    public void Destr()
    {
        parentScript.DeleteLevel(id);
    }

    public void Play() // Switch to play
    {
        parentScript.LoadToJson();
        FileNameController.filePath = SaveID.ToString();
        SceneManager.LoadScene("Game Scene");
    }

    public void Edit() // Switch to edit
    {
        parentScript.LoadToJson();
        FileNameController.filePath = SaveID.ToString();
        SceneManager.LoadScene("Editor");
    }
}
