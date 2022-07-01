using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.Bluetooth
{
    public class DigitalBoardUI : MonoBehaviour
    {
        [SerializeField]
        private Button target;

        [SerializeField]
        private int value;

        [SerializeField]
        private bool is_increment;

        [SerializeField]
        private string key;

        [SerializeField]
        private string category;

        public void Start()
        {
            Utility.UtilityFunc.SetSimpleBtnEvent(target, () => {
                UIDataStruct uiDataStruct = new UIDataStruct() { id = key, value = value, is_increment = is_increment};
                Utility.SimpleEventSystem.Send(category, uiDataStruct);
            });
        }

        [System.Serializable]
        public struct UIDataStruct {
            public string id;
            public int value;
            public bool is_increment;

        }

    }
}