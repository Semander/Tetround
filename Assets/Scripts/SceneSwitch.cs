using UnityEngine;
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
