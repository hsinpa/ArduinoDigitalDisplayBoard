using System.Collections;
using System.Collections.Generic;
using Hsinpa.Bluetooth;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hsinpa.Bluetooth
{
    [CreateAssetMenu(fileName = "Data", menuName = "DigitalMessageSRP", order = 1)]
    public class DigitalMessageSRP : ScriptableObject
    {
        [SerializeField]
        private DigitalBoardDataType.UIDataStruct[] uiDataStructs = new DigitalBoardDataType.UIDataStruct[0];

        public DigitalBoardDataType.UIDataStruct[] UIDataStructs => uiDataStructs;

        [SerializeField]
        private DigitalBoardDataType.UIDataStruct[] uniqueDataStruct = new DigitalBoardDataType.UIDataStruct[0];

        private CancellationTokenSource _cancelSource;

        public async void Execute()
        {
            this._cancelSource = new CancellationTokenSource();

            int length = uiDataStructs.Length;
            for (int i = 0; i < length; i++)
            {
                if (uiDataStructs[i].id == SimpleEvent.ID.MessageEventFlag.HsinpaBluetoothEvent.UtilityCommand.Delay) {
                    Debug.Log("Process Delay");
                    await Task.Delay(uiDataStructs[i].value, this._cancelSource.Token);
                    Debug.Log("End Delay");
                    continue;
                }

                //DigitalBoardDataType.UIDataStruct uiDataStruct = new DigitalBoardDataType.UIDataStruct() { id = key, value = value, is_increment = is_increment };
                Utility.SimpleEventSystem.Send(uiDataStructs[i].category, uiDataStructs[i]);
            }
        }

        public int GetUniqueDataStructWithTable(string id)
        {
            int length = uniqueDataStruct.Length;

            for (int i = 0; i < length; i++)
            {
                if (uniqueDataStruct[i].id == id)
                    return uniqueDataStruct[i].value;
            }

            return -1;
        }

        public void Dispose() {
            if (this._cancelSource != null)
                this._cancelSource.Cancel();

            this._cancelSource = null;
        }

    }
}