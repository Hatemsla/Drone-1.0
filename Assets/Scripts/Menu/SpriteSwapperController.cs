using System;
using UnityEngine;

namespace Menu
{
    public class SpriteSwapperController : MonoBehaviour
    {
        public SpriteSwapper spriteSwapper;

        private void Start()
        {
            spriteSwapper = GetComponent<SpriteSwapper>();
            HandleLevelNameChanged("");
            MenuManager.Instance.LevelNameChanged += HandleLevelNameChanged;
        }

        private void HandleLevelNameChanged(string level)
        {
            if (level.Length > 0)
            {
                spriteSwapper.objectImage.sprite = spriteSwapper.defaultSprite;
                spriteSwapper.enabled = true;
                spriteSwapper.button.enabled = true;
            }
            else
            {
                spriteSwapper.objectImage.sprite = spriteSwapper.disabledSprite;
                spriteSwapper.enabled = false;
                spriteSwapper.button.enabled = false;
            }
        }
    }
}