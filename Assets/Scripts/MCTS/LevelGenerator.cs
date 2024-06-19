using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{
    HUD hud;
    GameManager gameManager;

    State initialBoard = new State();

    Node rootNode;
    Node savedNode;

    public bool running = false;
    bool solutionFound = false;
    float startTime = 0.0f;
    float generationTime = 0.0f;
    float gameTimer = 0.0f;

    int maxDifficulty = 4;
    int[] minBoxNums = { 1, 1, 2, 2, 3 };
    int[] minBoxMoves = { 2, 3, 2, 3, 2 };
    public int currentMinBoxNum = 1;
    public int currentMinBoxMove = 2;

    public GameObject wall;
    public GameObject player;
    public GameObject box;
    public GameObject goal;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        hud = FindObjectOfType<HUD>();

        initialBoard.Initialise();
    }

    public IEnumerator GeneratePuzzle(int difficulty)
    {
        if (difficulty > maxDifficulty)
        {
            Debug.Log("Max Difficulty reached");
            difficulty = maxDifficulty;
        }

        // Set difficulty variables
        currentMinBoxNum = minBoxNums[difficulty];
        currentMinBoxMove = minBoxMoves[difficulty];

        rootNode = new Node(initialBoard, this, EActionType.EvaluateLevel);
        savedNode = rootNode;
        CreateLevel(rootNode.nodeState);

        running = true;
        startTime = Time.realtimeSinceStartup;
        hud.SetGeneratingTextActive(true);

        // Wait for next frame
        yield return 0;

        while (!solutionFound)
        {
            solutionFound = rootNode.SearchTree();
        }

        CreateFinalLevel(savedNode.nodeState);
        generationTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Time taken to generate level of difficulty " + difficulty + ": " + generationTime);
        hud.UpdateTimeElapsedText(generationTime);
        gameTimer = 0.0f;

        // Delete tree
        rootNode = null;
        running = false;
        solutionFound = false;
    }

    private void Update()
    {
        if (!running)
        {
            UpdateGameTimer();
            CheckResetLevel();
        }
    }

    public void SaveLevel(Node _savedNode)
    {
        savedNode = _savedNode;
    }

    public void CreateFinalLevel(State _board)
    {
        //Debug.Log("Final evalution score = " + savedNode.GetAverageEvaluationScore());

        // Clear current level
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        _board.ApplyPostProcessing();

        CreateLevel(_board);
        running = false;

        gameManager.FinishedGenerating(generationTime, savedNode.nodeState.GetBoxCount());
    }

    void ResetLevel()
    {
        // Clear current level
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        // Regenerate level
        CreateLevel(savedNode.nodeState);
    }

    void CreateLevel(State _board)
    {
        int xStartPos = -Util.width / 2 - 1;
        int yStartPos = -Util.height / 2 - 1;
        int xPos = xStartPos;
        int yPos = yStartPos;

        // + 2 accounting for outer wall
        for (int i = 0; i < Util.height + 2; i++)
        {
            for (int j = 0; j < Util.width + 2;  j++)
            {
                if (i == 0 || i == (Util.height + 1) || j == 0 || j == (Util.width + 1))
                {
                    // Outer wall
                    Instantiate(wall, new Vector3(xPos, yPos, 0.0f), Quaternion.identity, this.transform);
                }
                else
                {
                    switch (_board.boardState[i - 1, j - 1])
                    {
                        case 'w':
                        case 'o':
                            Instantiate(wall, new Vector3(xPos, yPos, 0.0f), Quaternion.identity, this.transform);
                            break;
                        case 'p':
                            Instantiate(player, new Vector3(xPos, yPos, 0.0f), Quaternion.identity, this.transform);
                            break;
                        case 'b':
                            Instantiate(box, new Vector3(xPos, yPos, 0.0f), Quaternion.identity, this.transform);
                            break;
                        case 'g':
                            Instantiate(goal, new Vector3(xPos, yPos, 0.0f), Quaternion.identity, this.transform);
                            break;
                        default:
                            break;
                    }
                }

                xPos++;
            }

            yPos++;
            xPos = xStartPos;
        }
    }

    void CheckResetLevel()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetLevel();
            gameManager.ResetLevel();
        }
    }

    void UpdateGameTimer()
    {
        gameTimer += Time.deltaTime;
    }
}


