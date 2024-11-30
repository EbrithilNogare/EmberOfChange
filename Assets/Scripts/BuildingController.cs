using DG.Tweening;
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
        RightWall,
        LeftDoor,
        RightDoor,
        outsideStairs,
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
    public GameObject leftDoorPrefab;
    public GameObject rightDoorPrefab;
    public GameObject outsideStairsPrefab;

    public GameObject personPrefab;
    public GameObject firePrefab;

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
            // Fill middle part
            for (int x = 2; x < width - 2; x++)
            {
                var typeOfRoom = RoomType.Pass;


                map[x, y] = new Room
                {
                    type = typeOfRoom,
                    onFire = false,
                    withHuman = false,
                    containsFireExtinguisher = false,
                };
            }

            // Stairs
            map[0, y] = new Room
            {
                type = RoomType.outsideStairs,
                onFire = false,
                withHuman = false,
                containsFireExtinguisher = false,
            };

            map[width - 1, y] = new Room
            {
                type = RoomType.outsideStairs,
                onFire = false,
                withHuman = false,
                containsFireExtinguisher = false,
            };

            // Doors
            map[1, y] = new Room
            {
                type = RoomType.LeftWall,
                onFire = false,
                withHuman = false,
                containsFireExtinguisher = false,
            };

            map[width - 2, y] = new Room
            {
                type = RoomType.RightWall,
                onFire = false,
                withHuman = false,
                containsFireExtinguisher = false,
            };
        }

        // Randomly place stairs within inner columns for connectivity
        for (int y = 0; y < height - 1; y++)
        {
            int stairsPlaced = 0;
            while (stairsPlaced < 2)
            {
                int stairX = Random.Range(2, width - 2);
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
            int x = Random.Range(1, width - 1);
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
            int x = Random.Range(1, width - 1);
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
            RoomType.outsideStairs => outsideStairsPrefab,
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

    public void BombColumn(int column)
    {
        CollapseColumn(column, 0);
    }

    public void CollapseColumn(int column, int floor)
    {

        map[column, floor].type = RoomType.Empty;
        map[column, floor].onFire = false;
        map[column, floor].withHuman = false;

        Destroy(map[column, floor].roomGameObject);
        map[column, floor].roomGameObject = null;
        if (map[column, floor].fireExtinguisherGameObject != null)
        {
            Destroy(map[column, floor].fireExtinguisherGameObject);
            map[column, floor].fireExtinguisherGameObject = null;
        }
        OnRoomDestroyed.Invoke();

        for (int y = floor; y < height - 1; y++)
        {
            map[column, y] = map[column, y + 1];
            MoveRoomObject(column, y, column, y + 1);
        }

    }

    void MoveRoomObject(int oldX, int oldY, int newX, int newY)
    {
        if (map[newX, newY].roomGameObject != null)
        {
            map[newX, newY].roomGameObject.transform.DOMove(new Vector3(oldX * roomWidth, oldY * roomHeight, 0), 0.5f, false);
            map[newX, newY].fireExtinguisherGameObject?.transform.DOMove(new Vector3(oldX * roomWidth, oldY * roomHeight, 0), 0.5f, false);
            map[oldX, oldY] = map[newX, newY];
            map[newX, newY] = new Room();
        }
    }

    public void DebugBombRandomColumn(int column)
    {
        BombColumn(column);
    }

    public void DebugRemoveRandomRoom(int column)
    {
        CollapseColumn(column, column);
    }

}
