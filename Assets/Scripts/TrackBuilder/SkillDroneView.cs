using UnityEngine;
using UnityEngine.UI;

namespace Drone.Builder
{
    public class SkillDroneView : MonoBehaviour
    {
        [SerializeField] private Image skillImage;

        public void GetSkill(Sprite currentSkillSkillSprite)
        {
            gameObject.SetActive(true);
            skillImage.sprite = currentSkillSkillSprite;
        }

        public void ResetSkill()
        {
            gameObject.SetActive(false);
        }
    }
}