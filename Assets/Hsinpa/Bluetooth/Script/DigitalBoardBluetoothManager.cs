﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using UnityEngine.Android;
using Hsinpa.Utility;
using SimpleEvent.ID;
using Hsinpa.Bluetooth.View;
using Hsinpa.Bluetooth.SignalTesting;
using Hsinpa.View;
using System.Threading.Tasks;

namespace Hsinpa.Bluetooth
{

    public class DigitalBoardBluetoothManager : MonoBehaviour
    {
        [Header("UI View")]
        [SerializeField]
        private SignalTestScript signalTestScript;

        [SerializeField]
        private HintView hintModeView;

        [SerializeField]
        private SportGridLogic sportModeView;

        [SerializeField]
        private DigitalCharacterLogic digitalCharacterLogic;

        [SerializeField]
        private DigitalBoardView digitalBoardModeView;
        public DigitalBoardView DigitalBoardModeView => digitalBoardModeView;

        [SerializeField]
        private DigitalCharacterView digitalCharacterView;

        [Header("BLE")]
        [SerializeField]
        private DigitlaBoardLogicHandler digitlaBoardLogicHandler;

        [SerializeField]
        private DigitalBoardEventSender digitalBoardEventSender;

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

        private bool first_application_start_flag = true;

        void Start()
        {
            try
            {
                Hsinpa.Utility.SimpleEventSystem.Dispose();

                hintModeView.SetText(StaticText.Hint.SearchBLE);
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
                DetectSaveRecord();
            }

            digitlaBoardLogicHandler.SetUp();
            signalTestScript.SetUp(digitlaBoardLogicHandler.DigitalBoardEventSender, scoreCharacteristic, timeCharacteristic);

            Hsinpa.Utility.UtilityFunc.SetSimpleBtnEvent(sportModeView.DemoBtn, TriggerDemoEvent);
            Hsinpa.Utility.UtilityFunc.SetSimpleBtnEvent(sportModeView.ExitBtn, TriggerExitEvent);
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

            //helper.Subscribe(timeCharacteristic);
            //helper.Subscribe(otherCharacteristic);

            hintModeView.gameObject.SetActive(false);

            if (first_application_start_flag) {
                DetectSaveRecord();
                first_application_start_flag = false;
            }

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

            helper.ScanNearbyDevices();

            hintModeView.SetText(StaticText.Hint.ReconnectionBLE);
            hintModeView.gameObject.SetActive(true);
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
                digitalCharacterLogic.Dispose();

                sportModeView.gameObject.SetActive(true);
                digitalBoardModeView.gameObject.SetActive(false);
                digitalCharacterView.gameObject.SetActive(false);
                signalTestScript.gameObject.SetActive(false);
                digitlaBoardLogicHandler.Dispose();

                BLEReconnection.Dispose();
            }

            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.signal_testing_mode_view)
            {
                sportModeView.gameObject.SetActive(false);
                signalTestScript.gameObject.SetActive(true);
            }

            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.word_display_mode_view)
            {
                sportModeView.gameObject.SetActive(false);
                digitalCharacterView.gameObject.SetActive(true);
            }

            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.game_reset)
            {
                digitlaBoardLogicHandler.ResetDigitalBoard();
                digitlaBoardLogicHandler.SetSportStruct(digitlaBoardLogicHandler.SportSettingStruct);
            }
        }

        private void TriggerDemoEvent() {
            digitalBoardEventSender.SendBluetoothData(new DigitalBoardDataType.BluetoothDataStruct()
            {
                characteristic = timeCharacteristic,
                data = new byte[12] { 0, 0, 0, 0, 0, 0, 0, 12, 0, 3, 0, 0 }
            });

            digitalBoardEventSender.SendBluetoothData(new DigitalBoardDataType.BluetoothDataStruct() {
                characteristic = scoreCharacteristic,
                data = new byte[10] { 0, 0, 0, 0, 0, 0, 0, 12, 0, 0 }
            });
        }

        private void TriggerExitEvent()
        {
            var save_modal = Modals.instance.OpenModal<SaveModal>();

            save_modal.SetUp(StaticText.Functions.QuitModalTitle, StaticText.Functions.QuitModalDescription, StaticText.Functions.UniversalBtnYes, StaticText.Functions.UniversalBtnNo,
                async () => {
                if (helper != null && helper.Available)
                    helper.Disconnect();

                await Task.Delay(200);

                Application.Quit();
                Debug.Log("Application Quit");
            });
        }

        private void DetectSaveRecord() {
            var savefile = BLEReconnection.GetSaveFile();

            if (savefile.Is_Valid && MessageEventFlag.HsinpaBluetoothEvent.SportSettingTable.TryGetValue(savefile.sport_id, out var sportSettingStruct)) {
                var save_modal = Modals.instance.OpenModal<SaveModal>();

                var dateTimeOffset = System.DateTimeOffset.FromUnixTimeSeconds(savefile.timestamp).ToLocalTime();
                string timeString = dateTimeOffset.ToString("g");

                string descString = string.Format(StaticText.Functions.SaveModalDescription, sportSettingStruct.title, timeString);

                save_modal.SetUp(StaticText.Functions.SaveModalTitle, descString, StaticText.Functions.SaveModalBtnYes, StaticText.Functions.SaveModalBtnNo,
                    async () => {
                    Debug.Log("Save Resume " + savefile.sport_id);

                    Modals.instance.Close();

                    SimpleEventSystem.Send(MessageEventFlag.HsinpaBluetoothEvent.UIEvent.digitalboard_mode_view, savefile.sport_id);

                    await System.Threading.Tasks.Task.Delay(500);

                    digitlaBoardLogicHandler.ReimportModelData(savefile.scores, savefile.others, savefile.time_left_second, savefile.foulStructs);

                    digitlaBoardLogicHandler.ExecReconnectAction();
                });
            }

            BLEReconnection.Dispose();
        }

        private void OnApplicationQuit()
        {
            OnDestroy();
        }
    }
}