using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Hsinpa.Utility;
using SimpleEvent.ID;

namespace Hsinpa.Bluetooth
{
    public class DigitalTextValue : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI tmproText;

        [SerializeField]
        private string field_format;

        public string target_id;

        [SerializeField]
        private List<DigitalBoardDataType.UIDataStruct> data;

        void Start()
        {
            SimpleEventSystem.CustomEventListener += OnSimpleEventSystem;

            UpdateDataValue(data);
        }

        public void SetFieldFormat(string p_format) { 
            this.field_format = p_format;
        } 

        private void OnSimpleEventSystem(string id, object[] values)
        {
            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.ui_text) {
                DigitalBoardDataType.UIDataStruct updateStruct = (DigitalBoardDataType.UIDataStruct) values[0];
                int findIndex = data.FindIndex(x => x.id == updateStruct.id);

                if (findIndex >= 0 && findIndex < data.Count) {
                    data[findIndex] = updateStruct;
                    UpdateDataValue(data);
                }

                return;
            }
        }

        private void UpdateDataValue(List<DigitalBoardDataType.UIDataStruct> dataStruct) {

            string prefix_text = field_format + "";
            foreach (DigitalBoardDataType.UIDataStruct data in dataStruct) {
                prefix_text = prefix_text.Replace("{"+ data.id +"}", data.value.ToString());
            }

            tmproText.text = prefix_text;
        }
    }
}