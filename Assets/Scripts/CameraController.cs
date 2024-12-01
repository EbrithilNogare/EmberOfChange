using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    public BuildingController buildingController;
    public Transform target;
    public float smoothSpeed = 0.125f;
    public float verticalOffset = 2f;
    public float initialHorizontalFOV;

    private float shakeOffsetX = 0f;

    private void Start()
    {
        buildingController = FindObjectOfType<BuildingController>();
        buildingController.OnRoomDestroyed.AddListener(ShakeCamera);
    }

    private void Update()
    {
        MaintainHorizontalFOV();
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(transform.position.x + shakeOffsetX, target.position.y + verticalOffset, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    void ShakeCamera()
    {
        transform.DOComplete();
        transform.DOShakePosition(0.8f, new Vector3(0.2f, 0f, 0f), 10, 10f, false, true, ShakeRandomnessMode.Full)
            .OnUpdate(() => shakeOffsetX = transform.position.x - transform.position.x)
            .OnComplete(() => shakeOffsetX = 0f);
    }

    void MaintainHorizontalFOV()
    {
        float newAspect = Camera.main.aspect;
        Camera.main.fieldOfView = 2f * Mathf.Atan(Mathf.Tan(initialHorizontalFOV * Mathf.Deg2Rad * 0.5f) / newAspect) * Mathf.Rad2Deg;
    }
}
