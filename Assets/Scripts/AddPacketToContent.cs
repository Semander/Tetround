using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPacketToContent : MonoBehaviour
{
    //public GameObject content;

    public List<GameObject> packetList;

    GameObject RPacket;

    RandPacket randPacket;
    List <RandPacket> randPacketList = new List<RandPacket>();

    List <long> packetBits = new List<long>();

    int currWave;


    private void Awake()
    {
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
        SetValues();

        packetBits.Add(0);
    }
    public void SetValues()
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

    public void LoadToJson()
    {

        for (int i = 0; i < randPacketList.Count; i++)
        {
            packetBits[i] = randPacketList[i].UpdBits(); // update all needed data
        }


    }
}
