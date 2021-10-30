using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int maxUntilRegen;
    [Range(8, 250)]
    public int roomAmount;
    [Range(3, 83)]
    public int assaultRooms;
    [Range(0, 83)]
    public int conditionalRooms;
    [Range(0, 83)]
    public int deathSentenceRooms;
    [Range(0f, 1f)]
    public float chanceOfBigRoom;
    public Room roomPrefab;
    public Room bigRoomPrefab;
    public Vector2 dirRandHeight;
    public List<DirectionRandomizer> dirRand = new List<DirectionRandomizer>();
    public List<DirectionRandomizerBig> dirRandBig = new List<DirectionRandomizerBig>();
    public List<Room> generatedRooms;
    public List<Room> canBeSpawnedNext;
    public List<Room> cantBeSpawnedNext;
    public GameObject currentRoom;
    public Transform center;

    int roomGen;
    Room roomToSpawn;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {

    }

    void GenerateMap()
    {
        currentRoom = Instantiate(roomPrefab.gameObject, center.position, Quaternion.identity);
        generatedRooms.Add(currentRoom.GetComponent<Room>());
        currentRoom.GetComponent<Room>().spawner = GameObject.FindGameObjectWithTag("SpawnSystem").GetComponent<SpawnSystem>();

        for (int i = 1; i < roomAmount; i++)
        {
            Room _roomPrefab = null;

            if (Random.value <= chanceOfBigRoom) _roomPrefab = bigRoomPrefab;
            else _roomPrefab = roomPrefab;

            Transform newSpawn = null;
            List<string> _directions = new List<string>();
            
            _directions = currentRoom.GetComponent<Room>().GetAvailableDirections(_roomPrefab.roomID);

            if (_directions.Count <= 0)
            {
                int randRoom = RandomAvailableRoom();
                currentRoom = generatedRooms[randRoom].gameObject;
                _directions = currentRoom.GetComponent<Room>().GetAvailableDirections(_roomPrefab.roomID);
                newSpawn = currentRoom.GetComponent<Room>().GetSpawnpointFromDirection(PositionFromSpawnList(_directions, _roomPrefab.roomID, currentRoom.GetComponent<Room>().roomID));
            }

            else
                newSpawn = currentRoom.GetComponent<Room>().GetSpawnpointFromDirection(PositionFromSpawnList(_directions, _roomPrefab.roomID, currentRoom.GetComponent<Room>().roomID));

            currentRoom = Instantiate(_roomPrefab.gameObject, newSpawn.position, Quaternion.identity);
            currentRoom.gameObject.name += i.ToString();
            currentRoom.GetComponent<Room>().spawner = GameObject.FindGameObjectWithTag("SpawnSystem").GetComponent<SpawnSystem>();
            generatedRooms.Add(currentRoom.GetComponent<Room>());
        }

        foreach (Room r in generatedRooms)
            r.SetDoors();
    }

    int RandomAvailableRoom()
    {
        int randRoom = Random.Range(0, generatedRooms.Count);

        if (generatedRooms[randRoom].GetAvailableDirections(generatedRooms[randRoom].roomID).Count > 0)
            return randRoom;
        
        else
            return RandomAvailableRoom();
    }

    int NextRoom(int index, Room.RoomType roomToPut)
    {   
        switch (roomToPut)
        {   
            case Room.RoomType.Assault:
                if (generatedRooms[index].currentRoomType != Room.RoomType.Assault)
                    return index;
                
                else
                    NextRoom(index - 1, roomToPut);
            break;

            case Room.RoomType.Conditional:
                if (generatedRooms[index].currentRoomType != Room.RoomType.Conditional)
                    return index;
                
                else
                    NextRoom(index - 1, roomToPut);
            break;

            case Room.RoomType.DeathSentence:
                if (generatedRooms[index].currentRoomType != Room.RoomType.DeathSentence)
                    return index;
                
                else
                    NextRoom(index - 1, roomToPut);
            break;
        }
        
        return -1;
    }

    string PositionFromSpawnList(List<string> _dir, string roomTo, string roomFrom)
    {
        int randomDirIndex = Random.Range(0, _dir.Count);
        string newDir = "nam8";
        List<string> availableGlobalDirection = new List<string>();

        if (roomTo == "smallroom" && roomFrom == "smallroom" || roomTo == "bigroom" && roomFrom == "bigroom")
        {
            if (roomGen > maxUntilRegen)
            {
                for (int i = 0; i < dirRand.Count; i++)
                    if (dirRand[i].used)
                        dirRand[i].used = false;
                    
                roomGen = 0;
            }

            for (int i = 0; i < dirRand.Count; i++)
                if (!dirRand[i].used)
                    availableGlobalDirection.Add(dirRand[i].directionTo.ToString());

            if (availableGlobalDirection.Contains(_dir[randomDirIndex]))
            {
                for (int i = 0; i < dirRand.Count; i++)
                {
                    if (dirRand[i].directionTo.ToString() == _dir[randomDirIndex])
                    {
                        newDir = dirRand[i].directionTo.ToString();
                        dirRand[i].used = true;
                    }
                }
            }

            else
            {
                int randIndexFromDirRand = Random.Range(0, dirRand.Count);
                newDir = dirRand[randIndexFromDirRand].directionTo.ToString();
                dirRand[randIndexFromDirRand].used = true;
            }
        }

        else
        {
            if (roomGen > maxUntilRegen)
            {
                for (int i = 0; i < dirRandBig.Count; i++)
                    if (dirRandBig[i].used)
                        dirRandBig[i].used = false;
                    
                roomGen = 0;
            }

            for (int i = 0; i < dirRandBig.Count; i++)
                if (!dirRandBig[i].used)
                    availableGlobalDirection.Add(dirRandBig[i].bigDirectionTo.ToString());
            
            if (availableGlobalDirection.Contains(_dir[randomDirIndex]))
            {
                for (int i = 0; i < dirRandBig.Count; i++)
                {
                    if (dirRandBig[i].bigDirectionTo.ToString() == _dir[randomDirIndex])
                    {
                        newDir = dirRandBig[i].bigDirectionTo.ToString();
                        dirRandBig[i].used = true;
                    }
                }
            }

            else
            {
                int randIndexFromDirRandBig = Random.Range(0, dirRandBig.Count);
                newDir = dirRandBig[randIndexFromDirRandBig].bigDirectionTo.ToString();
                dirRandBig[randIndexFromDirRandBig].used = true;
            }
        }

        roomGen++;
        return newDir;
    }
}

[System.Serializable]
public class DirectionRandomizer
{
    public enum Direction{Up, Down, Right, Left}
    public Direction directionTo;
    public bool used;
}

[System.Serializable]
public class DirectionRandomizerBig
{
    public enum BigDirection{UpA, UpB, DownA, DownB, RightA, RightB, LeftA, LeftB}
    public BigDirection bigDirectionTo;
    public bool used;
}
