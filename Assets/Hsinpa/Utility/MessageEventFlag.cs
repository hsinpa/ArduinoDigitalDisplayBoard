using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleEvent.ID {
    public class MessageEventFlag : MonoBehaviour
    {

        public class Const {
            public const System.Int32 ScoreMode3 = 0x03;
            public const System.Int32 ScoreMode9 = 0x09;

            public const int BasketBallRoundMin = 600;//10 mins
            public const int Intermission_15Min = 900; //15 mins
            public const int Intermission_10Min = 600; //15 mins

            public const int SoccerRoundMin = 2700;//45 mins
            public const int HandballRoundMin = 1800;//30 mins

        }
        public const int MaxTurn = 7;
        public const int MaxFoul = 9;

        public const int Team_H = 0;
        public const int Team_G = 1;

        public static readonly byte[] ResetTimerCommand = new byte[12] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00 };

        public class PlayerPref {
            public const string Save = "pref@save";

            [System.Serializable]
            public struct PlayerStruct
            {
                public int player_id;
                public int p_score;
                public int team;
            }

            [System.Serializable]
            public struct FoulStruct
            {
                public List<PlayerStruct> players;
            }
        }

        public class HsinpaBluetoothEvent {

            public class SportMode {
                public const string Default = "sport_mode@default";
                //public const string DisplayWord = "sport_mode@display_word";

                public const string Basketball = "sport_mode@basketball";
                public const string Volleyball = "sport_mode@volleyball";
                public const string Badminton = "sport_mode@badminton";
                public const string TableTennis = "sport_mode@tabletennis";
                public const string Soccer = "sport_mode@soccer";
                public const string Handball = "sport_mode@handball";
                public const string GameRecord = "sport_mode@game_record";
            }

            public class UIEvent {
                public const string score = "event@ui_score";
                public const string time = "event@ui_time";
                public const string other = "event@ui_other";

                public const string functions = "event@functions";
                public const string ui_text = "event@ui_text_sync";
                public const string dispose = "event@dispose";

                public const string sport_mode_view = "event@ui_sport_mode";
                public const string word_display_mode_view = "event@ui_word_display_mode";
                public const string signal_testing_mode_view = "event@ui_signal_testing_mode";
                public const string digitalboard_mode_view = "event@ui_digitalboard_mode";
                public const string game_reset = "event@game_reset";
            }

            public class FunctionUI
            {
                public const string Next_Turn = "function@next_turn";

                public const string Short_TimeOut = "function@short_timeout";
                public const string Long_TimeOut = "function@long_timeout";
                public const string Intermission = "function@intermission";
                public const string Substitution = "function@substitution";
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


                //public const string CountDown14s = "time_event@count_down_14s";
                //public const string CountDown24s = "time_event@count_down_24s";


                public const string Sync_Time = "time_event@sync_real_time";
            }

            public class OtherUI
            {
                public const string Placard1 = "other@placard_1";
                public const string Placard2 = "other@placard_2";
                public const string Placard3 = "other@placard_3";
                public const string Placard4 = "other@placard_4";

                //Basketball
                public const string FoulMode = "other@foul_mode";
                public const string FoulPlayer = "other@foul_player";
                public const string FoulCount = "other@foul_count";
                public const string FoulTotalH = "other@foul_total_h";
                public const string FoulTotalG = "other@foul_total_g";

                //Soccer, Handball
                public const string Substitution = "other@substitution";
                public const string Team = "other@team";

                public const string End = "other@end";

                public const string OnCourtPlayer = "other@on_court_player";
                public const string OffCourtPlayer = "other@off_court_player";
            }

            public class UtilityCommand {
                public const string Delay = "command@delay";
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

            public static Dictionary<int, string> ScoreKeyTable = new Dictionary<int, string>() {
                {0, ScoreUI.H_score},
                {1, ScoreUI.G_score},
                {2, ScoreUI.H_foul},
                {3,  ScoreUI.G_foul},
                {4, ScoreUI.H_main_attack},
                {5,ScoreUI.G_main_attack},
                {6, ScoreUI.Sports_times},
                {7, ScoreUI.Mode},
                {8, ScoreUI.H_name },
                {9, ScoreUI.G_name},
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

            public static Dictionary<int, string> TimeKeyTable = new Dictionary<int, string>() {
                {0, TimeUI.Year},
                {1,TimeUI.Month},
                {2, TimeUI.Day},
                { 3,TimeUI.Hour},
                { 4,TimeUI.Minute},
                { 5,TimeUI.Second},
                { 6,TimeUI.Weekday},
                { 7,TimeUI.Subsecond},
                { 8,TimeUI.adjust_reason},
                { 9,TimeUI.Counting_mode},

                { 10,TimeUI.Time_display_mode},
                { 11,TimeUI.Other_Second},
            };

            public static Dictionary<string, int> OtherIndexTable = new Dictionary<string, int>() {
                { OtherUI.Placard1, 0},
                { OtherUI.Placard2, 1},
                { OtherUI.Placard3, 2},
                { OtherUI.Placard4, 3},

                { OtherUI.FoulMode, 4},
                { OtherUI.FoulPlayer, 5},
                { OtherUI.FoulCount, 6},

                { OtherUI.Substitution, 7},
                { OtherUI.Team, 8},
                { OtherUI.OnCourtPlayer, 9},
                { OtherUI.OffCourtPlayer, 10},

                { OtherUI.End, 11},

                { OtherUI.FoulTotalH, 12},
                { OtherUI.FoulTotalG, 13},
            };

            public static Dictionary<int, string> OtherKeyTable = new Dictionary<int, string>() {
                { 0,OtherUI.Placard1},
                {1, OtherUI.Placard2},
                { 2,OtherUI.Placard3},
                {3, OtherUI.Placard4},

                {4, OtherUI.FoulMode},
                {5, OtherUI.FoulPlayer},
                {6, OtherUI.FoulCount},

                { 7,OtherUI.Substitution},
                { 8,OtherUI.Team},
                {9, OtherUI.OnCourtPlayer},
                {10, OtherUI.OffCourtPlayer},

                {11, OtherUI.End},
                {12, OtherUI.FoulTotalH},
                {13, OtherUI.FoulTotalG},
            };

            public struct SportSettingStruct {
                public string id;
                public string title;
            }

            public struct SportSaveStruct {
                public string sport_id;
                public int[] scores;
                public int[] others;

                public int time_left_second;

                public long timestamp;
                public PlayerPref.FoulStruct foulStructs;

                public bool Is_Valid => !string.IsNullOrEmpty(sport_id);
            }


            public static Dictionary<string, SportSettingStruct> SportSettingTable = new Dictionary<string, SportSettingStruct>() {
                { SportMode.Default, new SportSettingStruct() { id = SportMode.Default, title = "Internal Testing" } },
                { SportMode.Basketball, new SportSettingStruct() {id = SportMode.Basketball, title = "Basketball"} },
                { SportMode.Volleyball, new SportSettingStruct() {id = SportMode.Volleyball, title = "Volleyball" } },
                { SportMode.Badminton, new SportSettingStruct() {id = SportMode.Badminton, title = "Badminton" } },
                { SportMode.TableTennis, new SportSettingStruct() {id = SportMode.TableTennis, title = "Table Tennis" } },
                { SportMode.Soccer, new SportSettingStruct() {id = SportMode.Soccer, title = "Soccer"} },
                { SportMode.Handball, new SportSettingStruct() {id = SportMode.Handball, title = "Handball" } },
                { SportMode.GameRecord, new SportSettingStruct() {id = SportMode.GameRecord, title = "Game Record" } },
            };
        }
    }
}
