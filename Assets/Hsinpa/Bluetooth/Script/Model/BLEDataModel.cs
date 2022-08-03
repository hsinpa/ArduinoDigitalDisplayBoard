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


        private DigitalTimer _digitalTimer;
        public DigitalTimer DigitalTimer => _digitalTimer;

        public BLEDataModel(
            DigitalBoardDataType.CharacterirticsData scoreType, 
            DigitalBoardDataType.CharacterirticsData timeType, 
            DigitalBoardDataType.CharacterirticsData otherType)
        {
            this._scoreType = scoreType;
            this._timeType = timeType;
            this._otherType = otherType;

            this._digitalTimer =  new DigitalTimer();
        }

        public void UpdateTime() {
            if (this._digitalTimer != null && _digitalTimer.TimerState) {
                this._timeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Hour, _digitalTimer.GetHour());
                this._timeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Minute, _digitalTimer.GetMinute());
                this._timeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Second, _digitalTimer.GetSecond());
            }
        }


        public void Dispose() {
            _scoreType.Dispose();
            _timeType.Dispose();
            _otherType.Dispose();


        }


    }
}
