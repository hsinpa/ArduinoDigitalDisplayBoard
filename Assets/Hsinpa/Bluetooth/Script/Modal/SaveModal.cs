using Hsinpa.View;
using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.Bluetooth.View
{
    public class SaveModal : Modal
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI descriptionText;

        [SerializeField]

        private Button ignoreBtn;

        [SerializeField]
        private Button resumeBtn;

        public void SetUp(string sport_mode_name, long timestamp, System.Action resumeCallback) {
            var dateTimeOffset = System.DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime();
            string timeString = dateTimeOffset.ToString("g");

            string descString = string.Format(StaticText.Functions.SaveModalDescription, sport_mode_name, timeString);
            descriptionText.text = descString;

            Hsinpa.Utility.UtilityFunc.SetSimpleBtnEvent(ignoreBtn, () =>
            {
                Modals.instance.Close();
            });

            Hsinpa.Utility.UtilityFunc.SetSimpleBtnEvent(resumeBtn, resumeCallback);
        }
    }
}