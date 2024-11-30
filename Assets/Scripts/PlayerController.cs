using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public BuildingController buildingController;
    public StatsManager statsManager;
    public Vector2Int playerPosition;
    public Vector2 stepSize;

    private bool extinguishedFireThisTurn;

    public UnityEvent<int> onAnimalFalling;

    void Start()
    {
        statsManager = FindObjectOfType<StatsManager>();
        SetPlayerPosition(playerPosition);
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2Int newPosition = playerPosition;
        extinguishedFireThisTurn = false;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            newPosition = new Vector2Int(playerPosition.x, playerPosition.y + 1);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            newPosition = new Vector2Int(playerPosition.x - 1, playerPosition.y);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            newPosition = new Vector2Int(playerPosition.x, playerPosition.y - 1);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            newPosition = new Vector2Int(playerPosition.x + 1, playerPosition.y);
        else if (Input.GetKeyDown(KeyCode.Space))
            onAnimalFalling.Invoke(newPosition.x);

        if (CanExtinguishFire(newPosition))
            ExtinguishFire(newPosition);
        else if (!extinguishedFireThisTurn && IsValidMove(newPosition))
            SetPlayerPosition(newPosition);

    }

    void SetPlayerPosition(Vector2Int newPosition)
    {
        if (buildingController.map[newPosition.x, newPosition.y].containsFireExtinguisher && statsManager.fireExtinguisher < statsManager.maxFireExtinguisherCount)
        {
            statsManager.FindExtinguisher();
            buildingController.map[newPosition.x, newPosition.y].containsFireExtinguisher = false;
            buildingController.map[newPosition.x, newPosition.y].fireExtinguisherGameObject.SetActive(false);
        }

        playerPosition = newPosition;
        transform.DOMove(new Vector3(playerPosition.x * stepSize.x, playerPosition.y * stepSize.y, 0), 0.5f, false);
    }

    bool IsValidMove(Vector2Int newPosition)
    {
        if (newPosition.x < 0 || newPosition.y < 0 || newPosition.x >= buildingController.map.GetLength(0) || newPosition.y >= buildingController.map.GetLength(1))
            return false;

        var oldRoom = buildingController.map[playerPosition.x, playerPosition.y];
        var newRoom = buildingController.map[newPosition.x, newPosition.y];

        if (newRoom.type == BuildingController.RoomType.Empty)
            return false;

        if (newPosition.y > playerPosition.y && oldRoom.type != BuildingController.RoomType.Stairs) // stairs up
            return false;

        if (newPosition.y < playerPosition.y && newRoom.type != BuildingController.RoomType.Stairs) // stairs down
            return false;

        if (newRoom.onFire) // fire check
            return false;

        return true;
    }

    bool CanExtinguishFire(Vector2Int position)
    {
        if (position.x < 0 || position.y < 0 || position.x >= buildingController.map.GetLength(0) || position.y >= buildingController.map.GetLength(1))
            return false;

        if (statsManager.fireExtinguisher <= 0)
            return false;

        var oldRoom = buildingController.map[playerPosition.x, playerPosition.y];
        var newRoom = buildingController.map[position.x, position.y];

        if (newRoom.type == BuildingController.RoomType.Empty)
            return false;

        if (!newRoom.onFire)
            return false;

        if (position.y > playerPosition.y && oldRoom.type != BuildingController.RoomType.Stairs) // stairs up
            return false;

        if (position.y < playerPosition.y && newRoom.type != BuildingController.RoomType.Stairs) // stairs down
            return false;

        return true;
    }

    void ExtinguishFire(Vector2Int position)
    {
        if (statsManager.fireExtinguisher > 0)
        {
            statsManager.UseExtinguisher();
            buildingController.ExtinguishFire(position.x, position.y);
            extinguishedFireThisTurn = true;
        }
    }
}
