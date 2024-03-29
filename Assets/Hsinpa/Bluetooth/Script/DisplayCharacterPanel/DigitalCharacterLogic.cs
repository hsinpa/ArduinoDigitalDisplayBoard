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
            _characterType = new DigitalBoardDataType.CharacterirticsData(20, digitalBoardBluetoothManager.WordCharacteristic, CharacterMapping.CharactersTable, null);

            //Register Event
            characterView.UpperCharacterTxtField.OnCharInputfieldChange += (string v) => { OnValueChange(); };
            characterView.UpperCharacterTxtField.OnColorDropDownChange += (int index) => { OnValueChange(); };
            characterView.LowerCharacterTxtField.OnCharInputfieldChange += (string v) => { OnValueChange(); };
            characterView.LowerCharacterTxtField.OnColorDropDownChange += (int index) => { OnValueChange(); };

            characterView.Force_rotate_toggle.onValueChanged.AddListener((bool ison) => { OnValueChange(); });
            characterView.Rotate_speed_slider.onValueChanged.AddListener((float v) => {
                UpdateSliderText(v);
            });

            UpdateSliderText(characterView.Rotate_speed_slider.value);
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

            bool shouldKeepLooping = characterView.Force_rotate_toggle.isOn ||
                                        (characterView.UpperCharacterTxtField.InputText.Length > maxCharacterSize ||
                                        characterView.LowerCharacterTxtField.InputText.Length > maxCharacterSize);
            try
            {
                while (!cancelSourceToken.Token.IsCancellationRequested) {
                    int timestamp = (int)(characterView.Rotate_speed_slider.value * 1000);
                    await Task.Delay(timestamp, cancelSourceToken.Token);

                    upperCharacterIndex = PerformLoopEffect(characterView.UpperCharacterTxtField.InputText, characterView.UpperCharacterTxtField.ColorIndex, 0, upperCharacterIndex);
                    lowerCharacterIndex = PerformLoopEffect(characterView.LowerCharacterTxtField.InputText, characterView.LowerCharacterTxtField.ColorIndex, 10, lowerCharacterIndex);

                    DigitalBoardDataType.BluetoothDataStruct bluetoothDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
                    {
                        characteristic = _characterType.BLECharacteristic,
                        data = _characterType.Data.ToArray()
                    };

                    digitalBoardEventSender.SendBluetoothData(bluetoothDataStruct);

                    if (!shouldKeepLooping) {
                        cancelSourceToken.Cancel();
                        break;
                    }
                }
            }
            catch {
                Debug.Log("Dispose call");
            }            
        }

        private void UpdateSliderText(double v) {
            double round_value = System.Math.Round(v, 1);
            characterView.Rotate_slider_title.text = string.Format(SimpleEvent.ID.StaticText.Hint.SliderTitle, round_value.ToString());
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

            if (full_text.Length <= maxCharacterSize && !characterView.Force_rotate_toggle.isOn) {
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