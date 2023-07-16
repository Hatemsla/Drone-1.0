using System;
using System.Collections;
using System.Collections.Generic;
using Builder;
using UnityEngine;

public class BoostTrigger : InteractiveObject
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<DroneBuilderController>().boostsCount < 3)
            {
                StartCoroutine(BoostDrone(other.GetComponent<Rigidbody>(), other.GetComponent<DroneBuilderController>()));
            }
        }
    }

    private IEnumerator BoostDrone(Rigidbody rb, DroneBuilderController drone)
    {
        rb.velocity *= boostSpeed;
        drone.boostsCount++;
        yield return new WaitForSeconds(2);
        rb.velocity /= boostSpeed;
        drone.boostsCount--;
    }
}
