using Hsinpa.Bluetooth.Model;
using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Bluetooth.Sport
{
    public class HandBallSport : ISport
    {

        DigitlaBoardLogicHandler _digitlaBoardLogicHandler;
        BLEDataModel _bleDataModel;
        DigitalBoardView _digitalBoardView;
        DigitalMessageSRP _digitalMessageSRP;
        MessageEventFlag.HsinpaBluetoothEvent.SportSettingStruct _sportStruct;

        public DigitalMessageSRP SRP => _digitalMessageSRP;

        public MessageEventFlag.HsinpaBluetoothEvent.SportSettingStruct SportStruct => _sportStruct;


        public void SetSportSRP(DigitalMessageSRP srp)
        {
            this._digitalMessageSRP = srp;
        }
        public void Setup(DigitlaBoardLogicHandler digitlaBoardLogicHandler, MessageEventFlag.HsinpaBluetoothEvent.SportSettingStruct sportStruct, BLEDataModel bleDataModel, DigitalBoardView digitalBoardView)
        {
            this._digitlaBoardLogicHandler = digitlaBoardLogicHandler;
            this._sportStruct = sportStruct;
            this._bleDataModel = bleDataModel;
            this._digitalBoardView = digitalBoardView;
        }

        public void Exist()
        {
        }

        public void Init()
        {
            _bleDataModel.PrimaryTimer.SetTimeType(DigitalTimer.Type.Timer_CountDown);
            _bleDataModel.PrimaryTimer.StartTimer(MessageEventFlag.Const.HandballRoundSec);
            _bleDataModel.UpdateTime();

            _digitalBoardView.SetHandballMode();
        }

        public void OnFunctionUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
        }

        public void OnScoreUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
        }

        public void OnTimerUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
        }
    }
}