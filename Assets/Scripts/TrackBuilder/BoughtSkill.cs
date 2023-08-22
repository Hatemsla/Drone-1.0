using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drone.Builder
{
    public class BoughtSkill : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image skillImage;
        [SerializeField] private Image backImage;
        [SerializeField] private TMP_Text countText;
        [SerializeField] private Sprite backDefault;
        [SerializeField] private Sprite backBought;

        public Skills skill;
        public int skillCount;
        
        public event Action<BoughtSkill> SellSkillEvent;
        
        private void Start()
        {
            if (skill == Skills.None)
            {
                skillImage.enabled = false;
                countText.enabled = false;
            }
        }

        public void SetSkillType(Skills skills, Sprite currentSkillSprite)
        {
            skill = skills;
            skillImage.sprite = currentSkillSprite;
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
            
            if(skillCount == 0)
                ResetBoughtSkill();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(skill == Skills.None && skillCount > 0)
                return;

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                SellSkillEvent?.Invoke(this);
            }
        }
    }
}