using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{
    static int levelWidth = 5;
    static int levelHeight = 5;
    //float k = 1.0f;

    State initialBoard = new State();

    public GameObject wall;
    public GameObject player;
    public GameObject box;
    public GameObject goal;

    // Start is called before the first frame update
    void Start()
    {
        initialBoard.Initialise(levelWidth, levelHeight);
        CreateLevel(initialBoard);

        StartMCTS();
    }

    void SetupInitialBoard()
    {
        for (int i = 0; i < levelHeight; i++)
        {
            for (int j = 0; j < levelWidth; j++)
            {
                if (i == (levelHeight - 1) / 2 && j == (levelWidth - 1) / 2)
                {
                    initialBoard.boardState[i, j] = 'p';
                }
                else
                {
                    initialBoard.boardState[i, j] = 'w';
                }
            }
        }
    }

    void StartMCTS()
    {
        Debug.Log("Start MCTS");

        Node rootNode = new Node(initialBoard, this);

        //CreateLevel(rootNode.nodeState);

        int iterationNum = 1;

        for (int i = 0; i < iterationNum; i++)
        {
           // rootNode.SearchTree();
        }
    }
    
    public void CreateLevel(State _board)
    {
        _board.ApplyPostProcessing();

        int xStartPos = -(levelWidth + 1) / 2;
        int yStartPos = -(levelHeight + 1) / 2;
        int xPos = xStartPos;
        int yPos = yStartPos;

        // + 2 accounting for outer wall
        for (int i = 0; i < levelHeight + 2; i++)
        {
            for (int j = 0; j < levelWidth + 2;  j++)
            {
                switch(_board.boardState[i, j])
                {
                    case 'w':
                    case 'o':
                        Instantiate(wall, new Vector3(xPos, yPos, 0.0f), Quaternion.identity);
                        break;
                    case 'p':
                        Instantiate(player, new Vector3(xPos, yPos, 0.0f), Quaternion.identity);
                        break;
                    case 'b':
                        Instantiate(box, new Vector3(xPos, yPos, 0.0f), Quaternion.identity);
                        break;
                    case 'g':
                        Instantiate(goal, new Vector3(xPos, yPos, 0.0f), Quaternion.identity);
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
