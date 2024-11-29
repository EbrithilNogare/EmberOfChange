using UnityEngine;
using static BuildingController;

public class PlayerController : MonoBehaviour
{
    public BuildingController buildingController;
    public int fireExtinguishers;
    public Vector2Int playerPosition;
    public Vector2 stepSize;
    private bool isFightingFire;

    void Start()
    {
        transform.position = new Vector3(playerPosition.x * stepSize.x, playerPosition.y * stepSize.y, 0);
    }

    void Update()
    {
        HandleMovement();
        HandleFireExtinguisherUsage();
    }

    void HandleMovement()
    {
        if (isFightingFire)
            return;

        Vector2Int newPosition = playerPosition;

        if (Input.GetKeyDown(KeyCode.W))
        {
            newPosition = new Vector2Int(playerPosition.x, playerPosition.y + 1);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            newPosition = new Vector2Int(playerPosition.x - 1, playerPosition.y);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            newPosition = new Vector2Int(playerPosition.x, playerPosition.y - 1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            newPosition = new Vector2Int(playerPosition.x + 1, playerPosition.y);
        }

        if (IsValidMove(newPosition, playerPosition))
        {
            playerPosition = newPosition;
            transform.position = new Vector3(playerPosition.x * stepSize.x, playerPosition.y * stepSize.y, 0);
        }
    }

    bool IsValidMove(Vector2Int newPosition, Vector2Int oldPosition)
    {
        if (newPosition.x < 0 || newPosition.y < 0 || newPosition.x >= buildingController.map.GetLength(0) || newPosition.y >= buildingController.map.GetLength(1))
            return false;

        RoomType oldRoom = buildingController.map[oldPosition.x, oldPosition.y];
        RoomType newRoom = buildingController.map[newPosition.x, newPosition.y];

        if ((oldPosition - newPosition).y < 0)
            return oldRoom == RoomType.Stairs;

        if ((oldPosition - newPosition).y > 0)
            return newRoom == RoomType.Stairs;

        if (newRoom == RoomType.Stairs || newRoom == RoomType.Room || newRoom == RoomType.RoomWithPerson)
            return true;

        return false;
    }

    void HandleFireExtinguisherUsage()
    {
        if (fireExtinguishers <= 0)
            return;

        RoomType currentRoomType = buildingController.map[playerPosition.x, playerPosition.y];

        if (currentRoomType == RoomType.Fire && !isFightingFire)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isFightingFire = true;
                fireExtinguishers--;
                buildingController.map[playerPosition.x, playerPosition.y] = RoomType.Empty;
            }
        }
        else if (currentRoomType != RoomType.Fire && isFightingFire)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isFightingFire = false;
            }
        }
    }
}