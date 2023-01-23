using Components;
using Leopotam.Ecs;
using Tags;
using UnityEngine;

namespace Systems
{
    public class EngineSystem : IEcsRunSystem
    {
        private readonly EcsFilter<DirectionComponent, EngineComponent> _egineFilter = null;
        
        public void Run()
        {
            foreach (var i in _egineFilter)
            {
                ref var directionComponent = ref _egineFilter.Get1(i);
                ref var engineComponent = ref _egineFilter.Get2(i);

                ref var propellers = ref engineComponent.propellersTransforms;
                ref var propellerRotSpeed = ref engineComponent.propellerRotSpeed;

                foreach (var propeller in propellers)
                {
                    propeller.Rotate(Vector3.forward, propellerRotSpeed, Space.Self);
                }
            }
        }
    }
}