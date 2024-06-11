using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////
//
//  w = wall
//  p = player
//  b = box
//  g = goal
//  e = empty
//
//////////

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class State
{
    public char[,] boardState = new char[Util.height, Util.width];
    public bool frozen = false;
    public bool saved = false;

    Vector2Int playerPos;

    int boxCount = 0;
    int emptyCount = 0;
    List<Vector2Int> boxPos = new List<Vector2Int>();
    List<Vector2Int> boxStartPos = new List<Vector2Int>();

    public State() { }

    public State(State _copyState)
    {
        frozen = _copyState.frozen;
        saved = _copyState.saved;

        playerPos = _copyState.playerPos;

        boxCount = _copyState.boxCount;
        emptyCount = _copyState.emptyCount;
        boxPos = new List<Vector2Int>(_copyState.boxPos);
        boxStartPos = new List<Vector2Int>(_copyState.boxStartPos);

        // Copy board values
        for (int y = 0; y < Util.height; y++)
        {
            for (int x = 0; x < Util.width; x++)
            {
                boardState[y, x] = _copyState.boardState[y, x];
            }
        }
    }

    // Indexer overrides
    public char this[int y, int x]
    {
        // using get accessor 
        get
        {
            char temp = boardState[y, x];
            return temp;
        }

        // using set accessor 
        set
        {
            boardState[y, x] = value;
        }
    }

    public char this[Vector2Int _pos]
    {
        // using get accessor 
        get
        {
            char temp = boardState[_pos.x, _pos.y];
            return temp;
        }

        // using set accessor 
        set
        {
            boardState[_pos.x, _pos.y] = value;
        }
    }

    public void Initialise()
    {
        for (int i = 0; i < Util.height; i++)
        {
            for (int j = 0; j < Util.width; j++)
            {
                if (i == (Util.height - 1) / 2 && j == (Util.width - 1) / 2)
                {
                    // Centre of board starts as empty, will be player start position
                    boardState[i, j] = 'e';
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
        float a = 5.0f;
        float b = 5.0f;
        float c = 1.0f;

        float congestionScore = 0.0f;

        for (int i = 0; i < boxCount; i++)
        {
            // Area variables
            float height;
            float width;
            // These counts are within bounding rectangle between start and goal for box
            float startCount = 0.0f;
            float goalCount = 0.0f;
            float wallCount = 0.0f;

            // Calculate area of bounding rectangle using box position and starting position
            height = Mathf.Abs(boxPos[i].x - boxStartPos[i].x) + 1; // + 1 because always includes starting space
            width = Mathf.Abs(boxPos[i].y - boxStartPos[i].y) + 1;

            for (int x = Mathf.Min(boxPos[i].x, boxStartPos[i].x); x <= Mathf.Max(boxPos[i].x, boxStartPos[i].x); x++)
            {
                for (int y = Mathf.Min(boxPos[i].y, boxStartPos[i].y); y <= Mathf.Max(boxPos[i].y, boxStartPos[i].y); y++)
                {
                    Vector2Int tilePos = new Vector2Int(x, y);
                    if (tilePos == boxPos[i] || tilePos == boxStartPos[i])
                    {
                        // Don't include start and end positions of box
                        break;
                    }

                    if (boardState[x, y] == 'b')    // check if tile is a goal 
                    {
                        goalCount++;
                    }
                    else if (boardState[x, y] == 'w')   // check if tile is a wall
                    {
                        wallCount++;
                    }
                    else if (IsBoxStartPos(tilePos))
                    {
                        startCount++;
                    }
                }
            }

            // Add box congestion value to congestion score
            float area = width * height;

            //Debug.Log("Start Count = " + startCount + ", Goal Count = " + goalCount + ", Area = " + area + ", Wall Count = " + wallCount);

            congestionScore += (((a * startCount) + (b * goalCount)) / (c * (area - wallCount)));
        }

        return congestionScore;
    }

    public bool IsBoxStartPos(Vector2Int _pos)
    {
        foreach (Vector2Int boxPos in boxStartPos)
        {
            if (boxPos == _pos)
                return true;
        }

        return false;
    }

    public int GetBoxCount()
    {
        return boxCount;
    }

    public int GetEmptyCount()
    {
        return emptyCount;
    }

    public float Get3x3BlockCount()
    {
        float[,] tilesInBlock = new float[Util.height, Util.width];
        // 1.0f = not in block, 0.0f = in block

        // Initialise all values to 1
        for (int i = 0; i < Util.height; i++)
        {
            for (int j = 0; j < Util.width; j++)
            {
                tilesInBlock[i, j] = 1.0f;
            }
        }

        // Check if tiles are in 3x3 block
        // If true, set value to 0
        // Don't need to check last 2 rows and columns because we are only checking the top left tile of a 3x3 block
        // Last 2 rows and columns cannot be the top left tile, there are not enough rows or columns after it
        for (int y = 0; y < Util.height - 2; y++)
        {
            for (int x = 0; x < Util.width - 2; x++)
            {
                // Get tile type 
                char tile = boardState[y, x];

                if (tile != 'e' && tile != 'w')
                {
                    // If not empty or obstacle, not part of 3x3 block
                    continue;
                }

                bool inBlock = true;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Checks tiles in 3x3 block
                        // if contains something other than empty space or obstacle, not in 3x3 block

                        char neighbourTile = boardState[y + i, x + j];

                        if (neighbourTile != 'e' && neighbourTile != 'w')
                        {
                            inBlock = false;
                            break;
                        }
                        else if (IsBoxStartPos(new Vector2Int(y + i, x + j)))
                        {
                            inBlock = false;
                            break;
                        }
                    }

                    if (!inBlock)
                        break;
                }

                if (inBlock)
                {
                    // Is in 3x3 block so these tiles do not count towards score

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            tilesInBlock[(y + i), (x + j)] = 0.0f;
                        }
                    }
                }
            }
        }

        // Return how many tiles are in 3x3 blocks
        float tileCount = 0.0f;

        foreach (float tile in tilesInBlock)
        {
            tileCount += tile;
        }

        return (tileCount / (Util.width * Util.height));
    }

    public bool HasNeighbour(int y, int x, char _tile)
    {
        if (y > 0 && boardState[y - 1, x] == _tile)
            return true;
        if (y < (Util.height - 1) && boardState[y + 1, x] == _tile)
            return true;
        if (x > 0 && boardState[y, x - 1] == _tile)
            return true;
        if (x < (Util.width - 1) && boardState[y, x + 1] == _tile)
            return true;

        return false;
    }

    public State DeleteObstacle(int y, int x)
    {
        if (boardState[y, x] == 'w' && HasNeighbour(y, x, 'e'))
        {
            State newState = new State(this);
            newState[y, x] = 'e';
            newState.emptyCount++;

            return newState;
        }

        return null;
    }

    public State PlaceBox(int y, int x)
    {
        if (boardState[y, x] == 'e' && !PlayerSpace(y, x))
        {
            State newState = new State(this);
            newState[y, x] = 'b';

            // Add position to box pos lists
            newState.boxPos.Add(new Vector2Int(y, x));
            newState.boxStartPos.Add(new Vector2Int(y, x));

            newState.boxCount++;
            newState.emptyCount--;

            return newState;
        }

        return null;
    }

    public State MoveAgent(Direction _dir)
    {
        Vector2Int newSpace = GetSpace(playerPos, _dir);

        if (newSpace != Util.invalidPos)
        {
            if (this[newSpace] == 'e')
            {
                State newState = new State(this);
                newState.playerPos = newSpace;

                return newState;
            }
            else if (this[newSpace] == 'b')
            {
                Vector2Int newBoxSpace = GetSpace(newSpace, _dir);

                if (newBoxSpace != Util.invalidPos && this[newBoxSpace] == 'e')
                {
                    State newState = new State(this);

                    // Can push box into empty space
                    newState.SwapSpaces(newSpace, newBoxSpace);
                    newState.SetBoxPos(newSpace, newBoxSpace);
                    newState.playerPos = newSpace;

                    return newState;
                }
            }
        }

        return null;
    }

    public State SaveLevel()
    {
        State newState = new State(this);

        if (newState.RemoveSlowBoxes())
        {
            saved = true;
            return newState;
        }
        else
        {
            //return null;
            saved = true;
            return newState;
        }
    }

    bool RemoveSlowBoxes()
    {
        // Replace boxes that have never been pushed with obstacles
        // Replace boxes that have only been pushed once with empty spaces

        for (int i = 0; i < boxCount; i++)
        {
            if (Vector2Int.Distance(boxPos[i], boxStartPos[i]) <= 1)    // If box has moved less than 2 spaces
            {
                if (boxPos[i] == boxStartPos[i])
                {
                    // Box hasn't moved at all
                    this[boxPos[i]] = 'w';
                }
                else
                {
                    // Box has moved one space
                    this[boxPos[i]] = 'e';
                }

                // Clear from box lists
                boxPos.RemoveAt(i);
                boxStartPos.RemoveAt(i);
                boxCount--;

                // If box has been removed from list, index of other boxes has decreased
                i--;
            }
        }

        // Valid action if there is at least 1 box after post processing
        return (boxCount > 0);
    }

    Vector2Int GetSpace(Vector2Int _startPos, Direction _dir)
    {
        Vector2Int space;

        switch (_dir)
        {
            case Direction.Up:
                space = new Vector2Int(_startPos.x - 1, _startPos.y);
                break;

            case Direction.Down:
                space = new Vector2Int(_startPos.x + 1, _startPos.y);
                break;

            case Direction.Left:
                space = new Vector2Int(_startPos.x, _startPos.y - 1);
                break;

            case Direction.Right:
                space = new Vector2Int(_startPos.x, _startPos.y + 1);
                break;

            default:
                return Util.invalidPos;
        }

        // Check if space is valid space
        if (space.x < 0 || space.x >= Util.height || space.y < 0 || space.y >= Util.width)
        {
            return Util.invalidPos;
        }

        return space;
    }

    void SwapSpaces(Vector2Int _space1, Vector2Int _space2)
    {
        // Swaps values of two spaces
        char space1Value = this[_space1];
        this[_space1] = this[_space2];
        this[_space2] = space1Value;
    }

    void SetBoxPos(Vector2Int _oldPos, Vector2Int _newPos)
    {
        for (int i = 0; i < boxPos.Count; i++)
        {
            if (boxPos[i] == _oldPos)
            {
                boxPos[i] = _newPos;
                break;
            }
        }
    }

    bool PlayerSpace(int y, int x)
    {
        // x and y are backwards in Vector2Int
        return (playerPos.x == y && playerPos.y == x);
    }

    public void ApplyPostProcessing()
    {
        // Final box positions into goals
        for (int i = 0; i < boxCount; i++)
        {
            boardState[boxPos[i].x, boxPos[i].y] = 'g';
        }

        // Starting box position into boxes
        for (int i = 0; i < boxCount; i++)
        {
            boardState[boxStartPos[i].x, boxStartPos[i].y] = 'b';
        }

        // Player position set
        boardState[(Util.height - 1) / 2, (Util.width - 1) / 2] = 'p';
    }
}
