using System;
using UnityEngine;

public class DroneEngine : MonoBehaviour
{
    public float maxPower = 4f;
    public float propellerRotSpeed;
    public Transform propeller;

    public void UpdateEngine(Rigidbody rb, float throttle)
    {
        var upVector = transform.up;
        upVector.x = 0f;
        upVector.z = 0f;
        var diff = 1 - upVector.magnitude;
        var finalDiff = Physics.gravity.magnitude * diff;

        var engineForce = Vector3.zero;
        engineForce = transform.up * (rb.mass * Physics.gravity.magnitude + finalDiff + throttle * maxPower) / 4f;

        rb.AddForce(engineForce, ForceMode.Force);

        HandlePropellers();
    }

    private void HandlePropellers()
    {
        if (!propeller) return;

        propeller.Rotate(Vector3.forward, propellerRotSpeed, Space.Self);
    }
}