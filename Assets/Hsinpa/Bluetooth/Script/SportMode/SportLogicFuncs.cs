using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
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

            SyncTimeValue(timeType);
        }

        public static void CleanDigitalBoard(DigitalBoardBluetoothManager bleManager, DigitalBoardEventSender bleEventSender) {
            DigitalBoardDataType.BluetoothDataStruct scoreDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
            {
                characteristic = bleManager.ScoreCharacteristic,
                data = new byte[10]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00
                }
            };

            bleEventSender.SendBluetoothData(scoreDataStruct);

            DigitalBoardDataType.BluetoothDataStruct timeDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
            {
                characteristic = bleManager.TimeCharacteristic,
                data = MessageEventFlag.ResetTimerCommand
            };

            bleEventSender.SendBluetoothData(timeDataStruct);
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

        public void SyncTimeValue(DigitalBoardDataType.CharacterirticsData timeType)
        {
            DigitalBoardDataType.BluetoothDataStruct timeDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
            {
                characteristic = _digitalBoardBluetoothManager.TimeCharacteristic,
                data = timeType.Data
            };

            _digitalBoardEventSender.SendBluetoothData(timeDataStruct);
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

    }
}