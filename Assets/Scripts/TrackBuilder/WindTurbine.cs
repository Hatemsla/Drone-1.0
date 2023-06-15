using UnityEngine;

namespace Builder
{
    public class WindTurbine : MonoBehaviour
    {
        public float rotateSpeed;
        public Rigidbody trap;

        private void FixedUpdate()
        {
            trap.MoveRotation(trap.rotation * Quaternion.Euler(Time.deltaTime * rotateSpeed, 0, 0));
        }        
    }
}