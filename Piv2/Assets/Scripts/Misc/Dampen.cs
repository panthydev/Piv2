using System;
using UnityEngine;

public class Dampen : MonoBehaviour
{
    public float dampening;
    private void DampenCamera()
    {
        // transform.localPosition = Vector3.Lerp(
        //     transform.localPosition,
        //     Vector3.zero,
        //     Time.deltaTime * 50f
        // );

       // transform.localEulerAngles = Vector3.zero;
        //Vector3.Lerp(transform.localEulerAngles, Vector3.zero, Time.deltaTime * 1);

    }

    private void LateUpdate()
    {
        transform.localRotation =
            Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * dampening);
    }

    private void Update()
    {
        DampenCamera();
    }
}
