using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomType {Normal, Assault, Conditional, DeathSentence}
    public RoomType currentRoomType;
    public bool lockedRoom;
    public LayerMask whatIsRoom;
    public NextRoomSpawnPoint[] spawnPoints;
    public RoomContents[] possibleContents;
    public Interactable assaultStarter;
    public Transform[] spawnGroups;
    public List<GameObject> usedDoors = new List<GameObject>();

    MapGenerator map;

    void Start()
    {
        map = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<MapGenerator>();

        SetRoomSetup();
    }

    void Update()
    {
        HandleRoom();
    }

    void HandleRoom()
    {
        foreach (GameObject g in usedDoors)
            g.SetActive(lockedRoom);
    }

    public void SetRoomSetup()
    {
        if (currentRoomType != RoomType.Normal)
            assaultStarter.gameObject.SetActive(true);
    }

    public void CloseDoors()
    {
        lockedRoom = true;
    }

    public Transform GetSpawnpointFromDirection(string dir)
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (dir == spawnPoints[i].spawnDirection.ToString())
            {
                return spawnPoints[i].position;
            }
        }

        return null;
    }

    public int GetIndexFromAvailableRoom(int previousRoomIndex)
    {
        List<string> tmpDir = map.generatedRooms[previousRoomIndex].GetAvailableDirections();
        int newIndex = 0;

        if (tmpDir.Count > 0)
            newIndex = previousRoomIndex;
        
        else
            newIndex = map.generatedRooms[previousRoomIndex].GetIndexFromAvailableRoom(previousRoomIndex - 1);
            
        return newIndex;
    }

    public List<string> GetAvailableDirections()
    {
        List<string> directions = new List<string>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + spawnPoints[i].offset.x, transform.position.y + spawnPoints[i].offset.y), spawnPoints[i].GetDirection(spawnPoints[i].spawnDirection.ToString()), spawnPoints[i].rayLength, whatIsRoom);

            if (!hit)
                if (!directions.Contains(spawnPoints[i].spawnDirection.ToString()))
                    directions.Add(spawnPoints[i].spawnDirection.ToString());
        }

        return directions;
    }

    public void SetDoors()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + spawnPoints[i].offset.x, transform.position.y + spawnPoints[i].offset.y), spawnPoints[i].GetDirection(spawnPoints[i].spawnDirection.ToString()), spawnPoints[i].rayLength, whatIsRoom);

            spawnPoints[i].door.SetActive(!hit);
            spawnPoints[i].hallway.SetActive(hit);

            if (hit)
                usedDoors.Add(spawnPoints[i].door);
        }
    }

    void OnDrawGizmosSelected() 
    {
        if (spawnPoints.Length > 0)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(new Vector2(transform.position.x + spawnPoints[i].offset.x, transform.position.y + spawnPoints[i].offset.y), spawnPoints[i].GetDirection(spawnPoints[i].spawnDirection.ToString()) * spawnPoints[i].rayLength);
            }
        }
    }
}

[System.Serializable]
public class NextRoomSpawnPoint
{
    public Transform position;
    public Vector2 offset;
    public enum Direction {Up, Down, Right, Left}
    public GameObject door;
    public GameObject hallway;
    public Direction spawnDirection;
    public float rayLength;

    public Vector2 GetDirection(string dir)
    {
        switch (dir)
        {
            case "Up":
                return Vector2.up;
            
            case "Down":
                return Vector2.down;
            
            case "Right":
                return Vector2.right;
            
            case "Left":
                return Vector2.left;
        }

        return Vector2.zero;
    }
}
