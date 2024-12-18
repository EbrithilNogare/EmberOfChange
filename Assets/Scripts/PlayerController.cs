using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public BuildingController buildingController;
    public RandomEventManager randomEventManager;
    public StatsManager statsManager;
    public Vector2Int playerPosition;
    public Vector2 stepSize;
    public Animator animator;
    public SpriteRenderer spriteToFlip;

    private bool extinguishedFireThisTurn;

    [HideInInspector]
    public bool blockInputs = false;

    public UnityEvent<int> onAnimalFalling;

    void Start()
    {
        statsManager = FindObjectOfType<StatsManager>();
        if (randomEventManager == null)
            randomEventManager = FindObjectOfType<RandomEventManager>();
        transform.position = new Vector3(playerPosition.x * stepSize.x, playerPosition.y * stepSize.y, -0.1f);
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (blockInputs) return;

        Vector2Int newPosition = playerPosition;
        extinguishedFireThisTurn = false;

        if (Input.GetKeyDown(KeyCode.Space))
            PushAnimal(playerPosition.x, playerPosition.y);
        if (Input.GetKeyDown(KeyCode.P))
            EndGameAndJumpFromBuilding();


        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            newPosition = new Vector2Int(playerPosition.x, playerPosition.y + 1);
            randomEventManager.turnsUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            newPosition = new Vector2Int(playerPosition.x - 1, playerPosition.y);
            randomEventManager.turnsUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            newPosition = new Vector2Int(playerPosition.x, playerPosition.y - 1);
            randomEventManager.turnsUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            newPosition = new Vector2Int(playerPosition.x + 1, playerPosition.y);
            randomEventManager.turnsUpdate();
        }
        else
        {
            return; // no action
        }

        if (CanExtinguishFire(newPosition))
        {
            extinguishedFireThisTurn = true;
            ExtinguishFire(newPosition);
        }

        if (!extinguishedFireThisTurn && IsValidMove(newPosition))
            SetPlayerPosition(newPosition);
    }

    private void PushAnimal(int x, int y)
    {
        if (!buildingController.map[x, y].withAnimal) return;

        onAnimalFalling.Invoke(x);

        Store.Instance.savedAnimals++;

        GameObject animal = buildingController.map[x, y].innerGameObject;

        Vector3 fallTarget = new Vector3(animal.transform.position.x + .5f, 1f, animal.transform.position.z - 1.3f);
        float duration = Mathf.Max(.1f, Mathf.Sqrt(2 * (Mathf.Max(.01f, animal.transform.position.y - 1)) / 9.81f));

        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        float cameraSpeed = cameraController.smoothSpeed;
        cameraController.smoothSpeed = .5f;
        Camera.main.GetComponent<CameraController>().target = animal.transform;


        animator.SetBool("Kicking", true);
        blockInputs = true;

        DOTween.Sequence().AppendInterval(0.4f).AppendCallback(() => // wait
        {
            animator.SetBool("Kicking", false);
            blockInputs = false;
        });

        animal.GetComponent<Animator>().SetTrigger("Falling");

        Sequence fallSequence = DOTween.Sequence();
        fallSequence.Append(animal.transform.DOMove(animal.transform.position + new Vector3(0, 0, fallTarget.z), -0.1f).SetEase(Ease.Linear));
        fallSequence.Append(animal.transform.DOMove(fallTarget, duration).SetEase(Ease.OutBounce));
        fallSequence.Join(animal.transform.DORotate(new Vector3(0, 0, 90), duration, RotateMode.Fast).SetEase(Ease.Linear));
        fallSequence.OnKill(() =>
        {
            cameraController.smoothSpeed = cameraSpeed;
            cameraController.target = transform;
            Destroy(animal);

            if (Store.Instance.deadAnimals + Store.Instance.savedAnimals == buildingController.peopleCount) //end the game
                EndGameAndJumpFromBuilding();

        }); // todo maybe add some fadeout
        buildingController.map[x, y].withAnimal = false;
        buildingController.map[x, y].innerGameObject = null;
    }

    void SetPlayerPosition(Vector2Int newPosition)
    {
        if (buildingController.map[newPosition.x, newPosition.y].containsFireExtinguisher && statsManager.fireExtinguisher < statsManager.maxFireExtinguisherCount)
        {
            statsManager.FindExtinguisher();
            buildingController.map[newPosition.x, newPosition.y].containsFireExtinguisher = false;
            buildingController.map[newPosition.x, newPosition.y].innerGameObject.SetActive(false);
        }

        bool movingUpOrDown = math.abs(newPosition.y - playerPosition.y) > .1f;
        spriteToFlip.flipX = newPosition.x < playerPosition.x;

        playerPosition = newPosition;

        if (movingUpOrDown)
        {
            animator.transform.DOMove(new Vector3(playerPosition.x * stepSize.x, playerPosition.y * stepSize.y, -0.1f), 0.4f, false).SetEase(Ease.OutBack);
        }
        else
        {
            animator.SetBool("Running", true);
            animator.
            transform
                .DOMove(new Vector3(playerPosition.x * stepSize.x, playerPosition.y * stepSize.y, -0.1f), 0.4f, false).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    animator.SetBool("Running", false);
                });
        }
    }

    bool IsValidMove(Vector2Int newPosition)
    {
        if (newPosition.x < 0 || newPosition.y < 0 || newPosition.x >= buildingController.map.GetLength(0) || newPosition.y >= buildingController.map.GetLength(1))
            return false;

        var oldRoom = buildingController.map[playerPosition.x, playerPosition.y];
        var newRoom = buildingController.map[newPosition.x, newPosition.y];

        if (newRoom.type == BuildingController.RoomType.Empty)
            return false;

        if (newPosition.y > playerPosition.y && oldRoom.type != BuildingController.RoomType.Stairs && oldRoom.type != BuildingController.RoomType.outsideStairs) // stairs up
            return false;

        if (newPosition.y < playerPosition.y && newRoom.type != BuildingController.RoomType.Stairs && newRoom.type != BuildingController.RoomType.outsideStairs) // stairs down
            return false;

        if (newRoom.onFire) // fire check
            return false;

        if (newPosition.x > playerPosition.x && (oldRoom.type == BuildingController.RoomType.RightWall || newRoom.type == BuildingController.RoomType.LeftWall)) // right movement
            return false;

        if (newPosition.x < playerPosition.x && (oldRoom.type == BuildingController.RoomType.LeftWall || newRoom.type == BuildingController.RoomType.RightWall)) // left movement
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

        if (position.y > playerPosition.y && oldRoom.type != BuildingController.RoomType.Stairs && oldRoom.type != BuildingController.RoomType.outsideStairs) // stairs up
            return false;

        if (position.y < playerPosition.y && newRoom.type != BuildingController.RoomType.Stairs && newRoom.type != BuildingController.RoomType.outsideStairs) // stairs down
            return false;

        if (position.x > playerPosition.x && (oldRoom.type == BuildingController.RoomType.RightWall || newRoom.type == BuildingController.RoomType.LeftWall)) // fire to the right blocked by wall
            return false;

        if (position.x < playerPosition.x && (oldRoom.type == BuildingController.RoomType.LeftWall || newRoom.type == BuildingController.RoomType.RightWall)) // fire to the left blocked by wall
            return false;

        return true;
    }

    void ExtinguishFire(Vector2Int position)
    {
        if (statsManager.fireExtinguisher > 0)
        {
            statsManager.UseExtinguisher();
            GetComponent<AudioSource>().Play();

            animator.SetBool("FireFighting", true);
            blockInputs = true;
            spriteToFlip.flipX = position.x < playerPosition.x;
            DOTween.Sequence().AppendInterval(1.4f).AppendCallback(() => // wait
            {
                GetComponent<AudioSource>().Pause();
                animator.SetBool("FireFighting", false);
                blockInputs = false;
                buildingController.ExtinguishFire(position.x, position.y, playerPosition.x, playerPosition.y);
            });


            extinguishedFireThisTurn = true;
        }
    }

    public void EndGameAndJumpFromBuilding()
    {
        blockInputs = true;

        Store.Instance.deadAnimals = buildingController.peopleCount - Store.Instance.savedAnimals;

        Vector3 fallTarget = new Vector3(playerPosition.x * stepSize.x + .5f, 1f, -1.3f);
        float duration = Mathf.Max(.1f, Mathf.Sqrt(2 * Mathf.Max(.01f, transform.position.y - 1) / 9.81f)) * 1.2f;

        onAnimalFalling.Invoke(playerPosition.x);
        Camera.main.GetComponent<CameraController>().smoothSpeed = .5f;

        //i was here >:)
        animator.Play("Fall", 0);

        Sequence fallSequence = DOTween.Sequence();
        fallSequence.Append(transform.DOMove(new Vector3(playerPosition.x * stepSize.x, playerPosition.y * stepSize.y + 0.8f, fallTarget.z), .4f).SetEase(Ease.InCubic));
        fallSequence.Append(transform.DOMove(fallTarget, duration).SetEase(Ease.OutBounce));
        fallSequence.Join(transform.DORotate(new Vector3(0, 0, 90), duration, RotateMode.Fast).SetEase(Ease.Linear));
        fallSequence.OnKill(() =>
        {
            SceneManager.LoadScene("End screen");
        });
    }
}
