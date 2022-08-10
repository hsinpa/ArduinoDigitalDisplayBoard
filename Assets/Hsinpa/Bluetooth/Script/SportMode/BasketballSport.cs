using Hsinpa.Bluetooth.Model;
using Hsinpa.Bluetooth.View;
using Hsinpa.Utility;
using Hsinpa.View;
using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Hsinpa.Bluetooth.Sport
{
    public class BasketballSport : ISport
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

        public void Init()
        {
            _bleDataModel.PrimaryTimer.SetTimeType(DigitalTimer.Type.Timer_CountDown);
            _bleDataModel.PrimaryTimer.StartTimer(MessageEventFlag.Const.BasketBallRoundSec); //10 mins
            _bleDataModel.UpdateTime();

            _digitalBoardView.SetBasketballMode();
        }

        public void Exist()
        {

        }

        public void OnScoreUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
            switch (uiDataStruct.id)
            {
                case MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.H_foul:
                    OnFoulConfigClick(MessageEventFlag.Team_H);
                    return;
                case MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.G_foul:
                    OnFoulConfigClick(MessageEventFlag.Team_G);
                    return;
            }

            _digitlaBoardLogicHandler.SendUIDataStructBLE(uiDataStruct, this._bleDataModel.ScoreType);
        }


        public void OnFunctionUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
            switch (uiDataStruct.id)
            {
                case MessageEventFlag.HsinpaBluetoothEvent.FunctionUI.Next_Turn:
                    _digitlaBoardLogicHandler.SportLogicFuncs.NextTurn(this._bleDataModel.ScoreType);

                    _bleDataModel.PrimaryTimer.ResetTimer();
                    _bleDataModel.PrimaryTimer.StartTimer(MessageEventFlag.Const.BasketBallRoundSec);
                    _bleDataModel.UpdateTime();

                    this._digitlaBoardLogicHandler.SportLogicFuncs.SendTimeEvent(_bleDataModel.TimeType);
                    this._digitalBoardView.Action_Timer.Start_Timer.interactable = false;
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.FunctionUI.Intermission:
                    _bleDataModel.PrimaryTimer.ResetTimer();
                    _bleDataModel.PrimaryTimer.StartTimer(MessageEventFlag.Const.Intermission_15Sec);
                    _bleDataModel.UpdateTime();
                    this._digitlaBoardLogicHandler.SportLogicFuncs.SendTimeEvent(_bleDataModel.TimeType);
                    this._digitalBoardView.Action_Timer.Start_Timer.interactable = false;
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.FunctionUI.Short_TimeOut:
                    this._bleDataModel.TimeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Other_Second, 20);
                    this._bleDataModel.TimeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Counting_mode, 0);
                    this._digitlaBoardLogicHandler.DigitalBoardEventSender.SendBluetoothCharacterData(this._bleDataModel.TimeType);
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.FunctionUI.Long_TimeOut:
                    this._bleDataModel.TimeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Other_Second, 99);
                    this._bleDataModel.TimeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Counting_mode, 0);
                    this._digitlaBoardLogicHandler.DigitalBoardEventSender.SendBluetoothCharacterData(this._bleDataModel.TimeType);
                    break;
            }
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

        private void OnFoulConfigClick(int team_id) {
            ExtraFoulModal extraFoulModal = Modals.instance.OpenModal<ExtraFoulModal>();
            string team_label = string.Format(StaticText.Functions.ExtraFoulTitle, (team_id == 0) ? "H " : "G");

            extraFoulModal.SetText(team_label, "Player ID", "Fouls Count");

            extraFoulModal.SetUp(
            (int player_id) => {
                extraFoulModal.Input_Two_Field.text = _bleDataModel.TeamFoulModel.GetFouls(team_id, player_id).ToString();
            },

            (int player_id, int foul_count) => {
                Modals.instance.Close();

                string foul_ui = team_id == 0 ? MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.H_foul : MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.G_foul;
                _bleDataModel.TeamFoulModel.SetPlayerValue(team_id, player_id, foul_count);

                ExecuteFoulConfig(team_id, player_id, foul_count, _bleDataModel.TeamFoulModel.GetTotalFouls(team_id));

                SimpleEventSystem.Send(MessageEventFlag.HsinpaBluetoothEvent.UIEvent.ui_text,
                    new DigitalBoardDataType.UIDataStruct()
                    {
                        id = foul_ui,
                        value = _bleDataModel.TeamFoulModel.GetTotalFouls(team_id)
                    }
                );
            });
        }

        private async void ExecuteFoulConfig(int team_id, int player_id, int foul_count, int total_foul) {
            SportLogicFuncs.SetScoreMode(MessageEventFlag.Const.ScoreMode9, 
                                                            this._bleDataModel.ScoreType.BLECharacteristic, 
                                                            this._digitlaBoardLogicHandler.DigitalBoardEventSender);

            this._bleDataModel.OtherType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.OtherUI.FoulMode, (team_id == 0) ? 1 : 2);
            this._bleDataModel.OtherType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.OtherUI.FoulPlayer, player_id);
            this._bleDataModel.OtherType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.OtherUI.FoulCount, foul_count);

            string score_foul_id = (team_id == MessageEventFlag.Team_H) ? MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.H_foul : MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.G_foul;
            this._bleDataModel.ScoreType.Set_Value(score_foul_id, total_foul);

            this._digitlaBoardLogicHandler.DigitalBoardEventSender.SendBluetoothCharacterData(this._bleDataModel.ScoreType);

            await Task.Delay(System.TimeSpan.FromSeconds(0.2f));

            this._digitlaBoardLogicHandler.DigitalBoardEventSender.SendBluetoothCharacterData(this._bleDataModel.OtherType);

            await Task.Delay(System.TimeSpan.FromSeconds(1));

            //Set foul to 0, prevent basketball count down display error
            if (MessageEventFlag.HsinpaBluetoothEvent.ScoreIndexTable.TryGetValue(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.H_foul, out int h_index) &&
                MessageEventFlag.HsinpaBluetoothEvent.ScoreIndexTable.TryGetValue(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.G_foul, out int g_index)) {
                this._bleDataModel.ScoreType.Set_Raw_Value(h_index, 0);
                this._bleDataModel.ScoreType.Set_Raw_Value(g_index, 0);
                this._digitlaBoardLogicHandler.DigitalBoardEventSender.SendBluetoothCharacterData(this._bleDataModel.ScoreType);
            }
        }
    }
}