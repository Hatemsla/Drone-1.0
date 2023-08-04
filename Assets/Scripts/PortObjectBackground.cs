using System;
using Builder;
using UnityEngine;
using UnityEngine.UI;

namespace Drone
{
    public class PortObjectBackground : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Sprite yellow;
        [SerializeField] private Sprite red;
        [SerializeField] private Sprite green;
        [SerializeField] private Sprite blue;

        private Action _connectionAction;

        public void Connection()
        {
            _connectionAction?.Invoke();
        }
        
        public void GetConnectionAction(Action action)
        {
            _connectionAction = action;
        }

        public void SetBackground(int colorIndex)
        {
            switch (colorIndex)
            {
                case 0:
                    background.sprite = red;
                    break;
                case 1:
                    background.sprite = green;
                    break;
                case 2:
                    background.sprite = blue;
                    break;
                case 3:
                    background.sprite = yellow;
                    break;
            }
        }
    }
}