using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int numBoxes = 0;
    int boxesAtGoal = 0;
    bool levelWon = false;

    HUD hud;

    // Start is called before the first frame update
    void Start()
    {
        hud = FindObjectOfType<HUD>();
    }

    public void UpdateBoxes(bool _boxAtGoal)
    {
        boxesAtGoal += (_boxAtGoal ? 1 : -1);
        hud.UpdateBoxScore(numBoxes, boxesAtGoal);

        if (boxesAtGoal == numBoxes)
        {
            levelWon = true;
            hud.UpdateLevelWon(true);
        }  
    }

    public void FinishedGenerating(float _timeRunning, int _numBoxes)
    {
        hud.SetGeneratingTextActive(false);
        hud.UpdateTimeElapsedText(_timeRunning);

        numBoxes = _numBoxes;
        hud.UpdateBoxScore(numBoxes, 0);
    }

    public void ResetLevel()
    {
        levelWon = false;
        boxesAtGoal = 0;
        hud.UpdateLevelWon(false);
        hud.UpdateBoxScore(numBoxes, 0);
    }

    public bool GetLevelWon()
    {
        return levelWon;
    }
}
