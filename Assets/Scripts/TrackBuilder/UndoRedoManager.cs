using System;
using System.Collections.Generic;
using Builder.Interfaces;
using cakeslice;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Builder
{
    public class UndoRedoManager : MonoBehaviour
    {
        private Stack<IAction> _historyStack = new Stack<IAction>();
        private Stack<IAction> _redoHistoryStack = new Stack<IAction>();
        private BuilderManager _builderManager;

        private void Start()
        {
            _builderManager = FindObjectOfType<BuilderManager>();
        }

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
                if (_builderManager.objectsPool.IndexOf(obj) != -1)
                {
                    _builderManager.objectsPool.RemoveAt(_builderManager.objectsPool.IndexOf(obj));
                    if (obj.GetComponent<TrackObject>().objectType == ObjectsType.Gate)
                    {
                        _builderManager.droneBuilderCheckNode.RemoveNode(obj.transform);
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
                _builderManager.objectsPool.Add(obj);
                if (obj.GetComponent<TrackObject>().objectType == ObjectsType.Gate)
                {
                    _builderManager.droneBuilderCheckNode.AddNode(obj.transform);
                }
                TrackBuilderUtils.ChangeLayerRecursively(obj.transform, LayerMask.NameToLayer("TrackGround"));
                var outlines = FindObjectsOfType<Outline>();
                TrackBuilderUtils.TurnAllOutlineEffects(outlines, false);
                SceneManager.MoveGameObjectToScene(obj, _builderManager.levelScene);
            }
        }

    }
}