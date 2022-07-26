using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using UnityEngine.Android;
using Hsinpa.Utility;
using SimpleEvent.ID;

namespace Hsinpa.Bluetooth
{

    public class DigitalBoardBluetoothManager : MonoBehaviour
    {
        [Header("UI View")]
        [SerializeField]
        private GameObject hintModeView;

        [SerializeField]
        private SportGridLogic sportModeView;

        [SerializeField]
        private DigitalBoardView digitalBoardModeView;

        [SerializeField]
        private DigitalCharacterView digitalCharacterView;

        [Header("BLE")]
        [SerializeField]
        private DigitlaBoardLogicHandler digitlaBoardLogicHandler;

        private BluetoothHelper helper;
        private LinkedList<BluetoothDevice> devices;

        private const string serviceUUID = "6344A90E-C885-4060-8A3C-C4D1D7534E05";
        private const string scoreCharacteristicUUID = "6344A90E-C885-4060-8A3C-C4D100000008";
        private const string timeCharacteristicUUID = "6344A90E-C885-4060-8A3C-C4D100000009";
        private const string otherCharacteristicUUID = "6344A90E-C885-4060-8A3C-C4D10000000A";
        private const string wordCharacteristicUUID = "6344A90E-C885-4060-8A3C-C4D10000000B";

        private BluetoothHelperCharacteristic scoreCharacteristic;
        public BluetoothHelperCharacteristic ScoreCharacteristic => scoreCharacteristic;

        private BluetoothHelperCharacteristic timeCharacteristic;
        public BluetoothHelperCharacteristic TimeCharacteristic => timeCharacteristic;

        private BluetoothHelperCharacteristic otherCharacteristic;
        public BluetoothHelperCharacteristic OtherCharacteristic => otherCharacteristic;

        private BluetoothHelperCharacteristic wordCharacteristic;
        public BluetoothHelperCharacteristic WordCharacteristic => wordCharacteristic;

        public System.Action OnConnect;
        public System.Action OnDisconnect;

        void Start()
        {
            try
            {
                hintModeView.gameObject.SetActive(true);

                SimpleEventSystem.CustomEventListener += OnSimpleEventSystem;
                BluetoothHelper.BLE = true;
                helper = BluetoothHelper.GetInstance("Scoreboard Remote One");
                helper.OnConnected += OnConnected;
                helper.OnConnectionFailed += OnConnectionFailed;
                helper.OnScanEnded += OnScanEnded;
                helper.OnDataReceived += OnDataReceived;
                helper.OnCharacteristicNotFound += OnCharacteristicNotFound;
                helper.OnServiceNotFound += OnServiceNotFound;

                helper.ScanNearbyDevices();

                Permission.RequestUserPermission(Permission.CoarseLocation);

                scoreCharacteristic = new BluetoothHelperCharacteristic(scoreCharacteristicUUID, serviceUUID);
                timeCharacteristic = new BluetoothHelperCharacteristic(timeCharacteristicUUID, serviceUUID);
                otherCharacteristic = new BluetoothHelperCharacteristic(otherCharacteristicUUID, serviceUUID);
                wordCharacteristic = new BluetoothHelperCharacteristic(wordCharacteristicUUID, serviceUUID);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                hintModeView.gameObject.SetActive(false);
            }

        }

        void OnDataReceived(BluetoothHelper helper)
        {
        }

        void OnScanEnded(BluetoothHelper helper, LinkedList<BluetoothDevice> devices)
        {
            this.devices = devices;

            foreach (BluetoothDevice device in this.devices)
            {
                Debug.Log("Name" + device.DeviceName);
                Debug.Log("DeviceAddress " + device.DeviceAddress);
                Debug.Log("=============");
            }

            if (helper.isDevicePaired())
                helper.Connect();
            else
                helper.ScanNearbyDevices();
        }

        void OnConnected(BluetoothHelper helper)
        {
            List<BluetoothHelperService> services = helper.getGattServices();
            foreach (BluetoothHelperService s in services)
            {
                Debug.Log($"Service : [{s.getName()}]");
                foreach (BluetoothHelperCharacteristic c in s.getCharacteristics())
                {
                    Debug.Log($"Characteristic : [{c.getName()}]");
                }
            }

            helper.Subscribe(timeCharacteristic);
            helper.Subscribe(otherCharacteristic);

            byte[] test_byte_event = new byte[10] {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00
            };

            byte[] test_byte_event2 = new byte[10] {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00
            };

            //byte[] test_byte_event3 = new byte[11] {
            //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01
            //};


            byte[] test_byte_event4 = new byte[12] {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x05, 0x00
            };

            byte[] test_byte_event5 = new byte[12] {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00
            };

            //byte[] test_byte_event = new byte[10] {
            //    0x09, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            //};
            //helper.WriteCharacteristic(scoreCharacteristic, test_byte_event);
            //helper.WriteCharacteristic(scoreCharacteristic, test_byte_event2);
            //helper.WriteCharacteristic(timeCharacteristic, test_byte_event3);
            //helper.WriteCharacteristic(timeCharacteristic, test_byte_event4);
            //helper.WriteCharacteristic(timeCharacteristic, test_byte_event5);

            hintModeView.gameObject.SetActive(false);

            if (OnConnect != null)
                OnConnect();
        }

        public void WriteToCharacteristics(BluetoothHelperCharacteristic characteristic, byte[] bytes) {
            if (helper == null) return;

            helper.WriteCharacteristic(characteristic, bytes);            
        }

        void OnConnectionFailed(BluetoothHelper helper)
        {
            Debug.Log("Connection lost");

            if (OnDisconnect != null)
                OnDisconnect();
        }


        void OnServiceNotFound(BluetoothHelper helper, string service)
        {
            Debug.Log($"Service [{service}] not found");
        }

        void OnCharacteristicNotFound(BluetoothHelper helper, string service, string characteristic)
        {
            Debug.Log($"Characteristic [{service}] of service [{service}] not found");
        }

        void OnDestroy()
        {
            if (helper == null) return;

            helper.OnScanEnded -= OnScanEnded;
            helper.OnConnected -= OnConnected;
            helper.OnConnectionFailed -= OnConnectionFailed;
            helper.OnCharacteristicNotFound -= OnCharacteristicNotFound;
            helper.OnServiceNotFound -= OnServiceNotFound;
            helper.Disconnect();
            helper = null;
        }

        private void OnSimpleEventSystem(string id, object[] values)
        {
            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.digitalboard_mode_view && values != null && values.Length > 0)
            {
                string sport_id = (string)values[0];

                if (MessageEventFlag.HsinpaBluetoothEvent.SportSettingTable.TryGetValue(sport_id, out var sportSettingStruct)) {
                    sportModeView.gameObject.SetActive(false);
                    digitalBoardModeView.gameObject.SetActive(true);
                    digitalBoardModeView.SetTitle(sportSettingStruct.title);
                    digitlaBoardLogicHandler.ResetDigitalBoard();

                    digitlaBoardLogicHandler.SetSportStruct(sportSettingStruct);
                }
            }

            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.sport_mode_view)
            {
                sportModeView.gameObject.SetActive(true);
                digitalBoardModeView.gameObject.SetActive(false);
                digitalCharacterView.gameObject.SetActive(false);
                digitlaBoardLogicHandler.Dispose();
            }

            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.word_display_mode_view)
            {
                sportModeView.gameObject.SetActive(false);
                digitalCharacterView.gameObject.SetActive(true);
            }
        }

        private void OnApplicationQuit()
        {
            OnDestroy();
        }
    }
}