using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{
    HUD hud;

    State initialBoard = new State();

    Node rootNode;
    Node savedNode;

    bool running = false;
    int totalIterations = 10000;
    int currentIteration = 1;
    float timeRunning = 0.0f;

    public GameObject wall;
    public GameObject player;
    public GameObject box;
    public GameObject goal;

    // Start is called before the first frame update
    void Start()
    {
        hud = FindObjectOfType<HUD>();

        initialBoard.Initialise();

        rootNode = new Node(initialBoard, this, EActionType.EvaluateLevel);
        savedNode = rootNode;
        GenerateLevel(rootNode.nodeState);

        StartMCTS();
    }

    void StartMCTS()
    {
        running = true;
        timeRunning = 0.0f;
        hud.SetGeneratingTextActive(true);
    }

    private void Update()
    {
        if (running)
        {
            rootNode.SearchTree();
            currentIteration++;

            if (currentIteration > totalIterations)
            {
                Debug.Log("Finished");
                CreateFinalLevel(savedNode.nodeState);
                running = false;
                hud.SetGeneratingTextActive(false);
            }

            timeRunning += Time.deltaTime;
            hud.UpdateTimeElapsedText(timeRunning);
        }
    }

    public void SaveLevel(Node _savedNode)
    {
        savedNode = _savedNode;
    }

    public void CreateFinalLevel(State _board)
    {
        Debug.Log("Creating saved level");

        // Clear current level
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        _board.ApplyPostProcessing();

        GenerateLevel(_board);
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
}
