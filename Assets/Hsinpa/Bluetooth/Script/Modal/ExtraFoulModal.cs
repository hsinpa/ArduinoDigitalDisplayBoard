using Hsinpa.View;
using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.Bluetooth.View
{
    public class ExtraFoulModal : Modal
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI team_name;

        [SerializeField]
        private TMPro.TMP_InputField playerID_field;

        [SerializeField]
        private TMPro.TMP_InputField foul_count_field;
        public TMPro.TMP_InputField Foul_Count_Field => foul_count_field;

        [SerializeField]
        private Button submitBtn;

        private System.Action<int> m_playerIDEditEvent;

        public delegate void FoulSubmitEvent(int player_id, int foul_count);
        private FoulSubmitEvent m_submitEvent;

        protected override void Start()
        {
            base.Start();

            playerID_field.onEndEdit.AddListener((string p_value) =>
            {
                if (int.TryParse(p_value, out int n_value) && m_playerIDEditEvent != null) {
                    m_playerIDEditEvent(n_value);
                }
            });

            Hsinpa.Utility.UtilityFunc.SetSimpleBtnEvent(submitBtn, OnSubmit);
        }

        public void SetUp(int team_index, System.Action<int> playerIDEditEvent, FoulSubmitEvent submitEvent) {
            team_name.text = string.Format(StaticText.Functions.ExtraFoulTitle, (team_index == 0) ? "H " : "G");

            this.m_playerIDEditEvent = playerIDEditEvent;
            this.m_submitEvent = submitEvent;

            playerID_field.text = "";
            foul_count_field.text = "";
        }

        private void OnSubmit() {
            if (int.TryParse(playerID_field.text, out int p_value) &&
                int.TryParse(foul_count_field.text, out int f_value) && 
                m_submitEvent != null)
            {
                m_submitEvent(p_value, f_value);
                return;
            }

            m_submitEvent(-1, -1);
        }
    }
}