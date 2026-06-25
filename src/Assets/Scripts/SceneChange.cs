using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void Changed()
    {
        SceneManager.LoadScene("GameScene");
        //Debug.Log("게임시작");
    }
    public void OnClickRestart()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
        Debug.Log("게임종료");
#endif
    }

}
