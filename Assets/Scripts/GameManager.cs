using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int numBoxes = 0;
    int boxesAtGoal = 0;
    bool levelWon = false;

    HUD hud;
    Box[] boxes;

    // Start is called before the first frame update
    void Start()
    {
        hud = FindObjectOfType<HUD>();

        boxes = FindObjectsOfType<Box>();
        numBoxes = boxes.Length;

        hud.UpdateBoxScore(numBoxes, boxesAtGoal);
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

    public bool GetLevelWon()
    {
        return levelWon;
    }
}
