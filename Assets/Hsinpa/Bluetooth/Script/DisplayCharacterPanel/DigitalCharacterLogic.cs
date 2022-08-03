using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hsinpa.Bluetooth
{
    public class DigitalCharacterLogic : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField]
        private DigitalBoardEventSender digitalBoardEventSender;

        [SerializeField]
        DigitalBoardBluetoothManager digitalBoardBluetoothManager;

        [Header("UI")]
        [SerializeField]
        private DigitalCharacterView characterView;

        private DigitalBoardDataType.CharacterirticsData _characterType;

        private void Start()
        {
            _characterType = new DigitalBoardDataType.CharacterirticsData(20, digitalBoardBluetoothManager.WordCharacteristic, CharacterMapping.CharactersTable);

            //Register Event
            characterView.UpperCharacterTxtField.OnCharInputfieldChange += (string v) => { OnValueChange(); };
            characterView.UpperCharacterTxtField.OnColorDropDownChange += (int index) => { OnValueChange(); };
            characterView.LowerCharacterTxtField.OnCharInputfieldChange += (string v) => { OnValueChange(); };
            characterView.LowerCharacterTxtField.OnColorDropDownChange += (int index) => { OnValueChange(); };
        }

        private void OnEnable()
        {
            characterView.UpperCharacterTxtField.SetinputField("");
            characterView.LowerCharacterTxtField.SetinputField("");
        }

        private void OnValueChange() {
            TxtInputValue(_characterType, characterView.UpperCharacterTxtField.InputText, characterView.UpperCharacterTxtField.ColorIndex, 0);
            TxtInputValue(_characterType, characterView.LowerCharacterTxtField.InputText, characterView.LowerCharacterTxtField.ColorIndex, 10);

            DigitalBoardDataType.BluetoothDataStruct bluetoothDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
            {
                characteristic = _characterType.BLECharacteristic,
                data = _characterType.Data.ToArray()
            };

            digitalBoardEventSender.SendBluetoothData(bluetoothDataStruct);

            //_characterType.DebugLog();
        }

        private void TxtInputValue(DigitalBoardDataType.CharacterirticsData characterType, string text, int color_index, int offset) {
            for (int i = offset; i < 10 + offset; i++) {
                int textIndex = i - offset;
                string iputText = (textIndex < text.Length) ? text[textIndex].ToString() : "";

                if (CharacterMapping.CharactersTable.TryGetValue(iputText, out int value))
                {
                    characterType.Set_Raw_Value(i, value);
                }
            }

            characterType.Set_Raw_Value(offset + 9, color_index);
        }
    }
}