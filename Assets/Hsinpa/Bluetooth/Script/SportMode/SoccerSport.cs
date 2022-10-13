using Hsinpa.Bluetooth.Model;
using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Hsinpa.Bluetooth.Sport
{
    public class SoccerSport : ISport
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
            _bleDataModel.PrimaryTimer.StartTimer(MessageEventFlag.Const.SoccerRoundMin);
            _bleDataModel.UpdateTime();

            _digitalBoardView.SetSoccerMode();

        }

        public void OnFunctionUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
            switch (uiDataStruct.id)
            {
                case MessageEventFlag.HsinpaBluetoothEvent.FunctionUI.Next_Turn:
                    this._digitlaBoardLogicHandler.SportLogicFuncs.NextTurn_Soccer_Handball(_bleDataModel, _digitalBoardView, MessageEventFlag.Const.SoccerRoundMin);
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.FunctionUI.Intermission:
                    _bleDataModel.PrimaryTimer.ResetTimer();
                    _bleDataModel.PrimaryTimer.StartTimer(MessageEventFlag.Const.Intermission_15Min);
                    _bleDataModel.UpdateTime();
                    this._digitlaBoardLogicHandler.SportLogicFuncs.SendTimeEvent(_bleDataModel.TimeType, counting_mode: 2, time_mode: 1);
                    this._digitalBoardView.Action_Timer.Start_Timer.interactable = false;
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.FunctionUI.Substitution:
                    this._digitlaBoardLogicHandler.SportLogicFuncs.ShowSubstitutionModal(uiDataStruct.value, _bleDataModel);
                    break;
            }
        }

        public void OnScoreUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
            uiDataStruct.max_value = 127; //Prevent hardware score display error

            _digitlaBoardLogicHandler.SendUIDataStructBLE(uiDataStruct, this._bleDataModel.ScoreType);
        }

        public void OnTimerUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
            switch (uiDataStruct.id)
            {
                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Start_Timer:
                    this._bleDataModel.PrimaryTimer.StartTimer();
                    break;
            }
        }

        public void ExecuteReconnectionActions()
        {
            this._digitlaBoardLogicHandler.SportLogicFuncs.ResendData(this._digitalMessageSRP, this._bleDataModel);

            int g_main_attact = this._bleDataModel.ScoreType.GetValue(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.G_main_attack);

            if (g_main_attact == 1)
                _digitalBoardView.Action_Function.Next_turn_btn.interactable = false;
        }
    }
}