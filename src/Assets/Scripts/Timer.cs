using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private WeaponAssaultRifle weapon;

    [SerializeField]
    GameObject gameOverM;

    [SerializeField]
    private Text countTextA;


    [SerializeField]
    public float setTime;
    [SerializeField]
    private Text countdownText;


    private void Start()
    {
        gameOverM.SetActive(false);
        
        countdownText.text = setTime.ToString();
    }
    private void Update()
    {
        if(setTime > 0)
        {
            setTime -= Time.deltaTime;
            if (weapon.AllAmmo <= 0)
            {
                GameOver();
            }
        }
        else if(setTime<= 0)
        {
            GameOver();
        }

        countdownText.text = Mathf.Round(setTime).ToString();
    }

    public void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0.0f;
        gameOverM.SetActive(true);
        Score.count = Score.countHead + Score.countArm + Score.countBody + Score.countLeg;
        countTextA.text = "Score: " + Score.currentScore + "\nAll: " + Score.count+ "\nHeadShot: " + Score.countHead+"\nArm: " +Score.countArm+ "\nBody: " +
            Score.countBody + "\nLeg: " + Score.countLeg + "\nAccuracy: " + Math.Round(((double)Score.count/(double)Score.shotNum)*100)+"%";

    }

}