using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text boxScoreText;
    public GameObject winTextObject;

    public void UpdateLevelWon(bool _levelWon)
    {
        winTextObject.SetActive(_levelWon);
    }

    public void UpdateBoxScore(int _numBoxes, int _boxScore)
    {
        boxScoreText.text = "Boxes At Goal: " + _boxScore.ToString() + "/" + _numBoxes.ToString();
    }
}