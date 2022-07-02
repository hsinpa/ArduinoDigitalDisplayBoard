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
        private DigitalBoardDataType.UIDataStruct[] uIDataStructs = new DigitalBoardDataType.UIDataStruct[0];

        public void Start()
        {
            Utility.UtilityFunc.SetSimpleBtnEvent(target, () => {

                int length = uIDataStructs.Length;
                for (int i = 0; i < length; i++) {
                    //DigitalBoardDataType.UIDataStruct uiDataStruct = new DigitalBoardDataType.UIDataStruct() { id = key, value = value, is_increment = is_increment };
                    Utility.SimpleEventSystem.Send(uIDataStructs[i].category, uIDataStructs[i]);
                }

            });
        }
    }
}