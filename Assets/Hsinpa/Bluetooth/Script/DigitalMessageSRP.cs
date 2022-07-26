using System.Collections;
using System.Collections.Generic;
using Hsinpa.Bluetooth;
using UnityEngine;
using System.Linq;

namespace Hsinpa.Bluetooth
{
    [CreateAssetMenu(fileName = "Data", menuName = "DigitalMessageSRP", order = 1)]
    public class DigitalMessageSRP : ScriptableObject
    {
        [SerializeField]
        private DigitalBoardDataType.UIDataStruct[] uiDataStructs = new DigitalBoardDataType.UIDataStruct[0];

        public DigitalBoardDataType.UIDataStruct[] UIDataStructs => uiDataStructs;

        [SerializeField]
        private string[] constraint_array = new string[0];
        public string[] Constraint_Array => constraint_array;

        public void Execute()
        {
            int length = uiDataStructs.Length;
            for (int i = 0; i < length; i++)
            {
                //DigitalBoardDataType.UIDataStruct uiDataStruct = new DigitalBoardDataType.UIDataStruct() { id = key, value = value, is_increment = is_increment };
                Utility.SimpleEventSystem.Send(uiDataStructs[i].category, uiDataStructs[i]);
            }
        }

        public bool ConstraintPass(string id) {
            if (constraint_array == null) return true;
            return constraint_array.Contains(id);
        }
    }
}