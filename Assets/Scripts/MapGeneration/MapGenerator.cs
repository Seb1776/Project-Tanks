using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Range(8, 250)]
    public int roomAmount;
    [Range(3, 83)]
    public int assaultRooms;
    [Range(0, 83)]
    public int conditionalRooms;
    [Range(0, 83)]
    public int deathSentenceRooms;
    public Room availableRoom;
    public List<Room> generatedRooms;
    public List<Room> canBeSpawnedNext;
    public List<Room> cantBeSpawnedNext;
    public GameObject currentRoom;
    public Transform center;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {

    }

    void GenerateMap()
    {
        currentRoom = Instantiate(availableRoom.gameObject, center.position, Quaternion.identity);
        generatedRooms.Add(currentRoom.GetComponent<Room>());

        for (int i = 0; i < roomAmount; i++)
        {
            Transform newSpawn = null;
            List<string> _directions = currentRoom.GetComponent<Room>().GetAvailableDirections();

            if (_directions.Count <= 0)
            {
                _directions.Clear();
                int currentRoomIndex = generatedRooms.IndexOf(currentRoom.GetComponent<Room>());
                int newRoomIndex = currentRoom.GetComponent<Room>().GetIndexFromAvailableRoom(currentRoomIndex - 1);

                currentRoom = generatedRooms[newRoomIndex].gameObject;
                _directions = currentRoom.GetComponent<Room>().GetAvailableDirections();
            }

            if (_directions.Count == 1)
            {
                newSpawn = currentRoom.GetComponent<Room>().GetSpawnpointFromDirection(_directions[0]);
            }

            else if (_directions.Count > 1)
            {
                int randDirIndex = Random.Range(0, _directions.Count);
                newSpawn = currentRoom.GetComponent<Room>().GetSpawnpointFromDirection(_directions[randDirIndex]);
            }

            currentRoom = Instantiate(availableRoom.gameObject, newSpawn.position, Quaternion.identity);
            generatedRooms.Add(currentRoom.GetComponent<Room>());
        }

        foreach (Room r in generatedRooms)
        {
            r.SetDoors();
        }

        for (int i = 0; i < assaultRooms; i++)
        {
            int randRoomIndex = Random.Range(1, generatedRooms.Count);

            if (generatedRooms[randRoomIndex].currentRoomType != Room.RoomType.Assault)
                generatedRooms[randRoomIndex].currentRoomType = Room.RoomType.Assault;
            
            else
                generatedRooms[NextRoom(generatedRooms.IndexOf(generatedRooms[i]) - 1, Room.RoomType.Assault)].currentRoomType = Room.RoomType.Assault;
        }

        for (int i = 0; i < conditionalRooms; i++)
        {
            int randRoomIndex = Random.Range(1, generatedRooms.Count);

            if (generatedRooms[randRoomIndex].currentRoomType != Room.RoomType.Conditional)
                generatedRooms[randRoomIndex].currentRoomType = Room.RoomType.Conditional;
            
            else
                generatedRooms[NextRoom(generatedRooms.IndexOf(generatedRooms[i]) - 1, Room.RoomType.Conditional)].currentRoomType = Room.RoomType.Conditional;
        }

        for (int i = 0; i < deathSentenceRooms; i++)
        {
            int randRoomIndex = Random.Range(1, generatedRooms.Count);

            if (generatedRooms[randRoomIndex].currentRoomType != Room.RoomType.DeathSentence)
                generatedRooms[randRoomIndex].currentRoomType = Room.RoomType.DeathSentence;
            
            else
                generatedRooms[NextRoom(generatedRooms.IndexOf(generatedRooms[i]) - 1, Room.RoomType.DeathSentence)].currentRoomType = Room.RoomType.DeathSentence;
        }

        for (int i = 0; i < generatedRooms.Count; i++)
            generatedRooms[i].GetComponent<Collider2D>().enabled = false;
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
}
