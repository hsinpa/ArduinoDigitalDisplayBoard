using Hsinpa.Utility;
using SimpleEvent.ID;
using UnityEngine;

using System.Linq;
using ArduinoBluetoothAPI;

namespace Hsinpa.Bluetooth
{
    public class DigitlaBoardLogicHandler : MonoBehaviour
    {

        [SerializeField]
        private DigitalBoardBluetoothManager digitalBoardBluetoothManager;

        [SerializeField]
        private DigitalBoardEventSender digitalBoardEventSender;


        private DigitalBoardDataType.CharacterirticsData _scoreType;
        private DigitalBoardDataType.CharacterirticsData _timeType;
        private DigitalBoardDataType.CharacterirticsData _otherType;

        private DigitalTimer _digitalTimer;

        private const float update_period = 0.5f;
        private float update_record = 0;

        private void Awake()
        {
            Hsinpa.Utility.SimpleEventSystem.Dispose();
            this._scoreType = new DigitalBoardDataType.CharacterirticsData(10, MessageEventFlag.HsinpaBluetoothEvent.ScoreIndexTable);
            this._timeType = new DigitalBoardDataType.CharacterirticsData(11, MessageEventFlag.HsinpaBluetoothEvent.TimeIndexTable);
            //this._otherType = new DigitalBoardDataType.CharacterirticsData(14);

            this._digitalTimer = new DigitalTimer();
        }

        private void Start()
        {
            SimpleEventSystem.CustomEventListener += OnSimpleEventSystem;
        }

        private void Update()
        {
            if (this._digitalTimer != null && _digitalTimer.TimerState && Time.time >= update_record)
            {
                UpdateTimeData(this._digitalTimer);

                update_record = Time.time + update_period;
            }
        }

        private void UpdateTimeData(DigitalTimer p_digital_timer) {
            _timeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Hour, p_digital_timer.GetHour());
            _timeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Minute, p_digital_timer.GetMinute());
            _timeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Second, p_digital_timer.GetSecond());

            //Debug.Log(p_digital_timer.GetSecond());

            //DigitalBoardDataType.BluetoothDataStruct bluetoothDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
            //{
            //    characteristic = digitalBoardBluetoothManager.TimeCharacteristic,
            //    data = _timeType.Data.ToArray()
            //};

            //digitalBoardEventSender.SendBluetoothData(bluetoothDataStruct);
        }

        private void SendUIDataStructBLE(DigitalBoardDataType.UIDataStruct uiDataStruct,
            DigitalBoardDataType.CharacterirticsData characteristic_data,
            BluetoothHelperCharacteristic ble_characteristic) {

            if (uiDataStruct.is_increment)
            {
                if (uiDataStruct.value >= 0)
                    characteristic_data.Increment_Value(uiDataStruct.id);
                else
                    characteristic_data.Decrement_Value(uiDataStruct.id);
            }
            else
            {

                characteristic_data.Set_Value(uiDataStruct.id, uiDataStruct.value);
            }

            if (uiDataStruct.hide_bluetooth_event) return;

            DigitalBoardDataType.BluetoothDataStruct bluetoothDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
            {
                characteristic = ble_characteristic,
                data = characteristic_data.Data.ToArray()
            };

            digitalBoardEventSender.SendBluetoothData(bluetoothDataStruct);
        }

        #region Event
        private void OnSimpleEventSystem(string id, object[] values) {
            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.score && values.Length > 0)
                OnScoreUIChange((DigitalBoardDataType.UIDataStruct)values[0]);

            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.time && values.Length > 0)
                OnTimerUIChange((DigitalBoardDataType.UIDataStruct)values[0]);

            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.dispose) {
                Dispose();
            }
        }

        private void OnScoreUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct) {
            Debug.Log("OnScoreUIChange " + uiDataStruct.id);

            SendUIDataStructBLE(uiDataStruct, _scoreType, digitalBoardBluetoothManager.ScoreCharacteristic);
            //if (uiDataStruct.is_increment) {

            //    if (uiDataStruct.value >= 0)
            //        _scoreType.Increment_Value(uiDataStruct.id);
            //    else
            //        _scoreType.Decrement_Value(uiDataStruct.id);
            //} else {

            //    _scoreType.Set_Value(uiDataStruct.id, uiDataStruct.value);
            //}

            //if (uiDataStruct.hide_bluetooth_event) return;

            //DigitalBoardDataType.BluetoothDataStruct bluetoothDataStruct = new DigitalBoardDataType.BluetoothDataStruct() {
            //    characteristic = digitalBoardBluetoothManager.ScoreCharacteristic,
            //    data = _scoreType.Data.ToArray()
            //};

            //digitalBoardEventSender.SendBluetoothData(bluetoothDataStruct);
        }

        private void OnTimerUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct) {
            Debug.Log("OnTimerUIChange " + uiDataStruct.id);

            switch (uiDataStruct.id) {
                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Minute:

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Start_Timer:
                    _digitalTimer.StartTimer();
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Stop_Timer:
                    _digitalTimer.StopTimer();
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Reset_Timer:
                    _digitalTimer.ResetTimer();
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Counting_mode:
                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Time_display_mode:
                    SendUIDataStructBLE(uiDataStruct, _timeType, digitalBoardBluetoothManager.TimeCharacteristic);
                    break;
            }
        }

        private void OnDestroy()
        {
            SimpleEventSystem.CustomEventListener -= OnSimpleEventSystem;
        }

        private void Dispose()
        {
            _scoreType.Dispose();
            _timeType.Dispose();
            _digitalTimer.ResetTimer();
        }

        #endregion
    }
}
