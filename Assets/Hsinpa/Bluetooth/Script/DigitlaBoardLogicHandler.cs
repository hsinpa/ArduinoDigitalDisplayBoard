using Hsinpa.Utility;
using SimpleEvent.ID;
using UnityEngine;

namespace Hsinpa.Bluetooth
{
    public class DigitlaBoardLogicHandler : MonoBehaviour
    {

        [SerializeField]
        private DigitalBoardBluetoothManager digitalBoardBluetoothManager;

        private DigitalBoardDataType.CharacterirticsData _scoreType;
        //private DigitalBoardDataType.CharacterirticsData _timeType;
        //private DigitalBoardDataType.CharacterirticsData _otherType;

        private void Awake()
        {
            Hsinpa.Utility.SimpleEventSystem.Dispose();
            this._scoreType = new DigitalBoardDataType.CharacterirticsData(10, MessageEventFlag.HsinpaBluetoothEvent.ScoreIndexTable);
            //this._timeType = new DigitalBoardDataType.CharacterirticsData(12);
            //this._otherType = new DigitalBoardDataType.CharacterirticsData(14);
        }

        private void Start()
        {
            SimpleEventSystem.CustomEventListener += OnSimpleEventSystem;
        }

        private void OnSimpleEventSystem(string id, object[] values) {
            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.score && values.Length > 0)
                OnScoreUIChange((DigitalBoardUI.UIDataStruct)values[0]);
        }

        private void OnScoreUIChange(DigitalBoardUI.UIDataStruct uiDataStruct) {

            if (uiDataStruct.is_increment) {

                if (uiDataStruct.value >= 0)
                    _scoreType.Increment_Value(uiDataStruct.id);
                else
                    _scoreType.Decrement_Value(uiDataStruct.id);
            } else {

                _scoreType.Set_Value(uiDataStruct.id, uiDataStruct.value);
            }

            digitalBoardBluetoothManager.WriteToCharacteristics(digitalBoardBluetoothManager.ScoreCharacteristic, _scoreType.Data);
        }

        private void OnDestroy()
        {
            SimpleEventSystem.CustomEventListener -= OnSimpleEventSystem;
        }
    }
}
