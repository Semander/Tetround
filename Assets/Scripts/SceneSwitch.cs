using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public enum sceneName
    {
        Menu,
        Official,
        User,
        Editor,
        Game,
    }
    public static LinkedList<sceneName> scenesHistory = new LinkedList<sceneName>() { };

    public static Dictionary<sceneName, string> scenes = new Dictionary<sceneName, string>()
    {
        { sceneName.Menu, "Menu"},
        { sceneName.Official, "Official Levels"},
        { sceneName.User, "User Levels"},
        { sceneName.Editor, "Editor"},
        { sceneName.Game, "Game scene"},
    };

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log(Application.persistentDataPath + "/SaveData");

        string targetDir = Application.persistentDataPath + "/SaveData";
        if (!Directory.Exists(targetDir))
        {
            string source = Application.streamingAssetsPath + "/SaveData";

            Directory.CreateDirectory(targetDir);
            foreach(var file in Directory.GetFiles(source))
            {
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
            }
        }
    }
    public void Menu()
    {
        SceneManager.LoadScene(scenes[sceneName.Menu]);
    }
    public void PlayOfficialLevels()
    {
        scenesHistory.AddLast(sceneName.Official);

        SceneManager.LoadScene(scenes[sceneName.Official]);
    }
    public void EditorUserLevels()
    {
        scenesHistory.AddLast(sceneName.User);

        Debug.Log(Application.streamingAssetsPath);
        SceneManager.LoadScene(scenes[sceneName.User]);
    }
    public void EditorScene()
    {
        scenesHistory.AddLast(sceneName.Editor);

        SceneManager.LoadScene(scenes[sceneName.Editor]);
    }
    public void GameScene()
    {
        scenesHistory.AddLast(sceneName.Game);

        SceneManager.LoadScene(scenes[sceneName.Game]);
    }
    public void Back()
    {
        if (scenesHistory.Count > 0)
        {
            scenesHistory.RemoveLast();
        }

        switch (scenesHistory.Last.Value)
        {
            case sceneName.Official:
                SceneManager.LoadScene(scenes[sceneName.Official]);
                break;
            case sceneName.User:
                SceneManager.LoadScene(scenes[sceneName.User]);
                break;
            case sceneName.Editor:
                SceneManager.LoadScene(scenes[sceneName.Editor]);
                break;
            case sceneName.Game:
                SceneManager.LoadScene(scenes[sceneName.Game]);
                break;
            default: // Get back to menu
                SceneManager.LoadScene(scenes[sceneName.Menu]);
                break;
        }

    }
    

    // Update is called once per frame
    public void Exit()
    {
        Application.Quit();
        Debug.Log("Finish the G");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
