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

enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class State : MonoBehaviour
{
    public char[,] boardState;
    public bool frozen = false;
    public bool saved = false;

    int width;
    int height;

    Vector2Int playerPos;

    int boxCount = 0;
    List<Vector2Int> boxPos;
    List<Vector2Int> boxStartPos;

    public void Initialise(int _width, int _height)
    {
        // Adds 2 more spaces in each direction for outer walls of level
        // Also makes neighbour checking easier, can't go out of bounds
        width = _width;
        height = _height;

        boardState = new char[_height + 2, _width + 2];

        for (int i = 0; i < _height + 2; i++)
        {
            for (int j = 0; j < _width + 2; j++)
            {
                if (i == 0 || i == _height + 1 || j == 0 || j == _width + 1)
                {
                    // Outer walls around outside of board
                    boardState[i, j] = 'o';
                }
                else if (i == (height + 1) / 2 && j == (width + 1) / 2)
                {
                    // Player is in centre of board
                    boardState[i, j] = 'p';
                    playerPos = new Vector2Int(i, j);
                }
                else
                {
                    // Every other space is a wall obstacle
                    boardState[i, j] = 'w';
                }
            }
        }
    }

    public float GetCongestion()
    {
        // scaling weights
        float a = 1.0f;
        float b = 1.0f;
        float c = 1.0f;

        float congestionScore = 0.0f;

        for (int i = 0; i < boxCount; i++)
        {

        }

        return congestionScore;
    }

    public float GetBoxCount()
    {
        return boxCount;
    }

    public float Get3x3BlockCount()
    {
        return 0.0f;
    }

    float GetBoxStartPosInArea(Vector2 _areaStart, Vector2 _areaEnd)
    {
        float num = 0.0f;

        foreach(Vector2 pos in boxStartPos)
        {
            // Figure out how to check if its in area
        }

        return num;
    }

    public void DeleteRandomObstacle()
    {
        Vector2Int emptySpace;
        Vector2Int obstacleSpace;

        // Find obstacle next to empty space
        do
        {
            emptySpace = GetRandomSpace('e');
            obstacleSpace = GetRandomNeighbor(emptySpace, 'w');
        }
        while (obstacleSpace != Vector2Int.zero);

        // Remove obstacle from selected space
        boardState[obstacleSpace.x, obstacleSpace.y] = 'e';
    }

    public void PlaceRandomBox()
    {
        // Find random empty space and place box in space
        Vector2Int emptySpace = GetRandomSpace('e');
        boardState[emptySpace.x, emptySpace.y] = 'b';

        // Add position to box pos lists
        boxPos.Add(emptySpace);
        boxStartPos.Add(emptySpace);
    }

    public void MoveAgentRandomly()
    {
        List<Direction> possibleDirections = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        while(true)
        {
            // Choose random direction that hasn't been checked yet
            int randomIndex = Random.Range(0, possibleDirections.Count - 1);
            Direction direction = possibleDirections[randomIndex];

            // Get space in direction
            Vector2Int newSpace = GetSpace(playerPos, direction);

            // Check if player can move to this space
            if (boardState[newSpace.x, newSpace.y] == 'e')
            {
                // Can move to empty space
                SwapSpaces(playerPos, newSpace);
                playerPos = newSpace;
                break;
            }
            else if (boardState[newSpace.x, newSpace.y] == 'b')
            {
                Vector2Int newBoxSpace = GetSpace(newSpace, direction);

                // Check if box has empty space to be pushed into
                if (boardState[newBoxSpace.x, newBoxSpace.y] == 'e')
                {
                    // Can push box into empty space
                    SwapSpaces(newSpace, newBoxSpace);
                    SwapSpaces(playerPos, newSpace);
                    playerPos = newSpace;
                    break;
                }
            }

            // Space is not valid to move into
            // Remove direction from list of possible directions
            possibleDirections.RemoveAt(randomIndex);

            // Check if there are no valid directions in list
            if (possibleDirections.Count == 0)
            {
                Debug.LogError("No valid moves for player");
                break;
            }
        }
    }

    Vector2Int GetRandomSpace(char _spaceType)
    {
        Vector2Int space;

        // Generate random positions until space type matches
        do
        {
            space = new Vector2Int(Random.Range(1, height), Random.Range(1, width));
        }
        while (boardState[space.x, space.y] != _spaceType);

        return space;
    }

    Vector2Int GetRandomNeighbor(Vector2Int _pos, char _spaceType)
    {
        List<Vector2Int> validNeighbors = new List<Vector2Int>();

        // Add to list all neighbors that have matching space type
        foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
        {
            Vector2Int neighbor = GetSpace(_pos, direction);

            if (boardState[neighbor.x, neighbor.y] == _spaceType)
                validNeighbors.Add(neighbor);
        }


        if (validNeighbors.Count == 0)
            // No neighboring spaces of space type
            return Vector2Int.zero;
        else
            // Return random neigbor from valid neighbor list
            return validNeighbors[Random.Range(0, validNeighbors.Count - 1)];
    }

    Vector2Int GetRandomNeighbor(Vector2Int _pos, char _spaceType1, char _spaceType2)
    {
        List<Vector2Int> validNeighbors = new List<Vector2Int>();

        // Add to list all neighbors that have matching space type
        foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
        {
            Vector2Int neighbor = GetSpace(_pos, direction);

            if (boardState[neighbor.x, neighbor.y] == _spaceType1 || boardState[neighbor.x, neighbor.y] == _spaceType2)
                validNeighbors.Add(neighbor);
        }

        if (validNeighbors.Count == 0)
            // No neighboring spaces of space type
            return Vector2Int.zero;
        else
            // Return random neigbor from valid neighbor list
            return validNeighbors[Random.Range(0, validNeighbors.Count - 1)];
    }

    Vector2Int GetSpace(Vector2Int _startPos, Direction _dir)
    {
        switch (_dir)
        {
            case Direction.Up:
                return new Vector2Int(_startPos.x - 1, _startPos.y);

            case Direction.Down:
                return new Vector2Int(_startPos.x + 1, _startPos.y);

            case Direction.Left:
                return new Vector2Int(_startPos.x, _startPos.y - 1);

            case Direction.Right:
                return new Vector2Int(_startPos.x, _startPos.y + 1);

            default:
                return new Vector2Int(_startPos.x, _startPos.y);
        }
    }

    void SwapSpaces(Vector2Int _space1, Vector2Int _space2)
    {
        // Swaps values of two spaces
        char space1Value = boardState[_space1.x, _space1.y];
        boardState[_space1.x, _space1.y] = boardState[_space2.x, _space2.y];
        boardState[_space2.x, _space2.y] = space1Value;
    }
}
