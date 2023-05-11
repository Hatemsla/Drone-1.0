using Builder.Interfaces;
using UnityEngine;

namespace Builder
{
    public class PlaceCommand : IAction
    {
        private GameObject _spawnGameObject;
        private Vector3 _objectPosition;
        private Quaternion _objectRotation;

        private GameObject _spawnedGameObject;

        public PlaceCommand(GameObject spawnGameObject, Vector3 objectPosition, Quaternion objectRotation, GameObject spawnedGameObject)
        {
            _spawnGameObject = spawnGameObject;
            _objectPosition = objectPosition;
            _objectRotation = objectRotation;
            _spawnedGameObject = spawnedGameObject;
        }

        public GameObject ExecuteCommand()
        {
            _spawnedGameObject = GameObject.Instantiate(_spawnGameObject, _objectPosition, _objectRotation);
            return _spawnedGameObject;
        }

        public GameObject GetCommand()
        {
            return _spawnedGameObject;
        }

        public void UndoCommand()
        {
            GameObject.Destroy(_spawnedGameObject);
        }
    }
}