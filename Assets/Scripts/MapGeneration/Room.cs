using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
    public enum RoomType {Normal, Assault, Conditional, DeathSentence}
    public RoomType currentRoomType;
    public RoomLimits thisLimits;
    public string roomID;
    public bool lockedRoom;
    public LayerMask whatIsRoom;
    public NextRoomSpawnPoint[] spawnPoints;
    public RoomContents[] possibleContents;
    public Interactable assaultStarter;
    public Transform[] enemySpawns;
    public List<GameObject> usedDoors = new List<GameObject>();

    public SpawnSystem spawner;
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
                return spawnPoints[i].spawnPos;
        }

        return null;
    }

    public List<string> GetAvailableDirections(string spawnFor)
    {
        List<string> directions = new List<string>();

        if (spawnPoints.Length > 0)
        {
            if (spawnFor == "smallroom" && roomID == "smallroom" || spawnFor == "bigroom" && roomID == "bigroom")
            {
                for (int i = 0; i < 4; i++)
                {
                    Collider2D[] otherRoom = Physics2D.OverlapBoxAll(new Vector2(
                        spawnPoints[i].spawnPos.position.x + spawnPoints[i].offset.x, spawnPoints[i].spawnPos.position.y + spawnPoints[i].offset.y
                    ), new Vector2(spawnPoints[i].overlapSpace.x, spawnPoints[i].overlapSpace.y), 0f, whatIsRoom);

                    if (otherRoom.Length <= 0)
                    {
                        if (!directions.Contains(spawnPoints[i].spawnDirection.ToString()))
                        {
                            directions.Add(spawnPoints[i].spawnDirection.ToString());
                        }
                    }
                }
            }

            else if (spawnFor == "smallroom" && roomID == "bigroom" || spawnFor == "bigroom" && roomID == "smallroom")
            {
                for (int i = 4; i < 12; i++)
                {
                    Collider2D[] otherRoom = Physics2D.OverlapBoxAll(new Vector2(
                        spawnPoints[i].spawnPos.position.x + spawnPoints[i].offset.x, spawnPoints[i].spawnPos.position.y + spawnPoints[i].offset.y
                    ), new Vector2(spawnPoints[i].overlapSpace.x, spawnPoints[i].overlapSpace.y), 0f, whatIsRoom);

                    if (otherRoom.Length <= 0)
                    {
                        if (!directions.Contains(spawnPoints[i].spawnDirection.ToString()))
                        {
                            directions.Add(spawnPoints[i].spawnDirection.ToString());
                        }
                    }
                }
            }
        }

        return directions;
    }

    public void SetDoors()
    {   
        if (roomID == "smallroom")
        {        
            for (int i = 0; i < 4; i++)
            {
                Collider2D[] otherRoom = Physics2D.OverlapBoxAll(new Vector2(
                    spawnPoints[i].spawnPos.position.x + spawnPoints[i].offset.x, spawnPoints[i].spawnPos.position.y + spawnPoints[i].offset.y
                ), new Vector2(spawnPoints[i].overlapSpace.x, spawnPoints[i].overlapSpace.y), 0f, whatIsRoom);

                bool hit = otherRoom.Length > 0;

                spawnPoints[i].door.SetActive(!hit);
                spawnPoints[i].hallway.SetActive(hit);

                if (hit)
                    if (!usedDoors.Contains(spawnPoints[i].door))
                        usedDoors.Add(spawnPoints[i].door);
            }
        }

        else
        {
            for (int i = 4; i < 12; i++)
            {
                Collider2D[] otherRoom = Physics2D.OverlapBoxAll(new Vector2(
                    spawnPoints[i].spawnPos.position.x + spawnPoints[i].offset.x, spawnPoints[i].spawnPos.position.y + spawnPoints[i].offset.y
                ), new Vector2(spawnPoints[i].overlapSpace.x, spawnPoints[i].overlapSpace.y), 0f, whatIsRoom);

                bool hit = otherRoom.Length > 0;

                spawnPoints[i].door.SetActive(!hit);
                spawnPoints[i].hallway.SetActive(hit);

                if (hit)
                    if (!usedDoors.Contains(spawnPoints[i].door))
                        usedDoors.Add(spawnPoints[i].door);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //spawner.UpdateMaxLimits(thisLimits, enemySpawns.ToList(), this);
        }
    }

    void OnDrawGizmosSelected() 
    {
        if (spawnPoints.Length > 0)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {   
                if (spawnPoints[i].showGizmos)
                {
                    Gizmos.color = Color.magenta;

                    Gizmos.DrawWireCube(new Vector2(
                        spawnPoints[i].spawnPos.position.x + spawnPoints[i].offset.x, spawnPoints[i].spawnPos.position.y + spawnPoints[i].offset.y
                    ), new Vector2(spawnPoints[i].overlapSpace.x, spawnPoints[i].overlapSpace.y));
                }
            }
        }
    }
}

[System.Serializable]
public class NextRoomSpawnPoint
{
    public Transform spawnPos;
    public Vector2 offset;
    public Vector2 overlapSpace;
    public bool showGizmos;

    public enum Direction 
    {Up, Down, Right, Left,
     UpA, UpB, DownA, DownB, RightA, RightB, LeftA, LeftB}

    public Direction spawnDirection;
    public GameObject door;
    public GameObject hallway;
    public float rayLength;

    public Vector2 GetDirection(string dir)
    {
        switch (dir)
        {
            case "Up": case "UpA": case "UpB":
                return Vector2.up;
            
            case "Down": case "DownA": case "DownB":
                return Vector2.down;
            
            case "Right": case "RightA": case "RightB":
                return Vector2.right;
            
            case "Left": case "LeftA": case "LeftB":
                return Vector2.left;
        }

        return Vector2.zero;
    }
}
