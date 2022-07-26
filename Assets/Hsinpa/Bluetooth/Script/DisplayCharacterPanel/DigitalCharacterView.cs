using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }
}