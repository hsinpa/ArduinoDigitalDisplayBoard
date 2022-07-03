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

                public const string dispose = "event@dispose";
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

            public class TimeUI
            {
                public const string Year = "time@year";
                public const string Month = "time@month";
                public const string Day = "time@day";
                public const string Hour = "time@hour";
                public const string Minute = "time@minute";
                public const string Second = "time@second";
                public const string Weekday = "time@weekday";
                public const string Subsecond = "time@subsecond";
                public const string adjust_reason = "time@adjust_reason";
                public const string Counting_mode = "time@counting_mode";
                public const string Time_display_mode = "time@time_display_mode";
                public const string Other_Second = "time@other_second";

                //Action
                public const string Start_Timer = "time_event@start_timer";
                public const string Stop_Timer = "time_event@stop_timer";
                public const string Reset_Timer = "time_event@reset_timer";
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

            public static Dictionary<string, int> TimeIndexTable = new Dictionary<string, int>() {
                { TimeUI.Year, 0},
                { TimeUI.Month, 1},
                { TimeUI.Day, 2},
                { TimeUI.Hour, 3},
                { TimeUI.Minute, 4},
                { TimeUI.Second, 5},
                { TimeUI.Weekday, 6},
                { TimeUI.Subsecond, 7},
                { TimeUI.adjust_reason, 8},
                { TimeUI.Counting_mode, 9},

                { TimeUI.Time_display_mode, 10},
                { TimeUI.Other_Second, 11},
            };

        }
    }
}
