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
        private TMPro.TextMeshProUGUI titleText;

        [SerializeField]
        private TMPro.TextMeshProUGUI descriptionText;

        [SerializeField]
        private TMPro.TextMeshProUGUI yesBtnText;

        [SerializeField]
        private TMPro.TextMeshProUGUI noBtnText;

        [SerializeField]

        private Button ignoreBtn;

        [SerializeField]
        private Button resumeBtn;

        public void SetUp(string titleString, string descString, string yesBtnString, string noBtnString, System.Action resumeCallback) {
            descriptionText.text = descString;
            titleText.text = titleString;
            yesBtnText.text = yesBtnString;
            noBtnText.text = noBtnString;

            Hsinpa.Utility.UtilityFunc.SetSimpleBtnEvent(ignoreBtn, () =>
            {
                Modals.instance.Close();
            });

            Hsinpa.Utility.UtilityFunc.SetSimpleBtnEvent(resumeBtn, resumeCallback);
        }
    }
}