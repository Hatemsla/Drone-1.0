using Components;
using Leopotam.Ecs;
using UnityEngine;

namespace Systems
{
    public sealed class MovementSystem : IEcsRunSystem
    {
        private readonly EcsFilter<ModelComponent, MovableComponent, DirectionComponent> _movableFilter = null;

        private float _yaw;
        private float _finalPitch;
        private float _finalRoll;
        private float _finalYaw;
        
        public void Run()
        {
            foreach (var i in _movableFilter)
            {
                ref var modelComponent = ref _movableFilter.Get1(i);
                ref var movableComponent = ref _movableFilter.Get2(i);
                ref var directionComponent = ref _movableFilter.Get3(i);

                ref var cyclic = ref directionComponent.cyclic;
                ref var pedals = ref directionComponent.pedals;
                ref var throttle = ref directionComponent.throttle;
                
                ref var transform = ref modelComponent.modelTransform;

                ref var minMaxPitch = ref movableComponent.minMaxPitch;
                ref var minMaxRoll = ref movableComponent.minMaxRoll;
                ref var yawPower = ref movableComponent.yawPower;
                ref var lerpSpeed = ref movableComponent.lerpSpeed;
                ref var maxPower = ref movableComponent.maxPower;
                ref var rb = ref movableComponent.rb;
                
                UpdateEngine(rb, transform, throttle, maxPower);
                
                var pitch = cyclic.y * minMaxPitch;
                var roll = -cyclic.x * minMaxRoll;
                _yaw += pedals * yawPower;

                _finalPitch = Mathf.Lerp(_finalPitch, pitch, Time.deltaTime * lerpSpeed);
                _finalRoll = Mathf.Lerp(_finalRoll, roll, Time.deltaTime * lerpSpeed);
                _finalYaw = Mathf.Lerp(_finalYaw, _yaw, Time.deltaTime * lerpSpeed);

                var rot = Quaternion.Euler(_finalPitch, _finalYaw, _finalRoll);
                rb.MoveRotation(rot);
            }
        }
        
        public void UpdateEngine(Rigidbody rb, Transform transform, float throttle, float maxPower)
        {
            Vector3 upVector = transform.up;
            upVector.x = 0f;
            upVector.z = 0f;
            float diff = 1 - upVector.magnitude;
            float finalDiff = Physics.gravity.magnitude * diff;

            Vector3 engineForce;
            engineForce = transform.up * (rb.mass * Physics.gravity.magnitude + finalDiff + throttle * maxPower);

            rb.AddForce(engineForce, ForceMode.Force);
        }
    }
}