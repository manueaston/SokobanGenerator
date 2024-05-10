using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{
    State initialBoard = new State();

    Node rootNode;

    bool running = false;
    int totalIterations = 10000;
    int currentIteration = 1;

    public GameObject wall;
    public GameObject player;
    public GameObject box;
    public GameObject goal;


    // TODO ////////
    // Check why it's always choosing evaluate level
    ////////////////


    // Start is called before the first frame update
    void Start()
    {
        initialBoard.Initialise();

        rootNode = new Node(initialBoard, this);
        CreateLevel(rootNode.nodeState);

        StartMCTS();
    }

    void StartMCTS()
    {
        running = true;
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
                running = false;
            }   
        }
    }

    public void CreateLevel(State _board)
    {
        Debug.Log("Creating saved level");

        // Clear current level
        foreach(Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        _board.ApplyPostProcessing();

        int xStartPos = -Util.width / 2;
        int yStartPos = -Util.height / 2;
        int xPos = xStartPos;
        int yPos = yStartPos;

        // + 2 accounting for outer wall
        for (int i = 0; i < Util.height; i++)
        {
            for (int j = 0; j < Util.width;  j++)
            {
                switch(_board.boardState[i, j])
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

                xPos++;
            }

            yPos++;
            xPos = xStartPos;
        }
    }
}
