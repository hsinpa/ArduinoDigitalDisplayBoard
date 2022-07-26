using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Hsinpa.Bluetooth
{
    public class DigitalCharacterTextField : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField character_inputfield;
        public string InputText => character_inputfield.text;

        [SerializeField]
        private TMP_Dropdown color_dropdown;
        public int ColorIndex => color_dropdown.value;

        public System.Action<int> OnColorDropDownChange;
        public System.Action<string> OnCharInputfieldChange;

        private void Start()
        {
            character_inputfield.onEndEdit.AddListener(OnInputChange);
            color_dropdown.onValueChanged.AddListener(OnColorDropdownChange);
        }

        public void SetColorIndex(int index) {
            color_dropdown.SetValueWithoutNotify(index);
        }

        public void SetinputField(string text) {
            character_inputfield.SetTextWithoutNotify(text);
        }

        private void OnInputChange(string text) {
            Debug.Log("DigitalCharacterTextField OnInputChange " + text);

            if (OnCharInputfieldChange != null) OnCharInputfieldChange(text);
        }

        private void OnColorDropdownChange(int c_index)
        {
            Debug.Log("DigitalCharacterTextField OnColorDropdownChange " + c_index);

            if (OnColorDropDownChange != null) OnColorDropDownChange(c_index);
        }
    }
}