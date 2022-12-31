using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSettings
{
    public bool isCompleted; // Did you finish the level?
    public int score; // Best score so far (even if no finish)
    public GameMode gameMode; // All settings for buttons and gravity
    public List<Wave> wave; // Pieces allignment and amount of waves
}

[Serializable]
public class GameMode
{// (values 0 and 1 instead of bool)
    public bool rotClock45;
    public bool rotCount45;
    public bool rotClock90;
    public bool rotCount90;
    public bool rotClock180;
    public bool mirroring;
    public bool moveRight;
    public bool moveLeft;
    public bool moveDown;
    public bool drop;
    public bool gravity;
}

[Serializable]
public class Wave
{
    public int waveAmount; // amount of waves this set will work
    public int shapes;
}

public class JsonReadWriteSystem : MonoBehaviour
{
    public GameSettings myGameSettings = new GameSettings();
    public GameMode myGameMode = new GameMode();
    public Wave myWave = new Wave();
    public void SaveToJson(string filePath)
    {

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


        string json = JsonUtility.ToJson(myGameMode, true);
        //File.WriteAllText(Application.dataPath + "/SaveData/Level 0.json", json);
    }


    public GameMode LoadFromJson()
    {
        string json = File.ReadAllText(Application.dataPath + "/" + FileNameController.filePath + ".json");

        GameMode myGameMode = JsonUtility.FromJson<GameMode>(json);

        return myGameMode;

    }
}
