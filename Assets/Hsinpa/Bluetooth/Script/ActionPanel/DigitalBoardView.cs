using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hsinpa.Utility;
using SimpleEvent.ID;
using Hsinpa.Bluetooth.View;

namespace Hsinpa.Bluetooth
{
    public class DigitalBoardView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI titleText;

        [Header("Timer")]
        [SerializeField]
        private ActionTimerView action_timer;
        public ActionTimerView Action_Timer => action_timer;

        [Header("Timer")]
        [SerializeField]
        private ActionScoreView h_action_score;
        public ActionScoreView Action_Score_H => h_action_score;

        [SerializeField]
        private ActionScoreView g_action_score;
        public ActionScoreView Action_Score_G => g_action_score;

        public void SetTitle(string title) {
            titleText.text = title.ToUpper();
        }

        public void SetTVBMode() {
            action_timer.SetSyncTimeMode();
            h_action_score.SetTBVSportMode();
            g_action_score.SetTBVSportMode();

        }

        public void SetBasketballMode() {
            action_timer.SetBasketballTimeMode();
            h_action_score.SetBasketballSportMode();
            g_action_score.SetBasketballSportMode();
        }

    }
}