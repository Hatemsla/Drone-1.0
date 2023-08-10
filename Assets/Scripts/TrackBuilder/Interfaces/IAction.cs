using UnityEngine;

namespace Drone.Builder.Interfaces
{
    public interface IAction
    {
        public GameObject ExecuteCommand();
        public void UndoCommand();
        public GameObject GetCommand();
    }
}