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

        [SerializeField]
        private Button intermission_btn;

        [SerializeField]
        private Button short_timeout_btn;

        [SerializeField]
        private Button long_timeout_btn;

        [Header("Small Tab Functions")]
        [SerializeField]
        private Button extra_foul_btn;

        public void SetSyncTimeMode()
        {
            Reset();
            next_turn_btn.gameObject.SetActive(true);
        }

        public void SetBasketballTimeMode()
        {
            Reset();

            next_turn_btn.gameObject.SetActive(true);
            intermission_btn.gameObject.SetActive(true);

            short_timeout_btn.gameObject.SetActive(true);
            long_timeout_btn.gameObject.SetActive(true);

            extra_foul_btn.gameObject.SetActive(true);
        }

        public void Reset()
        {
            next_turn_btn.gameObject.SetActive(false);
            intermission_btn.gameObject.SetActive(false);
            long_timeout_btn.gameObject.SetActive(false);
            short_timeout_btn.gameObject.SetActive(false);
            extra_foul_btn.gameObject.SetActive(false);
        }

    }
}