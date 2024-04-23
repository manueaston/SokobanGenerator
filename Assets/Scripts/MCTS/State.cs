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
        // Get agent position

        // Choose random neighboring space that is empty or has a movable box

        // Set original position to empty
        // Set new position to player
        // If box moved, set new box position



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
        if (boardState[_pos.x - 1, _pos.y] == _spaceType)
            validNeighbors.Add(new Vector2Int(_pos.x - 1, _pos.y));

        if (boardState[_pos.x + 1, _pos.y] == _spaceType)
            validNeighbors.Add(new Vector2Int(_pos.x + 1, _pos.y));

        if (boardState[_pos.x, _pos.y - 1] == _spaceType)
            validNeighbors.Add(new Vector2Int(_pos.x, _pos.y - 1));

        if (boardState[_pos.x, _pos.y + 1] == _spaceType)
            validNeighbors.Add(new Vector2Int(_pos.x , _pos.y + 1));


        if (validNeighbors.Count == 0)
            // No neighboring spaces of space type
            return Vector2Int.zero;
        else
            // Return random neigbor from valid neighbor list
            return validNeighbors[Random.Range(0, validNeighbors.Count - 1)];
    }
}
