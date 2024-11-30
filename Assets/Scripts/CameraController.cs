using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public float verticalOffset = 2f;

    void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(transform.position.x, player.position.y + verticalOffset, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
