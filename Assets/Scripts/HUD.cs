using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text boxScoreText;
    public GameObject generatingTextObject;
    public Text timeElapsedText;
    public GameObject winTextObject;

    public void UpdateLevelWon(bool _levelWon)
    {
        winTextObject.SetActive(_levelWon);
    }

    public void UpdateBoxScore(int _numBoxes, int _boxScore)
    {
        boxScoreText.text = "Boxes At Goal: " + _boxScore.ToString() + "/" + _numBoxes.ToString();
    }

    public void SetGeneratingTextActive(bool _active)
    {
        generatingTextObject.SetActive(_active);
    }

    public void UpdateTimeElapsedText(float newTime)
    {
        int minutes = Mathf.FloorToInt(newTime / 60.0f);
        int seconds = Mathf.FloorToInt(newTime % 60.0f);
        timeElapsedText.text = minutes.ToString() + ":" + seconds.ToString();
    }
}