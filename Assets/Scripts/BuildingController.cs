using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BuildingController : MonoBehaviour
{
    public enum RoomType
    {
        Empty,
        Pass,
        PassWithDoors,
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
        public GameObject innerGameObject;
        public bool onFire;
        public bool withAnimal;
        public bool containsFireExtinguisher;
        public RoomType type;
    }

    [Header("Map Settings")]
    public int width;
    public int height;
    public int fireExtinguisherCount;
    public int fireCount;
    public int peopleCount;

    [Header("Prefab Room References")]

    public GameObject passPrefab;
    public GameObject passWithDoorsPrefab;
    public GameObject stairsPrefab;
    public GameObject leftWallPrefab;
    public GameObject rightWallPrefab;
    public GameObject leftDoorPrefab;
    public GameObject rightDoorPrefab;
    public GameObject outsideStairsPrefab;

    [Header("Prefab Inner References")]
    public GameObject fireExtinguisherPrefab;
    public GameObject firePrefab;
    public GameObject[] animalsPrefab;

    [Header("Room Dimensions")]
    public float roomWidth;
    public float roomHeight;

    public Room[,] map;
    public UnityEvent OnRoomDestroyed;
    public PlayerController player;

    public RandomEventManager randomEventManager;

    void Start()
    {
        map = new Room[width, height];
        GenerateMap();
        InstantiateRooms();
        if (randomEventManager == null)
            randomEventManager = FindObjectOfType<RandomEventManager>();
        randomEventManager.OnFireSpawn.AddListener(ChangeRoomToFire);
        randomEventManager.OnRoomDestroyed.AddListener(CollapseColumn);
    }

    private RoomType[] emptyRooms = new RoomType[] { RoomType.Pass, RoomType.LeftWall, RoomType.RightWall, RoomType.LeftDoor, RoomType.RightDoor };

    void GenerateMap()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 2; x < width - 2; x++)
            {
                map[x, y] = new Room { type = Random.Range(0, 4) == 0 ? RoomType.PassWithDoors : RoomType.Pass };
            }

            int randomX = Random.Range(2, width - 3);
            map[randomX, y] = new Room { type = Random.Range(0, 1) == 0 ? RoomType.LeftWall : RoomType.RightWall };


            map[0, y] = new Room { type = RoomType.outsideStairs };
            map[width - 1, y] = new Room { type = RoomType.outsideStairs };
            map[1, y] = new Room { type = RoomType.LeftDoor };
            map[width - 2, y] = new Room { type = RoomType.RightDoor };
        }

        for (int y = 0; y < height - 1; y++)
        {
            int stairX = Random.Range(2, width - 3);
            map[stairX, y].type = RoomType.Stairs;
        }

        PlaceRandomElements(fireCount - 1, true, false, false);
        PlaceRandomElements(peopleCount - 1, false, true, false);
        PlaceRandomElements(fireExtinguisherCount - 1, false, false, true);

        PlaceTutorialSetup();
    }

    void PlaceTutorialSetup()
    {
        map[2, 0].type = RoomType.LeftWall;
        map[3, 0].type = RoomType.RightWall;
        map[4, 0].type = RoomType.Pass;
        map[5, 0].type = RoomType.Pass;
        map[6, 0].type = RoomType.Stairs;
        map[7, 0].type = RoomType.RightWall;

        map[2, 1].type = RoomType.Stairs;
        map[3, 1].type = RoomType.Pass; map[3, 1].withAnimal = true;
        map[4, 1].type = RoomType.PassWithDoors; map[4, 1].onFire = true;
        map[5, 1].type = RoomType.Pass; map[5, 1].containsFireExtinguisher = true;
        map[6, 1].type = RoomType.RightWall;
    }

    void PlaceRandomElements(int count, bool fire, bool animal, bool containsFireExtinguisher)
    {
        int placed = 0;
        int loopCount = 0;
        while (placed < count)
        {
            if (loopCount++ > 10000) throw new System.Exception("Infinite Loop");

            int x = Random.Range(1, width - 1);
            int y = Random.Range(2, height);
            if (emptyRooms.Contains(map[x, y].type) && !map[x, y].onFire && !map[x, y].withAnimal)
            {
                map[x, y].onFire = fire;
                map[x, y].withAnimal = animal;
                map[x, y].containsFireExtinguisher = containsFireExtinguisher;
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
                    map[x, y].roomGameObject = Instantiate(prefab, position, Quaternion.identity, transform);
                }

                if (room.onFire)
                {
                    Vector3 position = new Vector3(x * roomWidth, y * roomHeight, 0);
                    map[x, y].innerGameObject = Instantiate(firePrefab, position, Quaternion.identity, transform);
                }

                if (room.containsFireExtinguisher)
                {
                    Vector3 position = new Vector3(x * roomWidth, y * roomHeight, 0);
                    map[x, y].innerGameObject = Instantiate(fireExtinguisherPrefab, position, Quaternion.identity, transform);
                }

                if (room.withAnimal)
                {
                    int randomAnimalIndex = Random.Range(0, animalsPrefab.Length);
                    Vector3 position = new Vector3(x * roomWidth, y * roomHeight, 0);
                    map[x, y].innerGameObject = Instantiate(animalsPrefab[randomAnimalIndex], position, Quaternion.identity, transform);
                }
            }
        }
    }

    GameObject GetPrefabForRoom(Room room)
    {
        return room.type switch
        {
            RoomType.Pass => passPrefab,
            RoomType.Stairs => stairsPrefab,
            RoomType.LeftWall => leftWallPrefab,
            RoomType.RightWall => rightWallPrefab,
            RoomType.LeftDoor => leftDoorPrefab,
            RoomType.RightDoor => rightDoorPrefab,
            RoomType.outsideStairs => outsideStairsPrefab,
            RoomType.PassWithDoors => passWithDoorsPrefab,
            _ => null,
        };
    }

    public void ChangeRoomToFire(int x, int y)
    {
        if (map[x, y].onFire) return;

        if (map[x, y].withAnimal)
        {
            map[x, y].withAnimal = false;

            // todo kill animal visually

            Store.Instance.deadAnimals++;
            if (Store.Instance.deadAnimals + Store.Instance.savedAnimals == peopleCount) //end the game
                player.EndGameAndJumpFromBuilding();
        }

        if (map[x, y].innerGameObject != null)
        {
            Destroy(map[x, y].innerGameObject);
            map[x, y].innerGameObject = null;
        }

        map[x, y].onFire = true;

        Vector3 position = new Vector3(x * roomWidth, y * roomHeight, 0);
        map[x, y].innerGameObject = Instantiate(firePrefab, position, Quaternion.identity, transform);
    }

    public void ExtinguishFire(int roomX, int roomY, int playerX, int playerY)
    {
        if (map[roomX, roomY].onFire && (CheckFireOnReach(roomX, roomY, playerX, playerY)))
        {
            map[roomX, roomY].onFire = false;

            if (map[roomX, roomY].innerGameObject != null)
            {
                Destroy(map[roomX, roomY].innerGameObject);
                fireExtinguisherCount--;
            }
        }
    }

    bool CheckFireOnReach(int roomX, int roomY, int playerX, int playerY)
    {
        bool isPlayerDirectlyLeft = playerX == roomX - 1 && playerY == roomY;
        bool isPlayerDirectlyRight = playerX == roomX + 1 && playerY == roomY;
        bool isPlayerBelowFireOnStairs = playerX == roomX && playerY + 1 == roomY && map[playerX, playerY].type == RoomType.Stairs;

        return isPlayerDirectlyLeft || isPlayerDirectlyRight || isPlayerBelowFireOnStairs;
    }

    public void BombColumn(int column, int floor)
    {
        CollapseColumn(column, floor);
    }

    public void CollapseColumn(int column, int floor)
    {
        if (map[column, floor].withAnimal && Store.Instance.deadAnimals + Store.Instance.savedAnimals == peopleCount) // end the game
        {
            player.EndGameAndJumpFromBuilding();
            return;
        }

        map[column, floor].type = RoomType.Empty;
        map[column, floor].onFire = false;
        map[column, floor].withAnimal = false;

        Destroy(map[column, floor].roomGameObject);
        map[column, floor].roomGameObject = null;
        if (map[column, floor].innerGameObject != null)
        {
            Destroy(map[column, floor].innerGameObject);
            map[column, floor].innerGameObject = null;
        }
        OnRoomDestroyed.Invoke();

        for (int y = floor; y < height - 1; y++)
        {
            map[column, y] = map[column, y + 1];
            MoveRoomObject(column, y, column, y + 1);
        }

        if (player.playerPosition.x == column && player.playerPosition.y >= floor)
        {
            player.blockInputs = true;
            player.playerPosition.y--;
            player.transform
                .DOMove(new Vector3(player.playerPosition.x * roomWidth, player.playerPosition.y * roomHeight, 0), 0.5f, false).SetEase(Ease.InSine)
                .OnComplete(() => player.blockInputs = false);
        }
    }

    void MoveRoomObject(int oldX, int oldY, int newX, int newY)
    {
        if (map[newX, newY].roomGameObject != null)
            map[newX, newY].roomGameObject.transform.DOMove(new Vector3(oldX * roomWidth, oldY * roomHeight, 0), 0.5f, false).SetEase(Ease.InSine);
        if (map[newX, newY].innerGameObject != null)
            map[newX, newY].innerGameObject.transform.DOMove(new Vector3(oldX * roomWidth, oldY * roomHeight, 0), 0.5f, false).SetEase(Ease.InSine);
        map[oldX, oldY] = map[newX, newY];
        map[newX, newY] = new Room();
    }

    public void DebugBombRandomColumn(int column)
    {
        BombColumn(column, 0);
    }

    public void DebugRemoveRandomRoom(int column)
    {
        CollapseColumn(column, column);
    }

}
