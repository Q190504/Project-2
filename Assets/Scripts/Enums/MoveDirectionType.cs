using UnityEngine;

public enum MoveDirectionType
{
    Horizontal,
    Vertical,
    Diagonal,
    FourDirection,   // Up, Down, Left, Right
    EightDirection,  // 8-way (including diagonals)
    Free             // Return normalized input
}


