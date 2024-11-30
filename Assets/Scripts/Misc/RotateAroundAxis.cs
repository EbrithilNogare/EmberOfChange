using UnityEngine;

public class RotateAroundAxis : MonoBehaviour
{
    [SerializeField] float Speed = 45f;
    [SerializeField] Vector3 Axis = Vector3.up;
    [SerializeField] Space Space = Space.World;

    private void Awake() => Axis = Axis.normalized;

    private void Update() => transform.Rotate(Axis, Speed * Time.deltaTime, Space);
}
