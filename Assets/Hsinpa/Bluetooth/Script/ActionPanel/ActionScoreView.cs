using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.Bluetooth.View
{
    public class ActionScoreView : MonoBehaviour
    {
        [Header("Substitude Config")]
        [SerializeField]
        private Button substitude_btn;

        [Header("Foul Config")]
        [SerializeField]
        private Button foul_plus;

        [SerializeField]
        private Button foul_minus;

        [SerializeField]
        private DigitalTextValue foul_title;

        public void SetSoccer_HandballSportMode()
        {
            substitude_btn.gameObject.SetActive(true);
            foul_plus.gameObject.SetActive(false);
            foul_minus.gameObject.SetActive(false);
            foul_title.gameObject.SetActive(false);
        }

        public void SetTBVSportMode()
        {
            substitude_btn.gameObject.SetActive(false);
            foul_plus.gameObject.SetActive(true);
            foul_minus.gameObject.SetActive(true);
            foul_title.SetFieldFormat("Set<br>{" + foul_title.target_id + "}");
        }

        public void SetBasketballSportMode()
        {
            substitude_btn.gameObject.SetActive(false);
            foul_minus.gameObject.SetActive(false);
            foul_plus.gameObject.SetActive(true);
            foul_title.SetFieldFormat("Fouls<br>{" + foul_title.target_id + "}");
        }
    }
}