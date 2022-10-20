using ArduinoBluetoothAPI;
using Hsinpa.Bluetooth.Model;
using Hsinpa.Bluetooth.Sport;
using Hsinpa.Utility;
using SimpleEvent.ID;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Hsinpa.Bluetooth
{
    public class DigitlaBoardLogicHandler : MonoBehaviour
    {

        [SerializeField]
        private DigitalBoardBluetoothManager digitalBoardBluetoothManager;

        [SerializeField]
        private DigitalBoardEventSender digitalBoardEventSender;
        public DigitalBoardEventSender DigitalBoardEventSender => digitalBoardEventSender;

        [Header("SRP Config")]
        [SerializeField]
        private DigitalMessageSRP boostConfigSRP;

        [SerializeField]
        private DigitalMessageSRP TBV_ConfigSRP;

        [SerializeField]
        private DigitalMessageSRP Basketball_ConfigSRP;

        [SerializeField]
        private DigitalMessageSRP Soccer_ConfigSRP;

        [SerializeField]
        private DigitalMessageSRP Handball_ConfigSRP;

        private BLEDataModel _bleDataModel;

        private SportLogicFuncs _sportLogicFuncs;
        public SportLogicFuncs SportLogicFuncs => _sportLogicFuncs;

        private ISport _currentSport;

        private const float update_period = 1f;
        private float update_record = 0;

        public void SetSportStruct(MessageEventFlag.HsinpaBluetoothEvent.SportSettingStruct sportSettingStruct) {
            _currentSport = GetSport(sportSettingStruct.id);
            _currentSport.Setup(this, sportSettingStruct, this._bleDataModel, digitalBoardBluetoothManager.DigitalBoardModeView);

            _bleDataModel.TeamFoulModel.Dipose();
            _currentSport.Init();

            _currentSport.SRP.Execute();

            //await Task.Delay(500);
            //_currentSport.Init();
        }

        public void SetUp()
        {
            digitalBoardBluetoothManager.OnConnect += OnBluetoothConnect;
            digitalBoardBluetoothManager.OnDisconnect += OnBluetoothDisconnect;

            this._bleDataModel = new BLEDataModel(
                scoreType: new DigitalBoardDataType.CharacterirticsData(10, digitalBoardBluetoothManager.ScoreCharacteristic, MessageEventFlag.HsinpaBluetoothEvent.ScoreIndexTable, MessageEventFlag.HsinpaBluetoothEvent.ScoreKeyTable),
                timeType: new DigitalBoardDataType.CharacterirticsData(12, digitalBoardBluetoothManager.TimeCharacteristic,  MessageEventFlag.HsinpaBluetoothEvent.TimeIndexTable, MessageEventFlag.HsinpaBluetoothEvent.TimeKeyTable),
                otherType: new DigitalBoardDataType.CharacterirticsData(14, digitalBoardBluetoothManager.OtherCharacteristic, MessageEventFlag.HsinpaBluetoothEvent.OtherIndexTable, MessageEventFlag.HsinpaBluetoothEvent.OtherKeyTable)
            );

            this._sportLogicFuncs = new SportLogicFuncs(this, digitalBoardEventSender, digitalBoardBluetoothManager);

            this._bleDataModel.ScoreType.OnValueChange += OnInternalValueChange;
            this._bleDataModel.TimeType.OnValueChange += OnInternalValueChange;

            SimpleEventSystem.CustomEventListener += OnSimpleEventSystem;
        }

        public void ResetDigitalBoard()
        {
            Dispose();

            if (boostConfigSRP != null)
                boostConfigSRP.Execute();
        }

        private void Update()
        {
            if (Time.time >= update_record && this._bleDataModel != null)
            {
                if (this._bleDataModel.PrimaryTimer.TimerState)
                    this._bleDataModel.UpdateTime();

                //Debug.Log(p_digital_timer.GetHour());
                //Debug.Log(p_digital_timer.GetMinute());
                //Debug.Log(this._bleDataModel.SecondaryTimer.GetSecond());

                if (this._currentSport != null && this._currentSport.SportStruct.id == MessageEventFlag.HsinpaBluetoothEvent.SportMode.Default)
                    SyncTimeData();

                if (this._currentSport != null)
                    BLEReconnection.Save(this._currentSport.SportStruct.id, this._bleDataModel);

                update_record = Time.time + update_period;
            }
        }

        private void SyncTimeData() {

            DigitalBoardDataType.BluetoothDataStruct bluetoothDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
            {
                characteristic = digitalBoardBluetoothManager.TimeCharacteristic,
                data = this._bleDataModel.TimeType.Data.ToArray()
            };

            digitalBoardEventSender.SendBluetoothData(bluetoothDataStruct);
        }


        public async void SendUIDataStructBLE(
            DigitalBoardDataType.UIDataStruct[] uiDataStruct,
            DigitalBoardDataType.CharacterirticsData characteristic_data)
        {
            if (uiDataStruct == null) return;

            foreach (var uiData in uiDataStruct) {
                await Task.Yield();
                SendUIDataStructBLE(uiData, characteristic_data);
            }
        }

        public void SendUIDataStructBLE(DigitalBoardDataType.UIDataStruct uiDataStruct,
            DigitalBoardDataType.CharacterirticsData characteristic_data) {
            Debug.Log($"SendUIDataStructBLE Send : { uiDataStruct.id }, value ${uiDataStruct.value}");

            if (uiDataStruct.is_increment && uiDataStruct.value >= 0 &&
                uiDataStruct.max_value > 0 && uiDataStruct.max_value <= characteristic_data.GetValue(uiDataStruct.id))
            {
                Debug.Log($"SendUIDataStructBLE Warning : { uiDataStruct.id } Max value is reach");
                return;
            }

            if (uiDataStruct.is_increment)
            {
                if (uiDataStruct.value >= 0) {
                    characteristic_data.Increment_Value(uiDataStruct.id);
                }
                else if (uiDataStruct.value < 0 && characteristic_data.GetValue(uiDataStruct.id) > 0) {
                    characteristic_data.Decrement_Value(uiDataStruct.id);
                }
            }
            else
            {
                characteristic_data.Set_Value(uiDataStruct.id, uiDataStruct.value);
            }

            if (uiDataStruct.hide_bluetooth_event) return;

            DigitalBoardDataType.BluetoothDataStruct bluetoothDataStruct = new DigitalBoardDataType.BluetoothDataStruct()
            {
                characteristic = characteristic_data.BLECharacteristic,
                data = characteristic_data.Data.ToArray()
            };

            digitalBoardEventSender.SendBluetoothData(bluetoothDataStruct);
        }

        public void ReimportModelData(int[] score_data, int[] other_data, int target_time, MessageEventFlag.PlayerPref.FoulStruct foulStructs) {
            this._bleDataModel.ScoreType.Set_DataSet(score_data);
            this._bleDataModel.OtherType.Set_DataSet(other_data);
            this._bleDataModel.TeamFoulModel.ImportFoulStruct(foulStructs);
            this._bleDataModel.PrimaryTimer.SetExactTime(target_time);
        }

        public void ExecReconnectAction()
        {
            if (this._currentSport != null)
            {
                SportLogicFuncs.SendSimpleMessage(new DigitalBoardDataType.UIDataStruct()
                {
                    id = MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Start_Timer,
                    category = MessageEventFlag.HsinpaBluetoothEvent.UIEvent.time
                });

                SportLogicFuncs.SendSimpleMessage(new DigitalBoardDataType.UIDataStruct()
                {
                    id = MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Sync_Time,
                    category = MessageEventFlag.HsinpaBluetoothEvent.UIEvent.time
                });

                this._currentSport.ExecuteReconnectionActions();
            }
        }

        #region Event
        private void OnSimpleEventSystem(string id, object[] values) {
            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.score && values.Length > 0)
                OnScoreUIChange((DigitalBoardDataType.UIDataStruct)values[0]);

            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.time && values.Length > 0)
                OnTimerUIChange((DigitalBoardDataType.UIDataStruct)values[0]);

            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.functions && values.Length > 0)
            {
                OnFunctionUIChange((DigitalBoardDataType.UIDataStruct)values[0]);
            }

            if (id == MessageEventFlag.HsinpaBluetoothEvent.UIEvent.dispose) {
                Dispose();
            }
        }

        private void OnScoreUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct) {
            Debug.Log("OnScoreUIChange " + uiDataStruct.id);
            uiDataStruct = GetUniqueSportStruct(uiDataStruct);
            uiDataStruct.max_value = 127;

            if (uiDataStruct.exclusive) {
                var emptyCharSet = new DigitalBoardDataType.CharacterirticsData(10, digitalBoardBluetoothManager.ScoreCharacteristic,
                                                                                    MessageEventFlag.HsinpaBluetoothEvent.ScoreIndexTable, MessageEventFlag.HsinpaBluetoothEvent.ScoreKeyTable);
                SendUIDataStructBLE(uiDataStruct, emptyCharSet);
                return;
            }

            if (this._currentSport != null)
                this._currentSport.OnScoreUIChange(uiDataStruct);
        }

        private void OnTimerUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct) {
            uiDataStruct = GetUniqueSportStruct(uiDataStruct);

            switch (uiDataStruct.id) {

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Second:
                    this._bleDataModel.PrimaryTimer.SetRelativelyTimer(uiDataStruct.value);
                    this._bleDataModel.UpdateTime();

                    int time_mode = _currentSport.SRP.GetUniqueDataStructWithTable(MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Time_display_mode);

                    SportLogicFuncs.SendTimeEvent(_bleDataModel.TimeType, counting_mode: 3, time_mode: time_mode);
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Stop_Timer:
                    this._bleDataModel.PrimaryTimer.StopTimer();
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Reset_Timer:
                    this._bleDataModel.PrimaryTimer.ResetTimer();
                    this._bleDataModel.TimeType.Dispose();
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Other_Second:
                    SendUIDataStructBLE(uiDataStruct, this._bleDataModel.TimeType);
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Counting_mode:
                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Time_display_mode:
                    SendUIDataStructBLE(uiDataStruct, _bleDataModel.TimeType);
                    break;
            }

            if (this._currentSport != null)
                this._currentSport.OnTimerUIChange(uiDataStruct);
        }

        private void OnFunctionUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
            Debug.Log("OnFunctionUIChange " + uiDataStruct.id);

            if (this._currentSport != null)
                this._currentSport.OnFunctionUIChange(uiDataStruct);
        }

        private void OnInternalValueChange(string key, int value) {
            SimpleEventSystem.Send( MessageEventFlag.HsinpaBluetoothEvent.UIEvent.ui_text, 
                new DigitalBoardDataType.UIDataStruct() { 
                    id = key,
                    value = value
                }
            );
        }

        private void OnBluetoothConnect() {
            ExecReconnectAction();
        }

        private void OnBluetoothDisconnect()
        {
            //Stop timer when disconnect
            SportLogicFuncs.SendSimpleMessage(new DigitalBoardDataType.UIDataStruct()
            {
                id = MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Stop_Timer,
                category = MessageEventFlag.HsinpaBluetoothEvent.UIEvent.time
            });
        }

        private void OnDestroy()
        {
            SimpleEventSystem.CustomEventListener -= OnSimpleEventSystem;
            digitalBoardBluetoothManager.OnConnect -= OnBluetoothConnect;
            digitalBoardBluetoothManager.OnDisconnect -= OnBluetoothDisconnect;
        }

        public void Dispose()
        {
            this._bleDataModel.Dispose();
            this._bleDataModel.PrimaryTimer.ResetTimer();

            if (this._currentSport != null) {
                this._currentSport.SRP.Dispose();
                this._currentSport.Exist();
            }

            this._currentSport = null;

            SportLogicFuncs.CleanDigitalBoard(digitalBoardBluetoothManager, digitalBoardEventSender);
        }

        #endregion

    private DigitalBoardDataType.UIDataStruct GetUniqueSportStruct(DigitalBoardDataType.UIDataStruct uiDataStruct) {
        if (uiDataStruct.sync_struct_table)
        {
            int unique_value = this._currentSport.SRP.GetUniqueDataStructWithTable(uiDataStruct.id);
            if (unique_value >= 0) uiDataStruct.value = unique_value;
        }

            return uiDataStruct;
    }

        private ISport GetSport(string id) {
            switch (id) {
                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.Badminton:
                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.Volleyball:
                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.TableTennis:

                    var tbvSport = new TBVSport();
                        tbvSport.SetSportSRP(TBV_ConfigSRP);

                    return tbvSport;

                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.Basketball:
                    var basketballSport = new BasketballSport();
                        basketballSport.SetSportSRP(Basketball_ConfigSRP);

                    return basketballSport;

                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.Soccer:
                    var soccerSport = new SoccerSport();
                    soccerSport.SetSportSRP(Soccer_ConfigSRP);
                    return soccerSport;

                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.Handball:
                    var handballSport = new HandBallSport();
                    handballSport.SetSportSRP(Handball_ConfigSRP);
                    return handballSport;
            
            }

            return new TBVSport();
        }
    }
}
