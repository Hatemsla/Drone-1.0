using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Builder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drone
{
    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private GameObject buyPanel;
        [SerializeField] private Image skillImage;
        [SerializeField] private Image buySkillTime;
        [SerializeField] private Image buySkillCoin;
        [SerializeField] private Image buySkillCrystal;
        [SerializeField] private TMP_Text timeCostText;
        [SerializeField] private TMP_Text coinCostText;
        [SerializeField] private TMP_Text crystalCostText;
        [SerializeField] private Sprite enoughMoneySprite;
        [SerializeField] private Sprite noEnoughMoneySprite;
        [SerializeField] private Timer timer;
        [SerializeField] private List<Skill> skills = new();
        [SerializeField] private BoughtSkill[] boughtSkills;

        public event Action BuySkillEvent;

        private DroneRpgController _droneRpgController;
        private Skill _currentSkill;

        private void Start()
        {
            buyPanel.SetActive(false);
            foreach (var skill in skills)
                skill.SelectSkillEvent += OnSelectSkill;
            BuilderManager.Instance.TestLevelEvent += FindDroneRpg;
        }

        private void FindDroneRpg()
        {
            if (!_droneRpgController)
            {
                _droneRpgController = FindObjectOfType<DroneRpgController>();
                _droneRpgController.SkillsCountChangedEvent += UpdateBoughtSkillsUI;
            }
        }

        private void OnDestroy()
        {
            foreach (var skill in skills)
                skill.SelectSkillEvent -= OnSelectSkill;
            
            BuilderManager.Instance.TestLevelEvent -= FindDroneRpg;
            _droneRpgController.SkillsCountChangedEvent -= UpdateBoughtSkillsUI;
        }

        private void OnSelectSkill(Skill skill)
        {
            buyPanel.SetActive(true);
            _currentSkill = skill;
            skillImage.sprite = skill.skillSprite;
            float minutes = Mathf.FloorToInt(skill.timeCost / 60);
            float seconds = Mathf.FloorToInt(skill.timeCost % 60);
            timeCostText.text = $"{minutes:00}:{seconds:00}";
            coinCostText.text = skill.coinCost.ToString(CultureInfo.InvariantCulture);
            crystalCostText.text = skill.crystalCost.ToString(CultureInfo.InvariantCulture);
            UpdateBuyButtonsUI();
        }

        private void UpdateBuyButtonsUI()
        {
            buySkillTime.sprite = timer.currentTime >= _currentSkill.timeCost && boughtSkills.Any(x => x.skill == Skills.None)
                ? enoughMoneySprite
                : noEnoughMoneySprite;
            buySkillCoin.sprite = _droneRpgController.Coins >= _currentSkill.coinCost && boughtSkills.Any(x => x.skill == Skills.None)
                ? enoughMoneySprite
                : noEnoughMoneySprite;
            buySkillCrystal.sprite =
                _droneRpgController.Crystals >= _currentSkill.crystalCost && boughtSkills.Any(x => x.skill == Skills.None)
                    ? enoughMoneySprite
                    : noEnoughMoneySprite;
        }

        public void BuyByTime()
        {
            BuySkillEvent?.Invoke();
        }

        public void BuyByCoins()
        {
            if (_droneRpgController.Coins >= _currentSkill.coinCost && boughtSkills.Any(x => x.skill == Skills.None))
            {
                _droneRpgController.Coins -= _currentSkill.coinCost;
                AddBoughtSkill(_currentSkill.skill, _currentSkill.skillSprite);
                BuySkillEvent?.Invoke();
            }
        }

        public void BuyByCrystals()
        {
            BuySkillEvent?.Invoke();
        }

        private void AddBoughtSkill(Skills skill, Sprite currentSkillSkillSprite)
        {
            foreach (var boughtSkill in boughtSkills)
            {
                if (boughtSkill.skill == skill)
                {
                    _droneRpgController.UpdateSkillValue(skill, _droneRpgController.SkillsCount[skill] + 1);
                    return;
                }
            }

            foreach (var boughtSkill in boughtSkills)
            {
                if (boughtSkill.skill == Skills.None)
                {
                    boughtSkill.GetSkillType(skill, currentSkillSkillSprite);
                    _droneRpgController.SkillsCount[skill] = boughtSkill.skillCount;
                    return;
                }
            }
        }

        private void UpdateBoughtSkillsUI()
        {
            UpdateBuyButtonsUI();
            foreach (var boughtSkill in boughtSkills)
            {
                if(!_droneRpgController.SkillsCount.ContainsKey(boughtSkill.skill))
                    continue;
                
                if (_droneRpgController.SkillsCount[boughtSkill.skill] == 0)
                {
                    boughtSkill.ResetBoughtSkill();
                }
                else
                {
                    boughtSkill.UpdateSkillCount(_droneRpgController.SkillsCount[boughtSkill.skill]);
                }
            }   
        }
    }
}