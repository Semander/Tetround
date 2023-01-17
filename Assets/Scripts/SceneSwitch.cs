using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
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

    public void PlayGame()
    {
        SceneManager.LoadScene("Official Levels");
    }

    public void Editor()
    {
        Debug.Log(Application.streamingAssetsPath);
        SceneManager.LoadScene("User Levels");
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
