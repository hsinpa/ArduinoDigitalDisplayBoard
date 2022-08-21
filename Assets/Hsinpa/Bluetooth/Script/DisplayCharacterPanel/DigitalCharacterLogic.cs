using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

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

        private int maxCharacterSize = 9;
        private string empty9Header = "         ";

        CancellationTokenSource cancelSourceToken = new CancellationTokenSource();


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

        private async void OnValueChange() {
            //TxtInputValue(_characterType, characterView.UpperCharacterTxtField.InputText, characterView.UpperCharacterTxtField.ColorIndex, 0);
            //TxtInputValue(_characterType, characterView.LowerCharacterTxtField.InputText, characterView.LowerCharacterTxtField.ColorIndex, 10);

            if (cancelSourceToken != null)
                cancelSourceToken.Cancel();

            cancelSourceToken = new CancellationTokenSource();

            int upperCharacterIndex = maxCharacterSize;
            int lowerCharacterIndex = maxCharacterSize;

            while (!cancelSourceToken.Token.IsCancellationRequested)
            {
                await Task.Delay(1000, cancelSourceToken.Token);

                upperCharacterIndex = PerformLoopEffect(characterView.UpperCharacterTxtField.InputText, characterView.UpperCharacterTxtField.ColorIndex, 0, upperCharacterIndex);
                lowerCharacterIndex = PerformLoopEffect(characterView.LowerCharacterTxtField.InputText, characterView.LowerCharacterTxtField.ColorIndex, 10, lowerCharacterIndex);

                DigitalBoardDataType.BluetoothDataStruct bluetoothDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
                {
                    characteristic = _characterType.BLECharacteristic,
                    data = _characterType.Data.ToArray()
                };

                digitalBoardEventSender.SendBluetoothData(bluetoothDataStruct);
            }
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

        private int PerformLoopEffect(string full_text, int color_index, int offset, int current_index)
        {
            if (full_text.Length <= 0) return current_index;

            if (full_text.Length <= maxCharacterSize) {
                TxtInputValue(_characterType, full_text, color_index, offset);

                return current_index;
            }

            string editedFullText = empty9Header + full_text;
            int fullLen = editedFullText.Length;

            int max_index = Mathf.Clamp(maxCharacterSize, 0, fullLen - (current_index));
            string splice_text = editedFullText.Substring(current_index, max_index);

            TxtInputValue(_characterType, splice_text, color_index, offset);

            return (current_index + 1) % fullLen;            
        }

        public void Dispose() {
            if (cancelSourceToken != null)
                cancelSourceToken.Cancel();

            cancelSourceToken = null;

        }

        private void OnDisable()
        {
            Dispose();
        }
    }
}