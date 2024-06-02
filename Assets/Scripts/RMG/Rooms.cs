using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Rooms : MonoBehaviour
{
   public Roomtypes roomType;
    public Direction direction;


    public Tilemap platforms;
    public float xoffset;
    public float yoffset;

    public Transform upExitTransform, downExitTransform,rightExitTransform,leftExitTransform;
    public bool spawnEnemies;
}

public enum Roomtypes
{
    Default,
    R_Exit,
    U_Exit,
    L_Exit,
    D_Exit,
    LR_Exit,
    UD_Exit,
    UR_Exit,
    UL_Exit,
    DR_Exit,
    DL_Exit,

}

public enum Direction
{
    Default,
    Right,
    Up,
    Left,
    Down
}
