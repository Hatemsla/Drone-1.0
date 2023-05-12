using System;
using System.Collections;
using System.Collections.Generic;
using Builder;
using UnityEngine;

public class BoostTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<DroneBuilderController>().boostsCount < 3)
            {
                StartCoroutine(BoostDrone(other.GetComponent<Rigidbody>()));
            }
        }
    }

    private IEnumerator BoostDrone(Rigidbody rb)
    {
        rb.velocity *= 2;
        yield return new WaitForSeconds(2);
        rb.velocity /= 2;
    }
}
