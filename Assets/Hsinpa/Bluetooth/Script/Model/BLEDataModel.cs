using Hsinpa.Utility;
using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Bluetooth.Model {
    public class BLEDataModel
    {
        private DigitalBoardDataType.CharacterirticsData _scoreType;
        public DigitalBoardDataType.CharacterirticsData ScoreType => _scoreType;

        private DigitalBoardDataType.CharacterirticsData _timeType;
        public DigitalBoardDataType.CharacterirticsData TimeType => _timeType;

        private DigitalBoardDataType.CharacterirticsData _otherType;
        public DigitalBoardDataType.CharacterirticsData OtherType => _otherType;


        private DigitalTimer _primaryTimer;
        public DigitalTimer PrimaryTimer => _primaryTimer;

        private DigitalTimer _secondaryTimer;
        public DigitalTimer SecondaryTimer => _secondaryTimer;


        public BLEDataModel(
            DigitalBoardDataType.CharacterirticsData scoreType, 
            DigitalBoardDataType.CharacterirticsData timeType, 
            DigitalBoardDataType.CharacterirticsData otherType)
        {
            this._scoreType = scoreType;
            this._timeType = timeType;
            this._otherType = otherType;

            this._primaryTimer =  new DigitalTimer();
            this._secondaryTimer = new DigitalTimer();
            this._secondaryTimer.SetTimeType(DigitalTimer.Type.Timer_CountDown);
        }

        public void UpdateTime() {
            UpdatePrimaryTimer(this._timeType, _primaryTimer);
        }


        public void Dispose() {
            _scoreType.Dispose();
            _timeType.Dispose();
            _otherType.Dispose();
        }

        private void UpdatePrimaryTimer(DigitalBoardDataType.CharacterirticsData timeType, DigitalTimer timer) {

            if (timer != null && timer.TimerState)
            {
                timeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Hour, timer.GetHour());
                timeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Minute, timer.GetMinute());
                timeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Second, timer.GetSecond());
            }
        }

    }
}
