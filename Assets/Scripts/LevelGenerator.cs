using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////
// 
//  o = outerWall
//  w = wall
//  p = player
//  b = box
//  g = goal
//  e = empty
//
//////////

public struct BoardState
{
    public char[,] layout;

    public void Initialise(int _width, int _height, char _initObj = 'e')
    {
        // Adds 2 more spaces in each direction for outer walls of level
        // Also makes neighbour checking easier, can't go out of bounds
        layout = new char[_height + 2, _width + 2];

        for (int i = 0; i < _height + 2; i++)
        {
            for (int j = 0; j < _width + 2; j++)
            {
                if (i == 0 || i == _height + 1 || j == 0 || j == _width + 1)
                {
                    layout[i, j] = 'o';
                }
                else
                {
                    layout[i, j] = _initObj;
                }    
            }
        }
    }
}

public class LevelGenerator : MonoBehaviour
{
    static int levelWidth = 5;
    static int levelHeight = 5;
    float k = 1.0f;

    BoardState initialBoard;

    public GameObject wall;
    public GameObject player;
    public GameObject box;
    public GameObject goal;

    // Start is called before the first frame update
    void Start()
    {
        initialBoard.Initialise(levelWidth, levelHeight, 'w');
        CreateLevel(initialBoard);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetupInitialBoard()
    {
        for (int i = 0; i < levelHeight; i++)
        {
            for (int j = 0; j < levelWidth; j++)
            {
                if (i == (levelHeight - 1) / 2 && j == (levelWidth - 1) / 2)
                {
                    initialBoard.layout[i, j] = 'p';
                }
                else
                {
                    initialBoard.layout[i, j] = 'w';
                }
            }
        }
    }

    float Evaluate()
    {
        //float p = Mathf.Sqrt(TerrainMetric() * CongestionMetric()) / k;

        //return p;
        return 1.0f;
    }

    float CongestionMetric()
    {
        return 1.0f;
    }

    float TerrainMetric(BoardState _board)
    {
        int terainMetric = 0;

        for (int i = 1; i < levelHeight + 1; i++)
        {
            for (int j = 1; j < levelWidth + 1; j++)
            {
                if (_board.layout[i, j] == 'e')
                {
                    // Adds 1 to terrainMetric for every neighbouring wall
                    terainMetric += (_board.layout[i - 1, j - 1] == 'w') ? 1 : 0;
                    terainMetric += (_board.layout[i - 1, j] == 'w') ? 1 : 0;
                    terainMetric += (_board.layout[i - 1, j + 1] == 'w') ? 1 : 0;
                    terainMetric += (_board.layout[i, j - 1] == 'w') ? 1 : 0;
                    terainMetric += (_board.layout[i, j + 1] == 'w') ? 1 : 0;
                    terainMetric += (_board.layout[i + 1, j - 1] == 'w') ? 1 : 0;
                    terainMetric += (_board.layout[i + 1, j] == 'w') ? 1 : 0;
                    terainMetric += (_board.layout[i + 1, j + 1] == 'w') ? 1 : 0;
                }
            }
        }

        return terainMetric;

        // I think the max possible value is 40
        // max value equation:                      i * (i - 1) * 2
    }

    void CreateLevel(BoardState _board)
    {
        int xStartPos = -(levelWidth + 1) / 2;
        int yStartPos = -(levelHeight + 1) / 2;
        int xPos = xStartPos;
        int yPos = yStartPos;

        // + 2 accounting for outer wall
        for (int i = 0; i < levelHeight + 2; i++)
        {
            for (int j = 0; j < levelWidth + 2;  j++)
            {
                switch(_board.layout[i, j])
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
