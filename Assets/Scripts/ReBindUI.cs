using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Drone
{
    public class ReBindUI : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputActionReference; //this is on the SO

        [SerializeField] private bool excludeMouse = true;
        [Range(0, 10)] [SerializeField] private int selectedBinding;
        [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions;

        [Header("Binding Info - DO NOT EDIT")] [SerializeField]
        private InputBinding inputBinding;

        private int _bindingIndex;
        private string _actionName;

        [Header("UI Fields")] [SerializeField] private TMP_Text actionText;
        [SerializeField] private Button rebindButton;
        [SerializeField] private TMP_Text rebindText;
        [SerializeField] private Button resetButton;

        private void OnEnable()
        {
            rebindButton.onClick.AddListener(() => DoRebind());
            // resetButton.onClick.AddListener(() => ResetBinding());

            if (inputActionReference != null)
            {
                // InputManager.Instance.LoadBindingOverride(actionName);
                GetBindingInfo();
                UpdateUI();
            }

            InputManager.RebindComplete += UpdateUI;
            InputManager.RebindCanceled += UpdateUI;
        }

        private void OnDisable()
        {
            InputManager.RebindComplete -= UpdateUI;
            InputManager.RebindCanceled -= UpdateUI;
        }

        private void OnValidate()
        {
            if (inputActionReference == null)
                return;

            GetBindingInfo();
            UpdateUI();
        }

        private void GetBindingInfo()
        {
            if (inputActionReference.action != null)
                _actionName = inputActionReference.action.name;

            if (inputActionReference.action.bindings.Count > selectedBinding)
            {
                inputBinding = inputActionReference.action.bindings[selectedBinding];
                _bindingIndex = selectedBinding;
            }
        }

        private void UpdateUI()
        {
            if (actionText != null)
                actionText.text = _actionName;

            if (rebindText != null)
            {
                if (Application.isPlaying)
                    rebindText.text = InputManager.GetBindingName(_actionName, _bindingIndex);
                else
                    rebindText.text = inputActionReference.action.GetBindingDisplayString(_bindingIndex);
            }
        }

        private void DoRebind()
        {
            InputManager.Instance.StartRebind(_actionName, _bindingIndex, rebindText, excludeMouse);
        }

        private void ResetBinding()
        {
            InputManager.Instance.ResetBinding(_actionName, _bindingIndex);
            UpdateUI();
        }
    }
}