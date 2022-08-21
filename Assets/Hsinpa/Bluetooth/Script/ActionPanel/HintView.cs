using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Hsinpa.Bluetooth.View
{
    public class HintView : MonoBehaviour
    {
        public TextMeshProUGUI tmpro_text;

        public void SetText(string p_text) {
            tmpro_text.text = p_text;
        }
    }
}