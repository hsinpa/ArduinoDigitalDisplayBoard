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
            if (_digitalMessageSRP.ConstraintPass(SportStruct.id))
                _digitalMessageSRP.Execute();
        }

        public void Exist()
        {

        }
        #region UI Event
        public void OnTimerUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
            if (uiDataStruct.sync_struct_table) {
                int unique_value = this._digitalMessageSRP.GetUniqueDataStructWithTable(uiDataStruct.id);
                if (unique_value >= 0) uiDataStruct.value = unique_value;
            }

            Debug.Log("OnTimerUIChange " + uiDataStruct.id + ", value " + uiDataStruct.value);

            switch (uiDataStruct.id)
            {
                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Sync_Time:
                    this._bleDataModel.PrimaryTimer.SetTimeType(DigitalTimer.Type.RealTime);
                    this._bleDataModel.PrimaryTimer.StartTimer();
                    this._bleDataModel.UpdateTime();
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Counting_mode:
                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Time_display_mode:
                    this._digitlaBoardLogicHandler.SendUIDataStructBLE(uiDataStruct, _bleDataModel.TimeType);
                    break;
            }
        }

        public void OnFunctionUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {

        }
        #endregion


    }
}