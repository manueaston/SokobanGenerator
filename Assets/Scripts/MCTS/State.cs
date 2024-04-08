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
}
