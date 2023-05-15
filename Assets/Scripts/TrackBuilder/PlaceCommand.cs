using Builder.Interfaces;
using UnityEngine;

namespace Builder
{
    public class PlaceCommand : IAction
    {
        private GameObject _spawnGameObject;
        private Vector3 _objectPosition;
        private Vector3 _objectScale;
        private Quaternion _objectRotation;
        private float _yOffset;

        private GameObject _spawnedGameObject;

        public PlaceCommand(GameObject spawnGameObject, Vector3 objectPosition, Vector3 objectScale, Quaternion objectRotation, GameObject spawnedGameObject, float yOffset)
        {
            _spawnGameObject = spawnGameObject;
            _objectPosition = objectPosition;
            _objectScale = objectScale;
            _objectRotation = objectRotation;
            _spawnedGameObject = spawnedGameObject;
            _yOffset = yOffset;
        }

        public GameObject ExecuteCommand()
        {
            _spawnedGameObject = GameObject.Instantiate(_spawnGameObject, _objectPosition, _objectRotation);
            _spawnedGameObject.transform.localScale = _objectScale;
            _spawnedGameObject.GetComponent<TrackObject>().yOffset = _yOffset;
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