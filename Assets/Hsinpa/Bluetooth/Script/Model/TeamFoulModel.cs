using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleEvent.ID;

namespace Hsinpa.Bluetooth.Model
{
    public class TeamFoulModel 
    {
        public Dictionary<int, int> team_h_dict = new Dictionary<int, int>();
        public Dictionary<int, int> team_g_dict = new Dictionary<int, int>();

        private MessageEventFlag.PlayerPref.FoulStruct m_playersStruct;
        public MessageEventFlag.PlayerPref.FoulStruct playersStruct => m_playersStruct;

        public TeamFoulModel() {
            m_playersStruct = new MessageEventFlag.PlayerPref.FoulStruct()
            {
                players = new List<MessageEventFlag.PlayerPref.PlayerStruct>()
            };
        }

        public void SetDefault() {
            Dipose();
        }

        public void ImportFoulStruct(MessageEventFlag.PlayerPref.FoulStruct foulStruct) {
            if (m_playersStruct.players == null) return;

            Dipose();
            m_playersStruct = foulStruct;

            for (int i = 0; i < m_playersStruct.players.Count; i++) {
                var team_dict = (m_playersStruct.players[i].team == 0) ? team_h_dict : team_g_dict;
                Hsinpa.Utility.UtilityFunc.SetDictionary<int, int>(team_dict, m_playersStruct.players[i].player_id, m_playersStruct.players[i].p_score);
            }

            Utility.SimpleEventSystem.Send(MessageEventFlag.HsinpaBluetoothEvent.UIEvent.ui_text,
                new DigitalBoardDataType.UIDataStruct()
                {
                    id = MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.H_foul,
                    value = GetTotalFouls(0)
                }
            );

            Utility.SimpleEventSystem.Send(MessageEventFlag.HsinpaBluetoothEvent.UIEvent.ui_text,
                new DigitalBoardDataType.UIDataStruct()
                {
                    id = MessageEventFlag.HsinpaBluetoothEvent.ScoreUI.G_foul,
                    value = GetTotalFouls(1)
                }
            );
        }

        public void SetPlayerValue(int team_id, int player_id, int p_value) {
            Dictionary<int, int> team_dict = (team_id == MessageEventFlag.Team_H) ? team_h_dict : team_g_dict;

            Hsinpa.Utility.UtilityFunc.SetDictionary<int, int>(team_dict, player_id, p_value);

            int structsIndex = FindIndex(team_id, player_id);

            var playerStruct = new MessageEventFlag.PlayerPref.PlayerStruct() { player_id = player_id, team = team_id, p_score = p_value };
            if (structsIndex < 0)
                m_playersStruct.players.Add(playerStruct);
            else
                m_playersStruct.players[structsIndex] = playerStruct;
        }

        public int GetFouls(int team_id, int player_id)
        {
            var dict = (team_id == MessageEventFlag.Team_H) ? team_h_dict : team_g_dict;

            if (dict.TryGetValue(player_id, out int p_value))
                return p_value;

            return 0;
        }

        public int GetTotalFouls(int team_id)
        {
            var values = (team_id == MessageEventFlag.Team_H) ? team_h_dict.Values : team_g_dict.Values;
            int accumulate = 0;
            foreach (int v in values) {
                accumulate += v;
            }
            return accumulate;
        }

        private int FindIndex(int team_id, int player_id) {
           return m_playersStruct.players.FindIndex(x=>x.team == team_id && x.player_id == player_id);
        }

        public void Dipose() {
            team_h_dict.Clear();
            team_g_dict.Clear();
            m_playersStruct.players.Clear();
        }
    }
}