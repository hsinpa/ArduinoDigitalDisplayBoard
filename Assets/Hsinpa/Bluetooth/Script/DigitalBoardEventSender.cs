using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Bluetooth
{
    public class DigitalBoardEventSender : MonoBehaviour
    {
        [SerializeField]
        DigitalBoardBluetoothManager digitalBoardBluetoothManager;

        private bool is_connected;

        private Queue<DigitalBoardDataType.BluetoothDataStruct> dataStructQueue = new Queue<DigitalBoardDataType.BluetoothDataStruct>();

        private float record_time;
        private float perioid_delay = 0.1f;

        #region API
        public void SendBluetoothData(DigitalBoardDataType.BluetoothDataStruct dataStruct) {
            dataStructQueue.Enqueue(dataStruct);
        }

        #endregion

        private void Start()
        {
            digitalBoardBluetoothManager.OnConnect += OnBluetoothConnected;
            digitalBoardBluetoothManager.OnDisconnect += OnBluetoothDisconnected;

            is_connected = false;
        }

        private void Update()
        {
            if (!is_connected || dataStructQueue.Count <= 0 || record_time > Time.time) return;

            DigitalBoardDataType.BluetoothDataStruct uiDataStruct = dataStructQueue.Dequeue();

            digitalBoardBluetoothManager.WriteToCharacteristics(uiDataStruct.characteristic, uiDataStruct.data);

            record_time = Time.time + perioid_delay;
        }

        private void OnBluetoothConnected() {
            is_connected = true;
        }

        private void OnBluetoothDisconnected()
        {
            is_connected = false;
            Dispose();
        }

        private void Dispose() {
            is_connected = false;
            dataStructQueue.Clear();
        }
    }
}