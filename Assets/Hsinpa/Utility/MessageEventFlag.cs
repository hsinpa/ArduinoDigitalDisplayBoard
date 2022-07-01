using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleEvent.ID {
    public class MessageEventFlag : MonoBehaviour
    {
        public class HsinpaBluetoothEvent {

            public class UIEvent {
                public const string score = "event@ui_score";
                public const string time = "event@ui_time";
                public const string other = "event@ui_other";
            }

            public class ScoreUI
            {
                public const string H_score = "score@h_score";
                public const string G_score = "score@g_score";
                public const string H_foul = "score@h_foul";
                public const string G_foul = "score@g_foul";
                public const string H_main_attack = "score@h_main_attack";
                public const string G_main_attack = "score@g_main_attack";
                public const string Sports_times = "score@sport_time";
                public const string Mode = "score@mode";
                public const string H_name = "score@h_name";
                public const string G_name = "score@g_name";
            }


            public static Dictionary<string, int> ScoreIndexTable = new Dictionary<string, int>() {
                { ScoreUI.H_score, 0},
                { ScoreUI.G_score, 1},
                { ScoreUI.H_foul, 2},
                { ScoreUI.G_foul, 3},
                { ScoreUI.H_main_attack, 4},
                { ScoreUI.G_main_attack, 5},
                { ScoreUI.Sports_times, 6},
                { ScoreUI.Mode, 7},
                { ScoreUI.H_name, 8},
                { ScoreUI.G_name, 9},
            };

            //public static Dictionary<string, int> TimeIndexTable = new Dictionary<string, int>() {
            //    { ScoreUI.H_score, 0},
            //    { ScoreUI.G_score, 1},
            //    { ScoreUI.H_foul, 2},
            //    { ScoreUI.G_foul, 3},
            //    { ScoreUI.H_main_attack, 4},
            //    { ScoreUI.G_main_attack, 5},
            //    { ScoreUI.Sports_times, 6},
            //    { ScoreUI.Mode, 7},
            //    { ScoreUI.H_name, 8},
            //    { ScoreUI.G_name, 9},
            //};

            //public static Dictionary<string, int> OtherIndexTable = new Dictionary<string, int>() {
            //    { ScoreUI.H_score, 0},
            //    { ScoreUI.G_score, 1},
            //    { ScoreUI.H_foul, 2},
            //    { ScoreUI.G_foul, 3},
            //    { ScoreUI.H_main_attack, 4},
            //    { ScoreUI.G_main_attack, 5},
            //    { ScoreUI.Sports_times, 6},
            //    { ScoreUI.Mode, 7},
            //    { ScoreUI.H_name, 8},
            //    { ScoreUI.G_name, 9},
            //};
        }
    }
}
