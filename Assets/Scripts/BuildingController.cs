using UnityEngine;
using UnityEngine.Events;

public class BuildingController : MonoBehaviour
{
    public enum RoomType
    {
        Empty,
        Pass,
        Stairs,
        LeftWall,
        RightWall
    }

    [System.Serializable]
    public struct Room
    {
        public GameObject roomGameObject;
        public GameObject fireExtinguisherGameObject;
        public bool onFire;
        public bool withHuman;
        public bool containsFireExtinguisher;
        public RoomType type;
    }

    [Header("Map Settings")]
    public int width;
    public int height;
    public int fireExtinguisherCount;
    public int fireCount;
    public int peopleCount;

    [Header("Prefab References")]
    public GameObject fireExtinguisherPrefab;
    public GameObject passPrefab;
    public GameObject stairsPrefab;
    public GameObject leftWallPrefab;
    public GameObject rightWallPrefab;
    public GameObject firePrefab;
    public GameObject personPrefab;

    [Header("Room Dimensions")]
    public float roomWidth;
    public float roomHeight;

    public Room[,] map;
    public UnityEvent OnRoomDestroyed;

    void Start()
    {
        map = new Room[width, height];
        GenerateMap();
        InstantiateRooms();
    }

    void GenerateMap()
    {
        for (int y = 0; y < height; y++)
        {
            int stairsPlaced = 0;
            for (int x = 0; x < width; x++)
            {
                map[x, y] = new Room
                {
                    type = RoomType.Pass,
                    onFire = false,
                    withHuman = false,
                    containsFireExtinguisher = false,
                };
            }

            while (stairsPlaced < 2 && y != height - 1)
            {
                int stairX = Random.Range(0, width);
                if (map[stairX, y].type != RoomType.Stairs)
                {
                    map[stairX, y].type = RoomType.Stairs;
                    stairsPlaced++;
                }
            }
        }

        PlaceRandomElements(fireCount, true, false);
        PlaceRandomElements(peopleCount, false, true);
        PlaceFireExtinguishers();
    }
    void PlaceFireExtinguishers()
    {
        int placed = 0;
        while (placed < fireExtinguisherCount)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(1, height);

            if (map[x, y].type == RoomType.Pass &&
                !map[x, y].onFire &&
                !map[x, y].withHuman &&
                !map[x, y].containsFireExtinguisher)
            {
                map[x, y].containsFireExtinguisher = true;
                Vector3 position = new Vector3(x * roomWidth, y * roomHeight, 0);
                map[x, y].fireExtinguisherGameObject = Instantiate(fireExtinguisherPrefab, position, Quaternion.identity, transform);
                placed++;
            }
        }
    }

    void PlaceRandomElements(int count, bool fire, bool human)
    {
        int placed = 0;
        while (placed < count)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(1, height);
            if (map[x, y].type == RoomType.Pass && !map[x, y].onFire && !map[x, y].withHuman)
            {
                map[x, y].onFire = fire;
                map[x, y].withHuman = human;
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
                Room room = map[x, y];
                GameObject prefab = GetPrefabForRoom(room);
                if (prefab != null)
                {
                    Vector3 position = new Vector3(x * roomWidth, y * roomHeight, 0);
                    room.roomGameObject = Instantiate(prefab, position, Quaternion.identity, transform);
                    map[x, y] = room;
                }
            }
        }
    }

    GameObject GetPrefabForRoom(Room room)
    {
        if (room.onFire) return firePrefab;
        if (room.withHuman) return personPrefab;
        return room.type switch
        {
            RoomType.Pass => passPrefab,
            RoomType.Stairs => stairsPrefab,
            RoomType.LeftWall => leftWallPrefab,
            RoomType.RightWall => rightWallPrefab,
            _ => null,
        };
    }

    public void ChangeRoomToFire(int x, int y)
    {
        if (!map[x, y].onFire)
        {
            map[x, y].onFire = true;
            UpdateRoom(x, y);
        }
    }

    public void ExtinguishFire(int x, int y)
    {
        if (map[x, y].onFire && (IsAdjacentToStairs(x, y) || IsOnSameFloor(x, y)))
        {
            map[x, y].onFire = false;
            UpdateRoom(x, y);
        }
    }

    void UpdateRoom(int x, int y)
    {
        if (map[x, y].roomGameObject != null)
            Destroy(map[x, y].roomGameObject);

        GameObject prefab = GetPrefabForRoom(map[x, y]);
        if (prefab != null)
        {
            Vector3 position = new Vector3(x * roomWidth, y * roomHeight, 0);
            map[x, y].roomGameObject = Instantiate(prefab, position, Quaternion.identity, transform);
        }
    }

    bool IsAdjacentToStairs(int x, int y)
    {
        return (x > 0 && map[x - 1, y].type == RoomType.Stairs) ||
               (x < width - 1 && map[x + 1, y].type == RoomType.Stairs) ||
               (y > 0 && map[x, y - 1].type == RoomType.Stairs) ||
               (y < height - 1 && map[x, y + 1].type == RoomType.Stairs);
    }

    bool IsOnSameFloor(int x, int y)
    {
        for (int i = 0; i < width; i++)
        {
            if (map[i, y].type == RoomType.Stairs)
                return true;
        }
        return false;
    }
}
