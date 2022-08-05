using Hsinpa.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.Bluetooth.View
{
    public class ExtraFoulModal : Modal
    {
        [SerializeField]
        private InputField team_name;

        [SerializeField]
        private InputField playerID_field;

        [SerializeField]
        private InputField foul_count_field;

        [SerializeField]
        private Button submitBtn;

        private System.Action<int> m_playerIDEditEvent;
        private System.Action m_submitEvent;

        public void SetUp(int team_index, System.Action<int> playerIDEditEvent, System.Action submitEvent) {



            this.m_playerIDEditEvent = playerIDEditEvent;
            this.m_submitEvent = submitEvent;
        }

    }
}