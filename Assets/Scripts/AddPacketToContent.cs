using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AddPacketToContent : MonoBehaviour
{
    //public GameObject content;

    public List<GameObject> packetList;

    GameObject RPacket;

    RandPacket randPacket;
    List <RandPacket> randPacketList = new List<RandPacket>();

    int currWave;

    public GameSettings myGameSettings = new GameSettings();
    public GameMode myGameMode = new GameMode();
    public List<Wave> myWaveList = new List<Wave>();
    public Wave myWave = new Wave();

    public GameObject RuleWindow;

    public Toggle rotClock45Toggle;
    public Toggle rotCount45Toggle;
    public Toggle rotClock90Toggle;
    public Toggle rotCount90Toggle;
    public Toggle rotClock180Toggle;
    public Toggle mirroringToggle;
    public Toggle moveRightToggle;
    public Toggle moveLeftToggle;
    public Toggle moveDownToggle;
    public Toggle gravityToggle;

    private void Start()
    {
        LoadFromJson();
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("User Levels");
    }

    public void OpenRuleWindow()
    {
        RuleWindow.SetActive(true);
    }

    public void CloseRuleWindow()
    {
        RuleWindow.SetActive(false);
        LoadRulesToJson();
    }

    public void AddPacket()
    {
        RPacket = Instantiate(Resources.Load("RandomPacket")) as GameObject;
        RPacket.transform.SetParent(transform, false);
        RPacket.transform.SetAsFirstSibling();

        packetList.Add(RPacket);

        randPacket = RPacket.GetComponent<RandPacket>();
        randPacket.parentScript = this;
        randPacketList.Add(randPacket);
        UpdAllValues();
    }
    public void UpdAllValues()
    {
        currWave = 1;
        for (int i = 0; i < randPacketList.Count; i++)
        {
            randPacketList[i].UpdId(i);
            currWave = randPacketList[i].UpdCurrentWave(currWave); // update all needed data
        }
    }

    public void UpdCurrWave()
    {
        currWave = 1;
        for (int i = 0; i < randPacketList.Count; i++)
        {
            currWave = randPacketList[i].UpdCurrentWave(currWave); // update all needed data
        }
    }

    public void DeleteRPackage(int Id)
    {
        RPacket = packetList[Id];
        packetList.RemoveAt(Id);
        randPacketList.RemoveAt(Id);
        Destroy(RPacket);

        UpdAllValues();
    }

    public void LoadFromJson()
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/SaveData/" + FileNameController.filePath + ".json");
        myGameSettings = JsonUtility.FromJson<GameSettings>(json);

        myGameMode = myGameSettings.gameMode;
        myWaveList = myGameSettings.waveList;

         rotClock45Toggle.isOn = myGameMode.rotClock45;
         rotCount45Toggle.isOn = myGameMode.rotCount45;
         rotClock90Toggle.isOn = myGameMode.rotClock90;
         rotCount90Toggle.isOn = myGameMode.rotCount90;
         rotClock180Toggle.isOn = myGameMode.rotClock180;
         mirroringToggle.isOn = myGameMode.mirroring;
         moveRightToggle.isOn = myGameMode.moveRight;
         moveLeftToggle.isOn = myGameMode.moveLeft;
         moveDownToggle.isOn = myGameMode.moveDown;
         gravityToggle.isOn = myGameMode.gravity;

        for (int j = 0; j < myWaveList.Count; j++)
        {
            myWave = myWaveList[j];

            RPacket = Instantiate(Resources.Load("RandomPacket")) as GameObject;
            RPacket.transform.SetParent(transform, false);
            RPacket.transform.SetAsFirstSibling();

            packetList.Add(RPacket);

            randPacket = RPacket.GetComponent<RandPacket>();
            randPacket.parentScript = this;
            randPacketList.Add(randPacket);
            UpdAllValues();

            randPacket.UpdWave(myWave);
        }
        UpdCurrWave();
    }

    public void SaveAndPlay()
    {
        LoadPacketsToJson();
        SceneManager.LoadScene("Game scene");
    }

    public void LoadRulesToJson()
    {
        myGameMode.rotClock45 = rotClock45Toggle.isOn;
        myGameMode.rotCount45 = rotCount45Toggle.isOn;
        myGameMode.rotClock90 = rotClock90Toggle.isOn;
        myGameMode.rotCount90 = rotCount90Toggle.isOn;
        myGameMode.rotClock180 = rotClock180Toggle.isOn;
        myGameMode.mirroring = mirroringToggle.isOn;
        myGameMode.moveRight = moveRightToggle.isOn;
        myGameMode.moveLeft = moveLeftToggle.isOn;
        myGameMode.moveDown = moveDownToggle.isOn;
        myGameMode.gravity = gravityToggle.isOn;

        myGameSettings.gameMode = myGameMode;
        myGameSettings.waveList = myWaveList;

        string json = JsonUtility.ToJson(myGameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/SaveData/" + FileNameController.filePath + ".json", json);
    }

    public void LoadPacketsToJson()
    {
        myWaveList.Clear();
        for (int i = 0; i < randPacketList.Count; i++)
        {
            myWaveList.Add(randPacketList[i].UpdBits()); // update all needed data and pass the Wave object
        }

        myGameSettings.gameMode = myGameMode;
        myGameSettings.waveList = myWaveList;

        string json = JsonUtility.ToJson(myGameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/SaveData/" + FileNameController.filePath + ".json", json);
        Debug.Log(json);
        Debug.Log("Must work...");
    }
 }
