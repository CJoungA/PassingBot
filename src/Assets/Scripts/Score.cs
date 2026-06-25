using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text headShotText;

    public static int currentScore;
    public static double accuracy;
    public static int countHead;
    public static int countBody;
    public static int countArm;
    public static int countLeg;
    public static int count;
    public static int shotNum;

    private void Awake()
    {
        currentScore = 0;
        countHead = 0;
        countBody = 0;
        countArm = 0;
        countLeg = 0;
        shotNum = 0;
        count = 0;
        accuracy = 0;
    }
    private void Update()
    {    
        scoreText.text = "Score: " + currentScore;
        headShotText.text = "HeadShot: " + countHead;
    }

}
