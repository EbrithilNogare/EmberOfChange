using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    public BuildingController buildingController;
    public Transform player;
    public float smoothSpeed = 0.125f;
    public float verticalOffset = 2f;

    private void Start()
    {
        buildingController = FindObjectOfType<BuildingController>();
        buildingController.OnRoomDestroyed.AddListener(ShakeCamera);
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(transform.position.x, player.position.y + verticalOffset, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    void ShakeCamera()
    {
        transform.DOShakePosition(0.8f, new Vector3(0.2f, 0.2f, 0f), 10, 10f, false, true, ShakeRandomnessMode.Full);
    }
}
