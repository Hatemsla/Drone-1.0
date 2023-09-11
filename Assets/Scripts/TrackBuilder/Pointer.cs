using Cinemachine;
using UnityEngine;

namespace Drone.Builder
{
    public static class Pointer
    {
        public static void PointerPosition(BuilderUI builderUI, CheckNode droneBuilderCheckNode, CinemachineBrain cameraBrain, Vector3 startPointerSize, float interfaceScale)
        {
            var targetCheckpoint = droneBuilderCheckNode.nodes[droneBuilderCheckNode.currentNode].transform;
            var realPos = cameraBrain.OutputCamera.WorldToScreenPoint(targetCheckpoint.position);
            var rect = new Rect(0, 0, Screen.width, Screen.height);

            var outPos = realPos;
            float direction = 1;

            builderUI.pathArrowImage.sprite = builderUI.outOfScreenIcon;

            if (!IsBehind(targetCheckpoint.position, cameraBrain)) // если цель спереди
            {
                if (rect.Contains(realPos)) // и если цель в окне экрана
                {
                    builderUI.pathArrowImage.sprite = builderUI.pointerIcon;
                }
            }
            else // если цель cзади
            {
                realPos = -realPos;
                outPos = new Vector3(realPos.x, 0, 0); // позиция иконки - снизу
                if (cameraBrain.transform.position.y < targetCheckpoint.position.y)
                {
                    direction = -1;
                    outPos.y = Screen.height; // позиция иконки - сверху				
                }
            }

            // ограничиваем позицию областью экрана
            var offset = builderUI.pathArrow.sizeDelta.x / 2;
            outPos.x = Mathf.Clamp(outPos.x, offset, Screen.width - offset);
            outPos.y = Mathf.Clamp(outPos.y, offset, Screen.height - offset);

            var pos = realPos - outPos; // направление к цели из PointerUI 

            RotatePointer(direction * pos, builderUI);

            builderUI.pathArrow.sizeDelta = new Vector2(startPointerSize.x / 100 * interfaceScale,
                startPointerSize.y / 100 * interfaceScale);
            builderUI.pathArrow.position = outPos;
        }

        private static bool IsBehind(Vector3 point, CinemachineBrain cameraBrain) // true если point сзади камеры
        {
            var forward = cameraBrain.transform.TransformDirection(Vector3.forward);
            var toOther = point - cameraBrain.transform.position;
            return Vector3.Dot(forward, toOther) < 0;
        }

        private static void RotatePointer(Vector2 direction, BuilderUI builderUI) // поворачивает PointerUI в направление direction
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            builderUI.pathArrow.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}