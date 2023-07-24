using UnityEngine;

namespace Builder
{
    public class FreezingBall : InteractiveObject
    {
        private void OnCollisionEnter(Collision other)
        {
            if(!isActive)
                return;
            
            var player = other.gameObject.GetComponent<DroneBuilderController>();
            if (player)
            {
                StartCoroutine(player.IsFreezing());
            }
        }

        public override void SetActive(bool active)
        {
            isActive = active;
        }
    }
}