using System.Collections.Generic;
using UnityEngine;

namespace Drone.Builder.Text3D
{
    public class TextWriter3D : InteractiveObject
    {
        [SerializeField] private float spacing;
        [SerializeField] private float spaceSize = 1;
        [SerializeField] private float charactersSize = 1;
        [SerializeField] private float lineSize = 1;
        [SerializeField] private Characters characters;
        [SerializeField] private Transform charactersParent;
        [SerializeField] private GameObject selectCube;

        private readonly Dictionary<GameObject, float> _charactersWidths = new();
        private readonly Dictionary<GameObject, float> _charactersHeights = new();

        public string Text
        {
            get => text3D;
            set
            {
                text3D = value;
                PrintTextIn3D();
            }
        }

        private void Start()
        {
            foreach (var d in characters.dictionary)
            {
                var dRender = d.Value.GetComponentInChildren<Renderer>();
                _charactersWidths.Add(d.Value, dRender.bounds.size.x);
                _charactersHeights.Add(d.Value, dRender.bounds.size.y);
            }
        }
        
        private void PrintTextIn3D()
        {
            ResetLetter();

            var letterPos = transform.localPosition;
            var lineHeight = 0f;

            foreach (var c in Text)
            {
                if (c == '\n')
                {
                    letterPos.x = transform.localPosition.x;
                    letterPos.y -= (charactersSize + lineSize) * lineHeight;
                    lineHeight = 0f;
                    continue;
                }

                if (!characters.dictionary.ContainsKey(c))
                {
                    letterPos.x += (spaceSize + spacing) * charactersSize;
                    continue;
                }

                var character = Instantiate(characters.dictionary[c], letterPos, Quaternion.identity, charactersParent);
                var width = _charactersWidths[characters.dictionary[c]];
                character.transform.localScale = new Vector3(charactersSize, charactersSize, charactersSize);
                letterPos.x += (width + spacing) * charactersSize;

                var height = _charactersHeights[characters.dictionary[c]];
                if (height > lineHeight)
                    lineHeight = height;
            }

            selectCube.SetActive(Text.Length == 0);
        }

        private void ResetLetter()
        {
            foreach (Transform child in charactersParent)
                Destroy(child.gameObject);
        }

        public override void SetActive(bool active)
        {
            isActive = active;
        }

        public override void SetColorIndex(int color)
        {
            
        }
    }
}