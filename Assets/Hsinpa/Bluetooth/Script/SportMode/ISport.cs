using Hsinpa.Bluetooth.Model;
using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hsinpa.Bluetooth.Sport
{
    public interface ISport
    {

        DigitalMessageSRP SRP { get; }
        MessageEventFlag.HsinpaBluetoothEvent.SportSettingStruct SportStruct { get; }

        void Setup(DigitlaBoardLogicHandler digitlaBoardLogicHandler, MessageEventFlag.HsinpaBluetoothEvent.SportSettingStruct sportStruct, 
                    BLEDataModel bleDataModel, DigitalBoardView digitalBoardView);

        void SetSportSRP(DigitalMessageSRP srp);

        void Init();
        void Exist();

        void OnTimerUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct);
        void OnFunctionUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct);
    }
}