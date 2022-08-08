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
        private TMPro.TextMeshProUGUI input_one_label;

        [SerializeField]
        private TMPro.TextMeshProUGUI input_two_label;

        [SerializeField]
        private TMPro.TMP_InputField input_one_field;
        private TMPro.TMP_InputField Input_One_Field => input_one_field;

        [SerializeField]
        private TMPro.TMP_InputField input_two_field;
        public TMPro.TMP_InputField Input_Two_Field => input_two_field;

        [SerializeField]
        private Button submitBtn;

        private System.Action<int> m_inputOneEditEvent;

        public delegate void FoulSubmitEvent(int player_id, int foul_count);
        private FoulSubmitEvent m_submitEvent;

        protected override void Start()
        {
            base.Start();

            input_one_field.onEndEdit.AddListener((string p_value) =>
            {
                if (int.TryParse(p_value, out int n_value) && m_inputOneEditEvent != null) {
                    m_inputOneEditEvent(n_value);
                }
            });

            Hsinpa.Utility.UtilityFunc.SetSimpleBtnEvent(submitBtn, OnSubmit);
        }

        public void SetText(string team_text, string label_1, string label_2) {
            team_name.text = team_text;//string.Format(StaticText.Functions.ExtraFoulTitle, (team_index == 0) ? "H " : "G");
            input_one_label.text = label_1;
            input_two_label.text = label_2;
        }

        public void SetUp(System.Action<int> playerIDEditEvent, FoulSubmitEvent submitEvent) {
            this.m_inputOneEditEvent = playerIDEditEvent;
            this.m_submitEvent = submitEvent;

            input_one_field.text = "";
            input_two_field.text = "";
        }

        private void OnSubmit() {
            if (int.TryParse(input_one_field.text, out int p_value) &&
                int.TryParse(input_two_field.text, out int f_value) && 
                m_submitEvent != null)
            {
                m_submitEvent(p_value, f_value);
                return;
            }

            m_submitEvent(-1, -1);
        }
    }
}