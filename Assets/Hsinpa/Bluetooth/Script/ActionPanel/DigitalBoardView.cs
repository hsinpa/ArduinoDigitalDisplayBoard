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

        public void SetTitle(string title) {
            titleText.text = title.ToUpper();
        }
    }
}