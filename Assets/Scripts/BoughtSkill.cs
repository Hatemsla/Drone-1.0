using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drone
{
    public class BoughtSkill : MonoBehaviour
    {
        [SerializeField] private Image skillImage;
        [SerializeField] private Image backImage;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private Sprite backDefault;
        [SerializeField] private Sprite backBought;
        public Skills skill;

        public int skillCount;

        private void Start()
        {
            if (skill == Skills.None)
            {
                skillImage.enabled = false;
                countText.enabled = false;
            }
        }

        public void GetSkillType(Skills skills, Sprite currentSkillSkillSprite)
        {
            skill = skills;
            skillImage.sprite = currentSkillSkillSprite;
            skillCount++;
            countText.enabled = true;
            skillImage.enabled = true;
            countText.text = skillCount.ToString();
            backImage.sprite = backBought;
        }

        public void ResetBoughtSkill()
        {
            skillImage.enabled = false;
            countText.enabled = false;
            skill = Skills.None;
            backImage.sprite = backDefault;
            skillCount = 0;
        }

        public void UpdateSkillCount(int newCount)
        {
            skillCount = newCount;
            countText.text = skillCount.ToString();
        }
    }
}