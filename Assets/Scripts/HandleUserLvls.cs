using System.Collections.Generic;
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
        Debug.Log("Yaay");
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
        int Id = 0;
        while (idList.Contains(Id))
        {
            Id++;
            if (Id >= 1000) break;

        }
        idList.Add(Id);

        userLevel.id = Id;
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
        myGameMode.drop = true;
        myGameMode.gravity = true;

        myGameSettings.gameMode = myGameMode;

        string json = JsonUtility.ToJson(myGameSettings, true);
        File.WriteAllText(Application.streamingAssetsPath + "/SaveData/" + Id.ToString() + ".json", json);

        myUser = new User();
        myUser.id = Id;
        myUser.name = "";
        myUser.isCompleted = false;

        myUserList.Add(myUser);
        myLevelSave.user = myUserList;

        json = JsonUtility.ToJson(myLevelSave, true);
        File.WriteAllText(Application.streamingAssetsPath + "/SaveData/SaveFile.json", json);
    }

    public void DeleteLevel(int Id)
    {
        ULevel = ULevelList[Id];
        ULevelList.RemoveAt(Id);
        userLevelList.RemoveAt(Id);
        Destroy(ULevel);

        UpdAllValues();

        File.Delete(Application.streamingAssetsPath + "/SaveData/" + Id.ToString() + ".json");
        LoadToJson();
    }

    public void LoadFromJson()
    {
        string json = File.ReadAllText(Application.streamingAssetsPath + "/SaveData/SaveFile.json");
        myLevelSave = JsonUtility.FromJson<LevelSave>(json);

        myUserList = myLevelSave.user;

        for (int j = 0; j < myUserList.Count; j++)
        {
            myUser = myUserList[j];

            ULevel = Instantiate(Resources.Load("LevelByUser")) as GameObject;
            ULevel.transform.SetParent(transform, false);
            ULevel.transform.SetAsFirstSibling();

            ULevelList.Add(ULevel);

            userLevel = ULevel.GetComponent<UserLevel>();
            userLevel.parentScript = this;
            userLevelList.Add(userLevel);
            UpdAllValues();

            Debug.Log("Oh no..." + userLevel.id);
            idList.Add(myUser.id);

            userLevel.setLevelData(myUser);
        }
    }

    public void LoadToJson()
    {
        myUserList.Clear();
        for (int i = 0; i < userLevelList.Count; i++)
        {
            myUserList.Add(userLevelList[i].saveData()); // update all needed data and pass the User object


            myLevelSave.user = myUserList;

            string json = JsonUtility.ToJson(myLevelSave, true);
            File.WriteAllText(Application.streamingAssetsPath + "/SaveData/SaveFile.json", json);
        }
    }
}
