using System;
using Systems;
using Leopotam.Ecs;
using UnityEngine;
using Voody.UniLeo;

namespace MonoBehaviours
{
    public class EcsGameStartup : MonoBehaviour
    {
        private EcsWorld _world;
        private EcsSystems _systemsFixedUpdate;

        private void Start()
        {
            _world = new EcsWorld();
            _systemsFixedUpdate = new EcsSystems(_world);

            _systemsFixedUpdate.ConvertScene();
            
            // Add systems
            _systemsFixedUpdate
                .Add(new PlayerInputSystem())
                .Add(new MovementSystem())
                .Add(new EngineSystem())
                ;

            // Add one frames


            // Add injects
            
            _systemsFixedUpdate.Init();
        }

        private void FixedUpdate()
        {
            _systemsFixedUpdate.Run();
        }

        private void OnDestroy()
        {
            if(_systemsFixedUpdate == null) return;
            
            _systemsFixedUpdate.Destroy();
            _systemsFixedUpdate = null;
            _world.Destroy();
            _world = null;
        }
    }
}