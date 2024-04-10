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

    int boxCount = 0;
    List<Vector2> boxPos;
    List<Vector2> boxStartPos;

    public void Initialise(int _width, int _height, char _initObj = 'e')
    {
        // Adds 2 more spaces in each direction for outer walls of level
        // Also makes neighbour checking easier, can't go out of bounds
        boardState = new char[_height + 2, _width + 2];

        for (int i = 0; i < _height + 2; i++)
        {
            for (int j = 0; j < _width + 2; j++)
            {
                if (i == 0 || i == _height + 1 || j == 0 || j == _width + 1)
                {
                    boardState[i, j] = 'o';
                }
                else
                {
                    boardState[i, j] = _initObj;
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
}
