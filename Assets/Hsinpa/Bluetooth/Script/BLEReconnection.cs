using ArduinoBluetoothAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hsinpa.Pref;
using Hsinpa.Bluetooth.Model;
using System;
using SimpleEvent.ID;
using static SimpleEvent.ID.MessageEventFlag;

namespace Hsinpa.Bluetooth
{
    public class BLEReconnection
    {
        public static MessageEventFlag.HsinpaBluetoothEvent.SportSaveStruct GetSaveFile() {
            return PlayerPrefLoader.GetJSON<MessageEventFlag.HsinpaBluetoothEvent.SportSaveStruct>(MessageEventFlag.PlayerPref.Save);
        }

        public static void Save(string sport_id, BLEDataModel bleDataModel ) {
            var sportSaveStruct = new SimpleEvent.ID.MessageEventFlag.HsinpaBluetoothEvent.SportSaveStruct();
            
            sportSaveStruct.sport_id = sport_id;
            sportSaveStruct.scores = bleDataModel.ScoreType.IntData;
            sportSaveStruct.others = bleDataModel.OtherType.IntData;

            sportSaveStruct.time_left_second = (int)bleDataModel.PrimaryTimer.Leak_datetime.TotalSeconds;

            sportSaveStruct.timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            sportSaveStruct.foulStructs = bleDataModel.TeamFoulModel.playersStruct;

            PlayerPrefLoader.SaveJSON(MessageEventFlag.PlayerPref.Save, sportSaveStruct);
        }

        public static void Dispose() {
            PlayerPrefLoader.Delete(MessageEventFlag.PlayerPref.Save);
        }
    }
}