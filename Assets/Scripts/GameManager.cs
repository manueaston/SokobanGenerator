using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int numBoxes = 0;
    int boxesAtGoal = 0;
    bool levelWon = false;
    public int currentDifficulty;
    public bool adaptiveDifficultyOn;
    int puzzleCounter = 0;
    float timeToSolve = 0.0f;

    HUD hud;
    LevelGenerator lg;

    // Start is called before the first frame update
    void Start()
    {
        hud = FindObjectOfType<HUD>();
        lg = FindObjectOfType<LevelGenerator>();

        currentDifficulty = PlayerPrefs.GetInt("Difficulty");
        adaptiveDifficultyOn = (PlayerPrefs.GetInt("AdaptiveDifficultyOn") > 0) ? true : false;
        
        hud.UpdateDifficultyText(currentDifficulty + 1); // Indexed 0, but displayed as index 1
        hud.UpdateAdaptiveDifficultyText(adaptiveDifficultyOn);

        StartCoroutine(lg.GeneratePuzzle(currentDifficulty)); 

    }

    private void Update()
    {
        CheckExit();
        CheckRegenerateLevel();

        if (!lg.running && !levelWon)
        {
            timeToSolve += Time.deltaTime;
            hud.UpdatePlayTimeText(timeToSolve);
        }
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

    void CheckRegenerateLevel()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Adaptive difficulty 
            if (adaptiveDifficultyOn)
            {
                if (levelWon && timeToSolve < 10.0f && currentDifficulty < 4)
                {
                    currentDifficulty++;
                }
                else if (currentDifficulty > 0)
                {
                    currentDifficulty--;
                }

                PlayerPrefs.SetInt("Difficulty", currentDifficulty);
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void CheckExit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public bool GetLevelWon()
    {
        return levelWon;
    }

    public void ToggleAdaptiveDifficulty()
    {
        adaptiveDifficultyOn = !adaptiveDifficultyOn;
        hud.UpdateAdaptiveDifficultyText(adaptiveDifficultyOn);
        adaptiveDifficultyOn = (PlayerPrefs.GetInt("AdaptiveDifficultyOn") > 0) ? true : false;
        PlayerPrefs.SetInt("AdaptiveDifficultyOn", (adaptiveDifficultyOn ? 0 : 1));
    }

    public void IncreaseDifficulty()
    {
        if (currentDifficulty < 4)
        {
            currentDifficulty++;
            PlayerPrefs.SetInt("Difficulty", currentDifficulty);
            hud.UpdateDifficultyText(currentDifficulty + 1); // Indexed 0, but displayed as index 1
        }
    }

    public void DecreaseDifficulty()
    {
        if (currentDifficulty > 0)
        {
            currentDifficulty--;
            PlayerPrefs.SetInt("Difficulty", currentDifficulty);
            hud.UpdateDifficultyText(currentDifficulty + 1); // Indexed 0, but displayed as index 1
        }
    }
}
