using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public enum RoomType
    {
        Empty,
        Room,
        Stairs,
        Fire,
        RoomWithPerson
    }

    [Header("Map Settings")]
    public int width;
    public int height;
    public int fireCount;
    public int peopleCount;

    [Header("Prefab References")]
    public GameObject roomPrefab;
    public GameObject stairsPrefab;
    public GameObject roomOnFirePrefab;
    public GameObject roomWithPersonPrefab;

    [Header("Room Dimensions")]
    public float roomWidth = 1f;  // Width of each room
    public float roomHeight = 1f; // Height of each room

    public RoomType[,] map;
    private GameObject[,] roomObjects;

    void Start()
    {
        roomObjects = new GameObject[width, height];
        GenerateMap();
        InstantiateRooms();
    }

    void GenerateMap()
    {
        map = new RoomType[width, height];

        for (int y = 0; y < height; y++)
        {
            int stairsPlaced = 0;
            for (int x = 0; x < width; x++)
            {
                map[x, y] = RoomType.Room;
            }

            while (stairsPlaced < 2 && y != height - 1)
            {
                int stairX = Random.Range(0, width);
                if (map[stairX, y] != RoomType.Stairs)
                {
                    map[stairX, y] = RoomType.Stairs;
                    stairsPlaced++;
                }
            }
        }

        PlaceRandomElements(RoomType.Fire, fireCount);
        PlaceRandomElements(RoomType.RoomWithPerson, peopleCount);
    }

    void PlaceRandomElements(RoomType type, int count)
    {
        int placed = 0;
        while (placed < count)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(1, height);
            if (map[x, y] == RoomType.Room)
            {
                map[x, y] = type;
                placed++;
            }
        }
    }

    void InstantiateRooms()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                RoomType roomType = map[x, y];
                GameObject prefab = GetPrefabForRoomType(roomType);
                if (prefab != null)
                {
                    Vector3 position = new Vector3(x * roomWidth, y * roomHeight, 0);
                    roomObjects[x, y] = Instantiate(prefab, position, Quaternion.identity, transform);
                }
            }
        }
    }

    GameObject GetPrefabForRoomType(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Room:
                return roomPrefab;
            case RoomType.Stairs:
                return stairsPrefab;
            case RoomType.Fire:
                return roomOnFirePrefab;
            case RoomType.RoomWithPerson:
                return roomWithPersonPrefab;
            default:
                return null;
        }
    }

    public void CollapseColumn(int column, int floor)
    {
        for (int y = floor; y < height - 1; y++)
        {
            map[column, y] = map[column, y + 1];
            MoveRoomObject(column, y, column, y + 1);
        }
        map[column, height - 1] = RoomType.Empty;
        Destroy(roomObjects[column, height - 1]);
        roomObjects[column, height - 1] = null;
    }

    public void BombColumn(int column)
    {
        for (int y = 0; y < height - 1; y++)
        {
            map[column, y] = map[column, y + 1];
            MoveRoomObject(column, y, column, y + 1);
        }
        map[column, height - 1] = RoomType.Empty;
        Destroy(roomObjects[column, height - 1]);
        roomObjects[column, height - 1] = null;
    }

    void MoveRoomObject(int oldX, int oldY, int newX, int newY)
    {
        if (roomObjects[oldX, oldY] != null)
        {
            roomObjects[newX, newY] = roomObjects[oldX, oldY];
            roomObjects[oldX, oldY] = null;
            roomObjects[newX, newY].transform.position = new Vector3(newX * roomWidth, newY * roomHeight, 0);
        }
    }

    public void ChangeRoomToFire(int x, int y)
    {
        if (map[x, y] != RoomType.Fire)
        {
            map[x, y] = RoomType.Fire;
            Destroy(roomObjects[x, y]);
            roomObjects[x, y] = Instantiate(roomOnFirePrefab, new Vector3(x * roomWidth, y * roomHeight, 0), Quaternion.identity, transform);
        }
    }

    public void ChangeRoomToNotFire(int x, int y)
    {
        if (map[x, y] == RoomType.Fire)
        {
            map[x, y] = RoomType.Room;
            Destroy(roomObjects[x, y]);
            roomObjects[x, y] = Instantiate(roomPrefab, new Vector3(x * roomWidth, y * roomHeight, 0), Quaternion.identity, transform);
        }
    }

    public void RemoveRoom(int x, int y)
    {
        if (roomObjects[x, y] != null)
        {
            Destroy(roomObjects[x, y]);
            roomObjects[x, y] = null;
            map[x, y] = RoomType.Empty;
        }
    }

    public void PrintMap()
    {
        for (int y = height - 1; y >= 0; y--)
        {
            string row = "";
            for (int x = 0; x < width; x++)
            {
                row += map[x, y] + " ";
            }
            Debug.Log(row);
        }
    }
}
