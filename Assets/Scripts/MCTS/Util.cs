using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static float C = Mathf.Sqrt(2);

    public static int height = 5;
    public static int width = 5;
    public static int numSpaces = height * width;

    public static int borderedHeight = height + 2;
    public static int borderedWidth = width + 2;
    // Bordered measurements include outer wall of obstacles

    // invalidPos returned whenever a position is outside of the range of the board
    public static Vector2Int invalidPos = new Vector2Int(-1, -1);

}
