using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hsinpa.Utility;
using SimpleEvent.ID;

namespace Hsinpa.Bluetooth
{
    public class DigitalBoardView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI titleText;

        [Header("Timer")]
        [SerializeField]
        private Button start_timer;

        [SerializeField]
        private Button stop_timer;

        [SerializeField]
        private Button reset_timer;

        [SerializeField]
        private Button sync_timer;

        private void Start()
        {
            SimpleEventSystem.CustomEventListener += OnSimpleEventSystem;
        }

        private void OnDisable()
        {
            start_timer.interactable = true;
        }

        private void OnSimpleEventSystem(string id, object[] values)
        {
            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.time && values.Length > 0)
                OnTimerUIChange((DigitalBoardDataType.UIDataStruct)values[0]);
        }


        private void OnTimerUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {

            switch (uiDataStruct.id)
            {
                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Start_Timer:
                    start_timer.interactable = false;
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Stop_Timer:
                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Reset_Timer:
                    start_timer.interactable = true;
                    break;
            }
        }

        public void SetTitle(string title) {
            titleText.text = title.ToUpper();
        }

        public void SetSyncTimeMode(bool enter_sync_time_mode) {
            start_timer.gameObject.SetActive(!enter_sync_time_mode);
            stop_timer.gameObject.SetActive(!enter_sync_time_mode);
            reset_timer.gameObject.SetActive(!enter_sync_time_mode);
            sync_timer.gameObject.SetActive(enter_sync_time_mode);
        }
    }
}