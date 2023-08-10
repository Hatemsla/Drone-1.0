using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drone.Builder
{
    public class Skill : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private Image skillImage;
        [SerializeField] private Sprite defaultBack;
        [SerializeField] private Sprite hoverBack;
        public Skills skill;
        public Sprite skillSprite;
        public int timeCost = 60;
        public int coinCost = 10;
        public int crystalCost = 1;

        public event Action<Skill> SelectSkillEvent;

        private void Start()
        {
            if (skillSprite != null)
                skillImage.sprite = skillSprite;
            else
                skillImage.enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && skillSprite != null)
                SelectSkillEvent?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(skillSprite != null)
                background.sprite = hoverBack;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(skillSprite != null)
                background.sprite = defaultBack;
        }
    }
}