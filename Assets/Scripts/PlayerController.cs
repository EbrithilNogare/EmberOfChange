using UnityEngine;
using static BuildingController;

public class PlayerController : MonoBehaviour
{
    public BuildingController buildingController;
    public StatsManager statsManager;
    public Vector2Int playerPosition;
    public Vector2 stepSize;

    private bool extinguishedFireThisTurn;

    void Start()
    {
        statsManager = FindObjectOfType<StatsManager>();

        transform.position = new Vector3(playerPosition.x * stepSize.x, playerPosition.y * stepSize.y, 0);
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2Int newPosition = playerPosition;
        extinguishedFireThisTurn = false;

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

        if (CanExtinguishFire(newPosition))
        {
            ExtinguishFire(newPosition);
        }
        else if (!extinguishedFireThisTurn && IsValidMove(newPosition, playerPosition))
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

        if ((oldPosition - newPosition).y < 0 && oldRoom != RoomType.Stairs)
            return false;

        if ((oldPosition - newPosition).y > 0 && newRoom != RoomType.Stairs)
            return false;

        if (newRoom == RoomType.Stairs || newRoom == RoomType.Room || newRoom == RoomType.RoomWithPerson)
            return true;

        return false;
    }

    bool CanExtinguishFire(Vector2Int position)
    {
        if (statsManager.fireExtinguisher <= 0)
            return false;

        if (position.x >= 0 && position.y >= 0 && position.x < buildingController.map.GetLength(0) && position.y < buildingController.map.GetLength(1))
        {
            return buildingController.map[position.x, position.y] == RoomType.Fire;
        }

        return false;
    }

    void ExtinguishFire(Vector2Int position)
    {
        if (statsManager.fireExtinguisher > 0)
        {
            statsManager.UseExtinguisher();
            buildingController.ChangeRoomToNotFire(position.x, position.y);
            extinguishedFireThisTurn = true;
        }
    }
}
