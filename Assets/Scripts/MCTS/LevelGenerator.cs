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

    bool running = false;
    int totalIterations = 500000;
    int currentIteration = 1;
    public float secondsToRun = 80.0f;
    float startTime = 0.0f;

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

        rootNode = new Node(initialBoard, this, EActionType.EvaluateLevel);
        savedNode = rootNode;
        GenerateLevel(rootNode.nodeState);

        StartCoroutine(StartMCTS());
    }

    IEnumerator StartMCTS()
    {
        running = true;
        startTime = Time.realtimeSinceStartup;
        hud.SetGeneratingTextActive(true);

        // Wait for next frame
        yield return 0;

        //for (int i = 0; i < totalIterations; i++)
        //{
        //    rootNode.SearchTree();
        //}

        while (Time.realtimeSinceStartup - startTime < secondsToRun)
        {
            rootNode.SearchTree();
        }

        CreateFinalLevel(savedNode.nodeState);
    }

    private void Update()
    {
        if (!running)
        {
            CheckResetLevel();
        }
    }

    public void SaveLevel(Node _savedNode)
    {
        savedNode = _savedNode;
    }

    public void CreateFinalLevel(State _board)
    {
        Debug.Log("Final evalution score = " + (savedNode.evaluationScoreSum / savedNode.visitCount));

        // Clear current level
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        _board.ApplyPostProcessing();

        GenerateLevel(_board);
        running = false;

        gameManager.FinishedGenerating(secondsToRun, savedNode.nodeState.GetBoxCount());
    }

    void ResetLevel()
    {
        // Clear current level
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        // Regenerate level
        GenerateLevel(savedNode.nodeState);
    }

    public void GenerateLevel(State _board)
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
}


