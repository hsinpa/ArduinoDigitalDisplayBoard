using Hsinpa.Utility;
using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.Bluetooth
{
    public class SportGridLogic : MonoBehaviour
    {
        [SerializeField]
        private Button demoBtn;
        public Button DemoBtn => demoBtn;

        private SportGridBtn[] gridBtnArray;

        private void OnEnable()
        {
            gridBtnArray = this.GetComponentsInChildren<SportGridBtn>(includeInactive: true);
            RegisterBtnEvent(gridBtnArray);
        }

        private void RegisterBtnEvent(SportGridBtn[] sportGridBtns) {
            if (sportGridBtns == null) return;

            foreach (SportGridBtn btn in sportGridBtns) {
                btn.SetBtnAction(() =>
                {
                    Debug.Log("RegisterBtnEvent " + btn.name);
                    SimpleEventSystem.Send(MessageEventFlag.HsinpaBluetoothEvent.UIEvent.digitalboard_mode_view, btn.id);
                });
            }
        }
    }
}