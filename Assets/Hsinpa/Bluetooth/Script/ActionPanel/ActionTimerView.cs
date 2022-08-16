using Hsinpa.Utility;
using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hsinpa.Bluetooth.View { 
    public class ActionTimerView : MonoBehaviour
    {

        [Header("Control Bar")]
        [SerializeField]
        private Button start_timer;
        public Button Start_Timer => start_timer;

        [SerializeField]
        private Button stop_timer;

        [SerializeField]
        private Button reset_timer;

        [SerializeField]
        private Button sync_timer;

        [SerializeField]
        private GameObject minutes_panel;

        [SerializeField]
        private GameObject seconds_panel;

        [Header("Count Down")]
        [SerializeField]
        private Button countdown_14_btn;

        [SerializeField]
        private Button countdown_24_btn;

        [Header("Text")]
        [SerializeField]
        private TextMeshProUGUI primary_time_text;

        [SerializeField]
        private TextMeshProUGUI secondary_time_text;

        public void SetPrimaryTimeTxt(string txt) {
            primary_time_text.text = txt;
        }

        public void SetSecondaryTimeTxt(string txt)
        {
            secondary_time_text.text = txt;
        }

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

        public void SetSyncTimeMode()
        {
            Reset();
            sync_timer.gameObject.SetActive(true);
        }

        public void SetBasketballTimeMode()
        {
            Reset();
            start_timer.interactable = false;
            start_timer.gameObject.SetActive(true);
            stop_timer.gameObject.SetActive(true);

            countdown_14_btn.gameObject.SetActive(true);
            countdown_24_btn.gameObject.SetActive(true);

            minutes_panel.gameObject.SetActive(true);
            seconds_panel.gameObject.SetActive(true);

        }

        public void SetSoccerHandballTimeMode()
        {
            Reset();

            start_timer.interactable = false;
            start_timer.gameObject.SetActive(true);
            stop_timer.gameObject.SetActive(true);

            minutes_panel.gameObject.SetActive(true);
            seconds_panel.gameObject.SetActive(true);
        }

        public void Reset()
        {
            start_timer.gameObject.SetActive(false);
            stop_timer.gameObject.SetActive(false);
            reset_timer.gameObject.SetActive(false);
            sync_timer.gameObject.SetActive(false);
            countdown_14_btn.gameObject.SetActive(false);
            countdown_24_btn.gameObject.SetActive(false);

            minutes_panel.gameObject.SetActive(false);
            seconds_panel.gameObject.SetActive(false);

            primary_time_text.text = "";
            secondary_time_text.text = "";
        }
    }
}