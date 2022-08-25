using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hsinpa.Bluetooth
{
    public class DigitalCharacterView : MonoBehaviour
    {
        [SerializeField]
        private DigitalCharacterTextField upperCharacterTxtField;
        public DigitalCharacterTextField UpperCharacterTxtField => upperCharacterTxtField;

        [SerializeField]
        private DigitalCharacterTextField lowerCharacterTxtField;
        public DigitalCharacterTextField LowerCharacterTxtField => lowerCharacterTxtField;

        [SerializeField]
        private Slider rotate_speed_slider;
        public Slider Rotate_speed_slider => rotate_speed_slider;

        [SerializeField]
        private TextMeshProUGUI rotate_slider_title;
        public TextMeshProUGUI Rotate_slider_title => rotate_slider_title;

        [SerializeField]
        private Toggle force_rotate_toggle;
        public Toggle Force_rotate_toggle => force_rotate_toggle;
    }
}