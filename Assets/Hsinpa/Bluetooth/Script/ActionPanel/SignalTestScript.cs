using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ArduinoBluetoothAPI;
using SimpleEvent.ID;

namespace Hsinpa.Bluetooth.SignalTesting
{
    public class SignalTestScript : MonoBehaviour
    {

        [SerializeField]
        private TextMeshProUGUI timer;
        private DigitalTimer _timer;

        DigitalBoardEventSender _digitalBoardEventSender;
        DigitalBoardDataType.CharacterirticsData _scoreCharacteristic;
        DigitalBoardDataType.CharacterirticsData _timeCharacteristic;
        private const float update_period = 1f;
        private float update_record = 0;

        public void SetUp(DigitalBoardEventSender digitalBoardEventSender, BluetoothHelperCharacteristic scoreCharacteristic, BluetoothHelperCharacteristic timeCharacteristic) {
            this._timer = new DigitalTimer();
            _digitalBoardEventSender = digitalBoardEventSender;
            _scoreCharacteristic = new DigitalBoardDataType.CharacterirticsData(10, scoreCharacteristic, MessageEventFlag.HsinpaBluetoothEvent.ScoreIndexTable, MessageEventFlag.HsinpaBluetoothEvent.ScoreKeyTable);
            _timeCharacteristic = new DigitalBoardDataType.CharacterirticsData(12, timeCharacteristic, MessageEventFlag.HsinpaBluetoothEvent.TimeIndexTable, MessageEventFlag.HsinpaBluetoothEvent.TimeKeyTable);
        }

        public void OnEnable()
        {
            this._timer.ResetTimer();
            this._timer.SetTimeType(DigitalTimer.Type.Timer_CountUp);
            this._timer.StartTimer();

            _scoreCharacteristic.Dispose();
            _scoreCharacteristic.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.Mode, 9);

            _timeCharacteristic.Dispose();

            _digitalBoardEventSender.SendBluetoothCharacterData(_scoreCharacteristic);
        }

        public void OnDisable()
        {
            this._timer.StopTimer();
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time >= update_record && this.gameObject.activeSelf && this._timer.TimerState)
            {
                timer.text = GetTimerText();

                _timeCharacteristic.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Time_display_mode, 5);
                _timeCharacteristic.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Counting_mode, 3);

                _timeCharacteristic.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Hour, _timer.GetHour());
                _timeCharacteristic.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Minute, _timer.GetMinute());
                _timeCharacteristic.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Second, _timer.GetSecond());

                _digitalBoardEventSender.SendBluetoothCharacterData(_timeCharacteristic);

                update_record = Time.time + update_period;
            }
        }

        public string GetTimerText() {
            int minute = this._timer.GetMinute();
            int second = this._timer.GetSecond();
            int hours = this._timer.GetHour();

            return $"Timer => {hours} : {minute} : {second}";
        }
    }
}
