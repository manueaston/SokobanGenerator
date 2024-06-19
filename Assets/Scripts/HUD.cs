using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text boxScoreText;
    public GameObject generatingTextObject;
    public Text generationTimeText;
    public Text playTimeText;
    public Text difficultyText;
    public Text adaptiveDifficultyText;
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
        generationTimeText.text = "Generation Time = " + newTime.ToString("F2") + "s";
    }

    public void UpdatePlayTimeText(float newTime)
    {
        playTimeText.text = "Puzzle Completion Time: " + newTime.ToString("F0") + "s";
    }

    public void UpdateDifficultyText(int difficulty)
    {
        difficultyText.text = "Current Difficulty: " + difficulty.ToString("F0");
    }

    public void UpdateAdaptiveDifficultyText(bool on)
    {
        if (on)
        {
            adaptiveDifficultyText.text = "True";
        }
        else
        {
            adaptiveDifficultyText.text = "False";
        }
    }
}