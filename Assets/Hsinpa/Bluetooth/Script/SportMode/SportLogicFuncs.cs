using Hsinpa.Bluetooth.Model;
using Hsinpa.Bluetooth.View;
using Hsinpa.View;
using SimpleEvent.ID;
using System.Threading.Tasks;
using UnityEngine;

namespace Hsinpa.Bluetooth
{
    public class SportLogicFuncs
    {

        DigitalBoardEventSender _digitalBoardEventSender;
        DigitalBoardBluetoothManager _digitalBoardBluetoothManager;
        DigitlaBoardLogicHandler _digitlaBoardLogicHandler;

        public SportLogicFuncs(DigitlaBoardLogicHandler digitlaBoardLogicHandler, DigitalBoardEventSender digitalBoardEventSender,
                                DigitalBoardBluetoothManager digitalBoardBluetoothManager) {
            this._digitlaBoardLogicHandler = digitlaBoardLogicHandler;
            this._digitalBoardEventSender = digitalBoardEventSender;
            this._digitalBoardBluetoothManager = digitalBoardBluetoothManager;
        }

        public void NextTurn(DigitalBoardDataType.CharacterirticsData scoreType)
        {
            int turn_count = scoreType.GetValue(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.Sports_times);
            if (turn_count >= MessageEventFlag.MaxTurn)
            {
                Debug.LogWarning("NextTurn fail, max turn count exceed");
                return;
            }

            scoreType.Increment_Value(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.Sports_times);
            SyncScoreValue(scoreType);
        }

        public async void NextTurn_HigherScoreWin(DigitalBoardDataType.CharacterirticsData scoreType, DigitalBoardDataType.CharacterirticsData timeType ) {

            ExecNextTurn(ref scoreType, ref timeType);

            await Task.Yield();

            CleanDigitalBoard(_digitalBoardBluetoothManager, _digitalBoardEventSender);

            await Task.Yield();

            SyncScoreValue(scoreType);

            await Task.Yield();

            _digitalBoardEventSender.SendBluetoothCharacterData(timeType); //Sync time data
        }

        public async void NextTurn_Soccer_Handball(BLEDataModel bleDataModel, DigitalBoardView digitalBoardView, int target_time) {
            bleDataModel.PrimaryTimer.ResetTimer();
            bleDataModel.PrimaryTimer.StartTimer(target_time);
            bleDataModel.UpdateTime();

            this._digitlaBoardLogicHandler.SportLogicFuncs.SendTimeEvent(bleDataModel.TimeType, counting_mode: 2, time_mode: 1);

            await Task.Delay(100);

            bleDataModel.ScoreType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.G_main_attack, 1);
            bleDataModel.ScoreType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.H_main_attack, 0);
            this._digitlaBoardLogicHandler.DigitalBoardEventSender.SendBluetoothCharacterData(bleDataModel.ScoreType);

            digitalBoardView.Action_Function.Next_turn_btn.interactable = false;
            digitalBoardView.Action_Timer.Start_Timer.interactable = false;
        }

        public static void CleanDigitalBoard(DigitalBoardBluetoothManager bleManager, DigitalBoardEventSender bleEventSender) {
            SetScoreMode(MessageEventFlag.Const.ScoreMode3, bleManager.ScoreCharacteristic, bleEventSender);
            DigitalBoardDataType.BluetoothDataStruct timeDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
            {
                characteristic = bleManager.TimeCharacteristic,
                data = MessageEventFlag.ResetTimerCommand
            };

            bleEventSender.SendBluetoothData(timeDataStruct);
        }

        public static void SetScoreMode(System.Int32 mode, ArduinoBluetoothAPI.BluetoothHelperCharacteristic characteristic, DigitalBoardEventSender bleEventSender) {
            DigitalBoardDataType.BluetoothDataStruct scoreDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
            {
                characteristic = characteristic,
                data = new byte[10]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, System.Convert.ToByte(mode), 0x00, 0x00
                }
            };

            bleEventSender.SendBluetoothData(scoreDataStruct);
        }

        public void SyncScoreValue(DigitalBoardDataType.CharacterirticsData scoreType) {
            _digitlaBoardLogicHandler.SendUIDataStructBLE(
                new DigitalBoardDataType.UIDataStruct()
                {
                    id = MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.Mode,
                    category = MessageEventFlag.HsinpaBluetoothEvent.UIEvent.score,
                    value = 0
                }, 
                scoreType);
        }

        public void SendTimeEvent(DigitalBoardDataType.CharacterirticsData timeType, int counting_mode, int time_mode) {
            int record_counting_mode = timeType.GetValue(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Counting_mode);

            if (record_counting_mode == counting_mode && (counting_mode != 3))
                counting_mode = 0;

            Debug.Log("Send counting mode " + counting_mode);
           //Prevent weird behavior from counting double call
            timeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Counting_mode, counting_mode);
            timeType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Time_display_mode, time_mode);

            this._digitalBoardEventSender.SendBluetoothCharacterData(timeType);
        }

        private bool ExecNextTurn(ref DigitalBoardDataType.CharacterirticsData scoreType, ref DigitalBoardDataType.CharacterirticsData timeType) {
            int h_score = scoreType.GetValue(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.H_score);
            int g_score = scoreType.GetValue(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.G_score);
            int h_foul = scoreType.GetValue(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.H_foul);
            int g_foul = scoreType.GetValue(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.G_foul);

            int turn_count = scoreType.GetValue(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.Sports_times);

            if (turn_count >= MessageEventFlag.MaxTurn)
            {
                Debug.LogWarning("NextTurn fail, max turn count exceed");
                return false;
            }

            if (g_score == h_score)
            {
                Debug.LogWarning("NextTurn Draw");
            }

            if (g_score > h_score && g_foul < MessageEventFlag.MaxFoul)
            {
                scoreType.Increment_Value(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.G_foul);
            }

            if (h_score > g_score && h_foul < MessageEventFlag.MaxFoul)
            {
                scoreType.Increment_Value(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.H_foul);
            }

            scoreType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.H_score, 0);
            scoreType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.G_score, 0);
            scoreType.Increment_Value(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.Sports_times);

            return false;
        }

        public void ShowSubstitutionModal(int team_id, BLEDataModel bleDataModel) {
            ExtraFoulModal extraFoulModal = Modals.instance.OpenModal<ExtraFoulModal>();
            string team_label = string.Format(StaticText.Functions.SubstitutionTitle, (team_id == 0) ? "H " : "G");

            extraFoulModal.SetText(team_label, "Brought off player", "Substitute player");

            extraFoulModal.SetUp(
            (int player_id) => {
            },

            (int broghtoff_id, int substitude_id) => {

                if ((broghtoff_id < 0 || substitude_id < 0) || (broghtoff_id == substitude_id)) return;

                Modals.instance.Close();

                ExecSubstitution(team_id, broghtoff_id, substitude_id, bleDataModel);
                //ExecuteFoulConfig(team_id, player_id, foul_count, bleDataModel.TeamFoulModel.GetTotalFouls(team_id));
            });
        }

        public async void ExecSubstitution(int team_id, int broghtoff_id, int substitude_id, BLEDataModel bleDataModel) {
            SportLogicFuncs.SetScoreMode(MessageEventFlag.Const.ScoreMode9,
                                                    bleDataModel.ScoreType.BLECharacteristic,
                                                    this._digitlaBoardLogicHandler.DigitalBoardEventSender);

            bleDataModel.OtherType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.OtherUI.Substitution, 1);
            bleDataModel.OtherType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.OtherUI.Team, (team_id == 0) ? 1 : 2);
            bleDataModel.OtherType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.OtherUI.OffCourtPlayer, broghtoff_id);
            bleDataModel.OtherType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.OtherUI.OnCourtPlayer, substitude_id);

            await Task.Yield();

            this._digitlaBoardLogicHandler.DigitalBoardEventSender.SendBluetoothCharacterData(bleDataModel.OtherType);

            await Task.Delay(1000);

            //Debug.Log("Mode, " + bleDataModel.ScoreType.GetValue(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.Mode));

            bleDataModel.ScoreType.Set_Value(MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.Mode, 0);
            this._digitlaBoardLogicHandler.DigitalBoardEventSender.SendBluetoothCharacterData(bleDataModel.ScoreType);

            await Task.Yield();
            SendTimeEvent(bleDataModel.TimeType, counting_mode: 2, time_mode: 0);
        }

        public static void SendSimpleMessage(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
            Utility.SimpleEventSystem.Send(uiDataStruct.category, uiDataStruct);
        }

        public async void ResendData(BLEDataModel bleDataModel) {

            SendSimpleMessage(new DigitalBoardDataType.UIDataStruct() { id = MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Stop_Timer, 
                                                                        category = MessageEventFlag.HsinpaBluetoothEvent.UIEvent.time });

            this._digitlaBoardLogicHandler.DigitalBoardEventSender.SendBluetoothCharacterData(bleDataModel.ScoreType);

            await Task.Delay(500);

            this._digitlaBoardLogicHandler.DigitalBoardEventSender.SendBluetoothCharacterData(bleDataModel.TimeType);
        }
    }
}