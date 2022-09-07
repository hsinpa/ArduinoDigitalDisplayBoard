using Hsinpa.Utility;
using SimpleEvent.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.Bluetooth.View
{
    public class ActionFunctionView : MonoBehaviour
    {
        [Header("Control Bar")]
        [SerializeField]
        private Button next_turn_btn;
        public Button Next_turn_btn => next_turn_btn;


        [SerializeField]
        private Button intermission_btn;

        [SerializeField]
        private Button short_timeout_btn;

        [SerializeField]
        private Button long_timeout_btn;


        public void SetTBVFuncMode()
        {
            Reset();
            next_turn_btn.gameObject.SetActive(true);
        }

        public void SetBasketballFuncMode()
        {
            Reset();

            next_turn_btn.gameObject.SetActive(true);
            intermission_btn.gameObject.SetActive(true);

            short_timeout_btn.gameObject.SetActive(true);
            long_timeout_btn.gameObject.SetActive(true);
        }

        public void SetSoccerHandballTimeMode()
        {
            Reset();

            next_turn_btn.gameObject.SetActive(true);
            intermission_btn.gameObject.SetActive(true);
        }

        public void Reset()
        {
            next_turn_btn.interactable = true;
            next_turn_btn.gameObject.SetActive(false);
            intermission_btn.gameObject.SetActive(false);
            long_timeout_btn.gameObject.SetActive(false);
            short_timeout_btn.gameObject.SetActive(false);
        }

    }
}