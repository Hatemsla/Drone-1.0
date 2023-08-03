using System.Collections;
using Builder;
using UnityEngine;

public class BoostTrigger : InteractiveObject
{
    private void Start()
    {
        boostSpeed = 2f;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<DroneController>();
        if (player)
        {
            if (player.boostsCount < 3)
            {
                StartCoroutine(BoostDrone(other.GetComponent<Rigidbody>(), player));
            }
        }
    }

    private IEnumerator BoostDrone(Rigidbody rb, DroneController drone)
    {
        rb.velocity *= boostSpeed;
        drone.boostsCount++;
        yield return new WaitForSeconds(2);
        rb.velocity /= boostSpeed;
        drone.boostsCount--;
    }

    public override void SetActive(bool active)
    {
        
    }

    public override void SetColorIndex(int active)
    {
    }
}
