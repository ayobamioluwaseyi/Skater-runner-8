using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PieceType
{
    // subway suff piece types
    none = -1,
    ramp = 0,
    longBlock = 1, // trains
    jump = 2, // jump over
    slide = 3, // slide under
}
public class Piece : MonoBehaviour
{
    public PieceType type;
    // variable to make enums spawn at random number
    public int visualIndex;
}
