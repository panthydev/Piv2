using UnityEngine;

public class Dampen : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float damping = 10f;

    private Quaternion previousHeadRotation;
    private Quaternion smoothedRotation;

    private void Start()
    {
        previousHeadRotation = head.rotation;
        smoothedRotation = Quaternion.identity;
    }

    private void LateUpdate()
    {
        Quaternion headDelta = head.rotation * Quaternion.Inverse(previousHeadRotation);

        Quaternion compensation = Quaternion.Inverse(headDelta);

        smoothedRotation = Quaternion.Slerp(
            smoothedRotation,
            compensation,
            Time.deltaTime * damping
        );

        transform.rotation = smoothedRotation * transform.rotation;

        previousHeadRotation = head.rotation;
    }
}