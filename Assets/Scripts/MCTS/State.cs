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

public class State
{
    public char[,] boardState = new char[Util.height, Util.width];
    public bool frozen = false;
    public bool saved = false;

    Vector2Int playerPos;

    int boxCount = 0;
    int emptyCount = 1;
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
        float a = 1.0f;
        float b = 1.0f;
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

                    foreach (Vector2Int pos in boxStartPos)     // check if tile is a box start position
                    {
                        if (tilePos == pos)
                        {
                            startCount++;
                            break;
                        }
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

        return tileCount;
    }

    //float GetBoxStartPosInArea(Vector2 _areaStart, Vector2 _areaEnd)
    //{
    //    float num = 0.0f;

    //    foreach(Vector2 pos in boxStartPos)
    //    {
    //        // Figure out how to check if its in area
    //    }

    //    return num;
    //}

    public bool DeleteRandomObstacle()
    {
        Vector2Int emptySpace;
        Vector2Int obstacleSpace;

        int counter = 0;

        // Find obstacle next to empty space
        do
        {
            emptySpace = GetRandomSpace('e');
            obstacleSpace = GetRandomNeighbor(emptySpace, 'w');

            counter++;

            if (counter >= Util.impossibleCount)
                return false;
            // Can't find obstacle to delete
        }
        while (obstacleSpace == Util.invalidPos);

        // Remove obstacle from selected space
        boardState[obstacleSpace.x, obstacleSpace.y] = 'e';

        emptyCount++;
        return true;
    }

    public bool PlaceRandomBox()
    {
        int counter = 0;

        // Find random empty space and place box in space
        Vector2Int emptySpace;
        do
        {
            emptySpace = GetRandomSpace('e');

            counter++;

            if (counter >= Util.impossibleCount)
                return false;
            // Can't find space to place box

        } while (emptySpace == playerPos);  // Finds new empty space if space found is player position

        boardState[emptySpace.x, emptySpace.y] = 'b';

        // Add position to box pos lists
        boxPos.Add(emptySpace);
        boxStartPos.Add(emptySpace);

        boxCount++;
        emptyCount--;
        return true;
    }

    public bool MoveAgentRandomly()
    {
        List<Direction> possibleDirections = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        int counter = 0;

        while(true)
        {
            // Choose random direction that hasn't been checked yet
            int randomIndex = Random.Range(0, possibleDirections.Count - 1);
            Direction direction = possibleDirections[randomIndex];

            // Get space in direction
            Vector2Int newSpace = GetSpace(playerPos, direction);

            if (newSpace != Util.invalidPos)
            {
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
                    // Find where box will move
                    Vector2Int newBoxSpace = GetSpace(newSpace, direction);

                    // Check if box has empty space to be pushed into
                    if (newBoxSpace != Util.invalidPos && boardState[newBoxSpace.x, newBoxSpace.y] == 'e')
                    {
                        // Can push box into empty space
                        SwapSpaces(newSpace, newBoxSpace);
                        SetBoxPos(newSpace, newBoxSpace);
                        SwapSpaces(playerPos, newSpace);
                        playerPos = newSpace;
                        break;
                    }
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

            counter++;

            if (counter >= Util.impossibleCount)
                return false;
            // Can't find space to move player into
        }

        return true;
    }

    public void Save()
    {
        saved = true;

        // Replace boxes that have never been pushed with obstacles
        // Replace boxes that have only been pushed once with empty spaces

        for (int i = 0; i < boxCount; i++)
        {
            if (Vector2Int.Distance(boxPos[i], boxStartPos[i]) <= 1)    // If box has moved less than 2 spaces
            {
                if (boxPos[i] == boxStartPos[i])    // box hasn't moved at all
                {
                    // Set space as wall
                    boardState[boxPos[i].x, boxPos[i].y] = 'e';
                }
                else   // box moved once
                {
                    // Set spaces as empty
                    boardState[boxPos[i].x, boxPos[i].y] = 'e';
                }

                // Clear from box lists
                boxPos.RemoveAt(i);
                boxStartPos.RemoveAt(i);
                boxCount--;

                // If box has been removed from list, index of other boxes has decreased
                i--;
            }
        }
    }

    Vector2Int GetRandomSpace(char _spaceType)
    {
        Vector2Int space = new Vector2Int(0,0);

        int counter = 0;

        // Generate random positions until space type matches
        do
        {
            space = new Vector2Int(Random.Range(0, Util.height), Random.Range(0, Util.width));

            counter++;

            if (counter >= Util.impossibleCount)
            {
                Debug.Log("Stuck in GetRandomSpace loop");
                break;
            }
        }
        while (boardState[space.x, space.y] != _spaceType);
        // Finds space that is of correct type and not where player is

        return space;
    }

    Vector2Int GetRandomNeighbor(Vector2Int _pos, char _spaceType)
    {
        List<Vector2Int> validNeighbors = new List<Vector2Int>();

        // Add to list all neighbors that have matching space type
        foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
        {
            Vector2Int neighbor = GetSpace(_pos, direction);

            if (neighbor != Util.invalidPos)    // Check space returned is valid
            {
                if (boardState[neighbor.x, neighbor.y] == _spaceType)
                    validNeighbors.Add(neighbor);
            }
        }

        if (validNeighbors.Count == 0)
            // No neighboring spaces of space type
            return Util.invalidPos;
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

            if (neighbor != Util.invalidPos)
            {
                if (boardState[neighbor.x, neighbor.y] == _spaceType1 || boardState[neighbor.x, neighbor.y] == _spaceType2)
                    validNeighbors.Add(neighbor);
            }
        }

        if (validNeighbors.Count == 0)
            // No neighboring spaces of space type
            return Util.invalidPos;
        else
            // Return random neigbor from valid neighbor list
            return validNeighbors[Random.Range(0, validNeighbors.Count - 1)];
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
        char space1Value = boardState[_space1.x, _space1.y];
        boardState[_space1.x, _space1.y] = boardState[_space2.x, _space2.y];
        boardState[_space2.x, _space2.y] = space1Value;
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
    }
}
