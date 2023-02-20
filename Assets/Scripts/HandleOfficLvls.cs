using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleOfficLvls : MonoBehaviour
{
    public LevelSave myLevelSave = new LevelSave();
    public List<Official> myOfficialList = new List<Official>();
    public Official myOfficial = new Official();

    public List<GameObject> OLevelList;
    public GameObject OLevel;

    public List<OfficLevel> officLevelList;
    public OfficLevel officLevel;

    public GameSettings myGameSettings = new GameSettings();
    public GameMode myGameMode = new GameMode();

    // Start is called before the first frame update
    void Start()
    {
        LoadFromJson();
        Debug.Log(Application.persistentDataPath);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void UpdAllValues()
    {
        for (int i = 0; i < officLevelList.Count; i++)
        {
            officLevelList[i].UpdId(i);
        }
    }

    public void LoadFromJson()
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/SaveData/SaveFile.json");
        myLevelSave = JsonUtility.FromJson<LevelSave>(json);

        myOfficialList = myLevelSave.official;

        for (int j = 0; j < myOfficialList.Count; j++)
        {
            myOfficial = myOfficialList[j];

            OLevel = Instantiate(Resources.Load("OfficialLevel")) as GameObject;
            OLevel.transform.SetParent(transform, false);
            OLevel.transform.SetAsFirstSibling();

            OLevelList.Add(OLevel);

            officLevel = OLevel.GetComponent<OfficLevel>();
            officLevel.parentScript = this;
            officLevelList.Add(officLevel);

            officLevel.setLevelData(myOfficial);
        }
        UpdAllValues();
    }
}
