using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleUserLvls : MonoBehaviour
{
    public LevelSave myLevelSave = new LevelSave();
    public List<User> myUserList = new List<User>();
    public User myUser = new User();

    public List<GameObject> ULevelList;
    public GameObject ULevel;

    public List<UserLevel> userLevelList;
    public UserLevel userLevel;

    public GameSettings myGameSettings = new GameSettings();
    public GameMode myGameMode = new GameMode();

    public List<int> idList = new List<int>();




    // Start is called before the first frame update
    void Start()
    {
        LoadFromJson();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void UpdAllValues()
    {
        for (int i = 0; i < userLevelList.Count; i++)
        {
            userLevelList[i].UpdId(i);
        }
    }

    public void AddLevel()
    {
        ULevel = Instantiate(Resources.Load("LevelByUser")) as GameObject;
        ULevel.transform.SetParent(transform, false);
        ULevel.transform.SetAsFirstSibling();

        ULevelList.Add(ULevel);

        userLevel = ULevel.GetComponent<UserLevel>();

        int SaveID = 0;
        Debug.Log("Contains " + SaveID + ": " + idList.Contains(SaveID));
        while (idList.Contains(SaveID))// find free index
        {
            SaveID++;

            if (SaveID >= 1000) {
                Debug.Log("Inf loop??");
                break; 
            }

        }
        idList.Add(SaveID);

        foreach(int ident in idList)
        {
            Debug.Log("All id in list" + ident);
        }

        userLevel.SaveID = SaveID;
        userLevel.parentScript = this;
        userLevelList.Add(userLevel);
        UpdAllValues();



        myGameMode.rotClock45 = true;
        myGameMode.rotCount45 = true;
        myGameMode.rotClock90 = true;
        myGameMode.rotCount90 = true;
        myGameMode.rotClock180 = true;
        myGameMode.mirroring = true;
        myGameMode.moveRight = true;
        myGameMode.moveLeft = true;
        myGameMode.moveDown = true;
        myGameMode.gravity = true;

        myGameSettings.gameMode = myGameMode;

        string json = JsonUtility.ToJson(myGameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/SaveData/" + SaveID.ToString() + ".json", json);

        myUser = new User();
        myUser.id = SaveID;
        myUser.name = "";
        myUser.isCompleted = false;

        Debug.Log("User object: " + myUser.id);

        myUserList.Add(myUser);
        myLevelSave.user = myUserList;


        json = JsonUtility.ToJson(myLevelSave, true);
        File.WriteAllText(Application.persistentDataPath + "/SaveData/SaveFile.json", json);
    }

    public void DeleteLevel(int Id)
    {
        int SaveID = userLevelList[Id].SaveID;

        ULevel = ULevelList[Id];
        ULevelList.RemoveAt(Id);
        userLevelList.RemoveAt(Id);
        myUserList.RemoveAt(Id);
        idList.RemoveAt(Id);
        Destroy(ULevel);

        UpdAllValues();

        File.Delete(Application.persistentDataPath + "/SaveData/" + SaveID.ToString() + ".json");
        LoadToJson();
    }

    public void LoadFromJson()
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/SaveData/SaveFile.json");
        myLevelSave = JsonUtility.FromJson<LevelSave>(json);

        myUserList = myLevelSave.user;

        foreach (User user in myUserList)
        {
            Debug.Log("Oh no..." + user.id);

            ULevel = Instantiate(Resources.Load("LevelByUser")) as GameObject;
            ULevel.transform.SetParent(transform, false);
            ULevel.transform.SetAsFirstSibling();

            ULevelList.Add(ULevel);

            userLevel = ULevel.GetComponent<UserLevel>();
            userLevel.parentScript = this;
            userLevelList.Add(userLevel);

            idList.Add(user.id);

            userLevel.setLevelData(user);
        }
        UpdAllValues();
    }

    public void UpdateName(int Id)
    {
        myUserList[Id].name = userLevelList[Id].levelName;
        Debug.Log(myUserList[Id].ToString());
    }

    public void LoadToJson()
    {
        myLevelSave.user = myUserList;

        string json = JsonUtility.ToJson(myLevelSave, true);
        File.WriteAllText(Application.persistentDataPath + "/SaveData/SaveFile.json", json);
    }
}
