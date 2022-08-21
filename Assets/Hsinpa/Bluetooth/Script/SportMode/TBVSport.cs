using Hsinpa.Bluetooth.Model;
using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Bluetooth.Sport
{
    public class TBVSport : ISport
    {
        DigitlaBoardLogicHandler _digitlaBoardLogicHandler;
        BLEDataModel _bleDataModel;  
        DigitalBoardView _digitalBoardView;
        DigitalMessageSRP _digitalMessageSRP;
        MessageEventFlag.HsinpaBluetoothEvent.SportSettingStruct _sportStruct;

        public DigitalMessageSRP SRP => _digitalMessageSRP;

        public MessageEventFlag.HsinpaBluetoothEvent.SportSettingStruct SportStruct => _sportStruct;

        public void Setup(DigitlaBoardLogicHandler digitlaBoardLogicHandler, MessageEventFlag.HsinpaBluetoothEvent.SportSettingStruct sportStruct, BLEDataModel bleDataModel, DigitalBoardView digitalBoardView)
        {
            this._digitlaBoardLogicHandler = digitlaBoardLogicHandler;
            this._sportStruct = sportStruct;
            this._bleDataModel = bleDataModel;
            this._digitalBoardView = digitalBoardView;
        }

        public void SetSportSRP(DigitalMessageSRP srp)
        {
            this._digitalMessageSRP = srp;
        }

        public void Init()
        {
            _digitalBoardView.SetTVBMode();
        }

        public void Exist()
        {
        }


        #region UI Event
        public void OnScoreUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
            _digitlaBoardLogicHandler.SendUIDataStructBLE(uiDataStruct, this._bleDataModel.ScoreType);
        }

        public void OnTimerUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {

            Debug.Log("OnTimerUIChange " + uiDataStruct.id + ", value " + uiDataStruct.value);

            switch (uiDataStruct.id)
            {
                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Sync_Time:
                    this._bleDataModel.PrimaryTimer.SetTimeType(DigitalTimer.Type.RealTime);
                    this._bleDataModel.PrimaryTimer.StartTimer();
                    this._bleDataModel.UpdateTime();
                    break;

            }
        }

        public void OnFunctionUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
            switch (uiDataStruct.id)
            {
                case MessageEventFlag.HsinpaBluetoothEvent.FunctionUI.Next_Turn:
                    _digitlaBoardLogicHandler.SportLogicFuncs.NextTurn_HigherScoreWin(this._bleDataModel.ScoreType, this._bleDataModel.TimeType);
                    break;
            }
        }

        public void ExecuteReconnectionActions()
        {
            throw new System.NotImplementedException();
        }
        #endregion


    }
}