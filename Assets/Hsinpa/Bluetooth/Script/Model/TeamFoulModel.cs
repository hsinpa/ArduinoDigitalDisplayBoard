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

        public TeamFoulModel() { 
        
        }

        public void SetDefault() {
            Dipose();
        }

        public void SetPlayerValue(int team_id, int player_id, int p_value) {
            Dictionary<int, int> team_dict = (team_id == MessageEventFlag.Team_H) ? team_h_dict : team_g_dict;

            Hsinpa.Utility.UtilityFunc.SetDictionary<int, int>(team_dict, player_id, p_value);
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

        public void Dipose() {
            team_h_dict.Clear();
            team_g_dict.Clear();
        }

        public struct PlayerStruct {
            public int player_id;
            public int p_score;
        }
    }
}