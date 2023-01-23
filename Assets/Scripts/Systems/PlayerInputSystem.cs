using Components;
using Leopotam.Ecs;
using Tags;
using UnityEditor;
using UnityEngine;

namespace Systems
{
    public sealed class PlayerInputSystem : IEcsRunSystem
    {
        private readonly EcsFilter<PlayerTag, DirectionComponent> _directionFilter = null;

        private Vector2 _cyclic;
        private float _pedals;
        private float _throttle;
        
        public void Run()
        {
            SetDirection();
            
            foreach (var i in _directionFilter)
            {
                ref var directionComponent = ref _directionFilter.Get2(i);
                ref var cyclic = ref directionComponent.cyclic;
                ref var pedals = ref directionComponent.pedals;
                ref var throttle = ref directionComponent.throttle;

                cyclic = _cyclic;
                pedals = _pedals;
                throttle = _throttle;
            }
        }

        private void SetDirection()
        {
            _cyclic.x = Input.GetAxis("Horizontal");
            _cyclic.y = Input.GetAxis("Vertical");
            _pedals = Input.GetAxis("Pedal");
            _throttle = Input.GetAxis("Throttle");
        }
    }
}