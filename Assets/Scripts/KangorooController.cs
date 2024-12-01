using DG.Tweening;
using UnityEngine;

public class KangorooController : MonoBehaviour
{
    [SerializeField] BuildingController buildingController;
    [SerializeField] PlayerController playerController;

    public Animator leftKangarooAnimator;
    public Animator rightKangarooAnimator;

    // Start is called before the first frame update
    void Start()
    {
        if (buildingController == null)
            buildingController = FindObjectOfType<BuildingController>();
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        playerController.onAnimalFalling.AddListener(MoveToFallingAnimal);
    }

    void Update()
    {

    }

    public void MoveToFallingAnimal(int column)
    {
        leftKangarooAnimator.SetBool("Walking", true);
        rightKangarooAnimator.SetBool("Walking", true);

        transform.DOMoveX(column * buildingController.roomWidth, 0.5f).OnComplete(() =>
        {
            leftKangarooAnimator.SetBool("Walking", false);
            rightKangarooAnimator.SetBool("Walking", false);
        });

    }
}
