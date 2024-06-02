using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneration : MonoBehaviour
{
    [SerializeField]
    private Door door;

    [SerializeField]
    private int roomMinCount = 4;

    [SerializeField]
    private int roomMaxCount = 10;

    public int RoomsToSpawn;

    public List<Rooms> roomPresets;
    public List<GameObject> enemies;
    public List<Rooms> mapRooms;

    Direction tempDirection = Direction.Default;

    Vector3 roomOffset = Vector3.zero;


    private void Start()
    {
        GenerateMap();
        PlaceEnemies();
    }

    private void PlaceEnemies()
    {
        //Assign Randomly which room should spawn enemies
        for (int i = 0; i < mapRooms.Count; i++)
        {
            mapRooms[i].spawnEnemies = Random.value < 0.5f;
        }

        //actually spawn enemies in the selected rooms
        foreach (var room in mapRooms)
        {
            if (room.spawnEnemies)
            {
                List<Vector3> availablePositions = new List<Vector3>();
                Tilemap tilemap = room.platforms;

                foreach (var pos in tilemap.cellBounds.allPositionsWithin)
                {
                    Vector3 place = tilemap.CellToWorld(pos);
                    if (!tilemap.HasTile(pos))
                    {
                        availablePositions.Add(place);
                    }
                }

                if (availablePositions.Count == 0)
                {
                    return;
                }

                int numOfEnmiesPerRoom = Random.Range(1, 4);
                Vector3 offset = new Vector3(1f, 1f);

                for (int i = 0; i < numOfEnmiesPerRoom; i++)
                {
                    int randomIndex = Random.Range(0, availablePositions.Count);
                    GameObject enemy = Instantiate(enemies[Random.Range(0, enemies.Count)], availablePositions[randomIndex] + offset, Quaternion.identity);
                    enemy.transform.SetParent(room.transform);
                    //print("Enemy spawned in " + room + " at " + availablePositions[randomIndex] + offset);
                }
            }
        }

        Debug.Log("Hello Beautiful");
    }


    public void GenerateMap()
    {
        RoomsToSpawn = Random.Range(roomMinCount, roomMaxCount);

        Debug.Log(RoomsToSpawn + "Rooms Spawned");
        int roomindex = -1;
        GameObject tempObj = null;

        for (int i = 0; i < RoomsToSpawn; i++)
        {
            if (i == 0)
            {
                roomindex = Random.Range(0, 4);
                tempDirection = (Direction)roomindex + 1;
                tempObj = Instantiate(roomPresets[roomindex].gameObject, roomOffset, Quaternion.identity);
                tempObj.transform.SetParent(this.transform);                //Only corner rooms to be spawned


            }

            else if (i == RoomsToSpawn - 1)
            {

               
                roomindex = (int)HandleNewRoomDirection(mapRooms[mapRooms.Count - 1].roomType, tempDirection, true);

                tempObj = Instantiate(roomPresets[roomindex - 1].gameObject,
                PositionRoom(mapRooms[mapRooms.Count - 1].xoffset, mapRooms[mapRooms.Count - 1].yoffset), Quaternion.identity);
                tempObj.transform.SetParent(this.transform);

            }

            else
            {
                Direction directionforCustomRoom = tempDirection; 

                roomindex = (int)HandleNewRoomDirection(mapRooms[mapRooms.Count - 1].roomType, tempDirection, false);

                tempObj = Instantiate(roomPresets[roomindex - 1].gameObject,
                PositionRoom(mapRooms[mapRooms.Count - 1].xoffset, mapRooms[mapRooms.Count - 1].yoffset), Quaternion.identity);
                
                /*
                tempObj.transform.position = PositionCustomRooms (tempDirection, mapRooms [mapRooms.Count - 1].GetComponent<Rooms>
                */

                
                tempObj.transform.SetParent(this.transform);
                tempObj.transform.SetParent(this.transform);

                if (IsRoomPresentAtPosition())
                {
                    print("Room Deleted at: ");
                    Destroy(tempObj);
                    tempObj = null;
                    tempDirection = directionforCustomRoom;
                    RoomsToSpawn++;
                    break;
                }
            }


            mapRooms.Add(tempObj.GetComponent<Rooms>());

        }
    }

    private bool IsRoomPresentAtPosition()
    {
        foreach (var Room in mapRooms)
        
        {
            if(Room.transform.position.x == roomOffset.x && 
                Room.transform.position.y == roomOffset.y)
            {
                return true; 
            }

        }
        return false;
    }

/*
    public Vector3 PositionRoom(Direction _direction, Rooms _oldRooms, Rooms _newRooms)
    {
        switch (_direction)
        {
            case Direction.Left;
                roomOffset = new Vector3(roomOffset.x = _oldRooms.rightExitTransform.position.x - _newRooms.leftExitTransform.position.x,
                    roomOffset.y = _oldRooms.rightExitTransform.position.y - _newRooms.leftExitTransform.y, 0f);
                break;

            case Direction.Right;
                roomOffset = new Vector3(roomOffset.x = _oldRooms.leftExitTransform.position.x - _newRooms.rightExitTransform.position.x,
                            roomOffset.y = _oldRooms.leftExitTransform.position.y - _newRooms.rightExitTransform.y, 0f);
                
                break;


            case Direction.Up;
                roomOffset = new Vector3(roomOffset.x = _oldRooms.downExitTransform.position.x - _newRooms.upExitTransform.position.x,
                            roomOffset.y = _oldRooms.downExitTransform.position.y - _newRooms.upExitTransform.y);

                break;

            case Direction.Down;
                roomOffset = new Vector3(roomOffset.x = _oldRooms.upExitTransform.position.x - _newRooms.downExitTransform.position.x,
                            roomOffset.y = _oldRooms.upExitTransform.position.y - _newRooms.downExitTransform.y);

                break;


        }
    }
    */

    private Vector3 PositionRoom(float x, float y)
    {
        switch (tempDirection)
        {
            case Direction.Right:
                roomOffset = new Vector3(roomOffset.x -x, roomOffset.y, roomOffset.z); 
                break;

            case Direction.Up:
                roomOffset = new Vector3(roomOffset.x, roomOffset.y-y, roomOffset.z);
                break;

            case Direction.Left:
                roomOffset = new Vector3(roomOffset.x +x, roomOffset.y, roomOffset.z);
                break;

            case Direction.Down:
                roomOffset = new Vector3(roomOffset.x, roomOffset.y+y, roomOffset.z);
                break;
        }
        return roomOffset;
    }
    private List<Roomtypes> LeftCases(bool isLast)
    {
        List<Roomtypes> cases = new List<Roomtypes>();

        if (isLast)
        {
            cases.Add(Roomtypes.L_Exit);
        }
        else
        {
            cases.Add(Roomtypes.LR_Exit);
            cases.Add(Roomtypes.UL_Exit);
            cases.Add(Roomtypes.DL_Exit);
        }
            return cases;

    }

    private List<Roomtypes> RightCases(bool isLast)
    {
        List<Roomtypes> cases = new List<Roomtypes>();
        if (isLast)
        {
            cases.Add(Roomtypes.R_Exit);
        }
       
         else
        {
            cases.Add(Roomtypes.LR_Exit);
        cases.Add(Roomtypes.UR_Exit);
        cases.Add(Roomtypes.DR_Exit);
        }
        return cases;
    }

    private List<Roomtypes> UpCases(bool isLast)
    {
        List<Roomtypes> cases = new List<Roomtypes>();
        if (isLast)
        {
            cases.Add(Roomtypes.D_Exit);
        }
        else
        {

        cases.Add(Roomtypes.UD_Exit);
        cases.Add(Roomtypes.UR_Exit);
        cases.Add(Roomtypes.UL_Exit);

        }
        return cases;
    }

    private List<Roomtypes> DownCases(bool isLast)
    {
        List<Roomtypes> cases = new List<Roomtypes>();
        if (isLast)
        {
            cases.Add(Roomtypes.U_Exit);
        }
        else
        {

            cases.Add(Roomtypes.UD_Exit);
            cases.Add(Roomtypes.DR_Exit);
            cases.Add(Roomtypes.DL_Exit);
        }
        return cases;
    }
    public Roomtypes HandleNewRoomDirection(Roomtypes _previoustype, Direction _prevDirection, bool isLast)
    {
        Roomtypes roomToReturn = Roomtypes.Default;
        List<Roomtypes> possibleCases = new List<Roomtypes>();

        #region
        switch (_previoustype)
        {            
            case Roomtypes.R_Exit:
                possibleCases = LeftCases(isLast);
                roomToReturn = possibleCases[Random.Range(0, possibleCases.Count)];
                tempDirection = Direction.Left;
                break;
            
            case Roomtypes.U_Exit:
                possibleCases = DownCases(isLast);
                roomToReturn = possibleCases[Random.Range(0, possibleCases.Count)];
                tempDirection = Direction.Down;
                break;
            
            case Roomtypes.L_Exit:
                possibleCases = RightCases(isLast);
                roomToReturn = possibleCases[Random.Range(0, possibleCases.Count)];
                tempDirection = Direction.Right;
                break;

            case Roomtypes.D_Exit:
                possibleCases = UpCases(isLast);
                roomToReturn = possibleCases[Random.Range(0, possibleCases.Count)];
                tempDirection = Direction.Up;
                break;

            case Roomtypes.LR_Exit:
                if (_prevDirection == Direction.Left)
                {
                    possibleCases = LeftCases(isLast);
                    tempDirection = Direction.Left;
                }
                else if (_prevDirection == Direction.Right)
                {
                    possibleCases = RightCases(isLast);
                    tempDirection = Direction.Right;
                }
                roomToReturn = possibleCases[Random.Range(0, possibleCases.Count)];
                break;

            case Roomtypes.UD_Exit:
                if (_prevDirection == Direction.Up)
                {
                    possibleCases = UpCases(isLast);
                    tempDirection = Direction.Up;
                }
                else if (_prevDirection == Direction.Down)
                {
                    possibleCases = DownCases(isLast);
                    tempDirection = Direction.Down;
                  
                }

                roomToReturn = possibleCases[Random.Range(0, possibleCases.Count)];
                break;

            case Roomtypes.UR_Exit:
           
                if (_prevDirection == Direction.Up)
                {
                    possibleCases = LeftCases(isLast);
                    tempDirection = Direction.Left;
                }
                else if (_prevDirection == Direction.Right)
                {
                    possibleCases = DownCases(isLast);
                    tempDirection = Direction.Down;
                }
                roomToReturn = possibleCases[Random.Range(0, possibleCases.Count)];
                break;

            case Roomtypes.UL_Exit:
                if (_prevDirection == Direction.Up)
                {
                    possibleCases = RightCases(isLast);
                    tempDirection = Direction.Right;
                }
                else if (_prevDirection == Direction.Left)
                {
                    possibleCases = DownCases(isLast);
                    tempDirection = Direction.Down;
                }
                roomToReturn = possibleCases[Random.Range(0, possibleCases.Count)];
                break;

            case Roomtypes.DR_Exit:
                if (_prevDirection == Direction.Down)
                {
                    possibleCases = LeftCases(isLast);
                    tempDirection = Direction.Left;
                }
                else if (_prevDirection == Direction.Right)
                {
                    possibleCases = UpCases(isLast);
                    tempDirection = Direction.Up;

                }
                roomToReturn = possibleCases[Random.Range(0, possibleCases.Count)];
                break;

            case Roomtypes.DL_Exit:
                if (_prevDirection == Direction.Down)
                {
                    possibleCases = RightCases(isLast);
                    tempDirection = Direction.Right;
                }
                else if (_prevDirection == Direction.Left)
                {
                    possibleCases = UpCases(isLast);
                    tempDirection = Direction.Up;

                }
                roomToReturn = possibleCases[Random.Range(0, possibleCases.Count)];
                break;
        }

        return roomToReturn;

        #endregion //Switch Case
    }
}


