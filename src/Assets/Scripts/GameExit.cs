using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameExit : MonoBehaviour
{
    public void GameQuit()
    {
        Application.Quit();
        Debug.Log("게임종료");
    }
}
