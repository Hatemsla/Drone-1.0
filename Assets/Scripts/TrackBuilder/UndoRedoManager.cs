using System;
using System.Collections.Generic;
using cakeslice;
using Drone.Builder.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Drone.Builder
{
    public class UndoRedoManager : MonoBehaviour
    {
        private Stack<IAction> _historyStack = new Stack<IAction>();
        private Stack<IAction> _redoHistoryStack = new Stack<IAction>();

        public void ExecuteCommand(IAction action)
        {
            _historyStack.Push(action);
            _redoHistoryStack.Clear();
        }

        public void UndoCommand()
        {
            if (_historyStack.Count > 0)
            {
                _redoHistoryStack.Push(_historyStack.Peek());
                var obj = _historyStack.Peek().GetCommand();
                if (BuilderManager.Instance.objectsPool.IndexOf(obj) != -1)
                {
                    BuilderManager.Instance.objectsPool.RemoveAt(BuilderManager.Instance.objectsPool.IndexOf(obj));
                    if (obj.GetComponent<TrackObject>().objectType == ObjectsType.Gate)
                    {
                        BuilderManager.Instance.droneBuilderCheckNode.RemoveNode(obj.transform);
                    }
                    _historyStack.Pop().UndoCommand();
                }
            }
        }
        
        public void RedoCommand()
        {
            if (_redoHistoryStack.Count > 0)
            {
                _historyStack.Push(_redoHistoryStack.Peek());
                var obj = _redoHistoryStack.Pop().ExecuteCommand();
                BuilderManager.Instance.objectsPool.Add(obj);
                if (obj.GetComponent<TrackObject>().objectType == ObjectsType.Gate)
                {
                    BuilderManager.Instance.droneBuilderCheckNode.AddNode(obj.transform);
                }
                TrackBuilderUtils.ChangeLayerRecursively(obj.transform, LayerMask.NameToLayer("TrackGround"));
                var outlines = FindObjectsOfType<Outline>();
                TrackBuilderUtils.TurnAllOutlineEffects(outlines, false);
                SceneManager.MoveGameObjectToScene(obj, BuilderManager.Instance.levelScene);
            }
        }

    }
}