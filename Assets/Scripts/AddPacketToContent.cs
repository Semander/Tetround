using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    private void Start()
    {
        LoadFromJson();
    }

    public void AddPacket()
    {
        Debug.Log("lol");

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
        string json = File.ReadAllText(Application.dataPath + "/" + FileNameController.filePath + ".json");
        myGameSettings = JsonUtility.FromJson<GameSettings>(json);

        Debug.Log(Application.dataPath + "/" + FileNameController.filePath + ".json");

        myGameMode = myGameSettings.gameMode;
        myWaveList = myGameSettings.wave;

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

    public void LoadToJson()
    {

        for (int i = 0; i < randPacketList.Count; i++)
        {
            myWave = randPacketList[i].UpdBits(); // update all needed data
        }


    }
}
