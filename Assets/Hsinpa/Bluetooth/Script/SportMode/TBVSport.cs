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
                    this._bleDataModel.DigitalTimer.time_type = DigitalTimer.Type.RealTime;
                    this._bleDataModel.DigitalTimer.StartTimer();
                    this._bleDataModel.UpdateTime();
                    break;
                
                //case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Start_Timer:
                //    this._bleDataModel.DigitalTimer.time_type = DigitalTimer.Type.Timer_CoutUp;
                //    this._bleDataModel.DigitalTimer.StartTimer();
                //    break;

                //case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Stop_Timer:
                //    this._bleDataModel.DigitalTimer.StopTimer();
                //    break;

                //case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Reset_Timer:
                //    this._bleDataModel.DigitalTimer.ResetTimer();
                //    this._bleDataModel.TimeType.Dispose();
                //    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Counting_mode:
                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Time_display_mode:

                    break;
            }
        }

        public void OnFunctionUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {

        }
        #endregion


    }
}