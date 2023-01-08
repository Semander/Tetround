using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayGame()
    {
        SceneManager.LoadScene("Game Scene");
    }

    public void Editor()
    {
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
