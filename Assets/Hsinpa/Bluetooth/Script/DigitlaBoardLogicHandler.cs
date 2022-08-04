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

        [SerializeField]
        private DigitalMessageSRP boostConfigSRP;

        [SerializeField]
        private DigitalMessageSRP TBV_ConfigSRP;

        private BLEDataModel _bleDataModel;

        private SportLogicFuncs _sportLogicFuncs;
        public SportLogicFuncs SportLogicFuncs => _sportLogicFuncs;

        private ISport _currentSport;

        private const float update_period = 1f;
        private float update_record = 0;

        public void SetSportStruct(MessageEventFlag.HsinpaBluetoothEvent.SportSettingStruct sportSettingStruct) {
            _currentSport = GetSport(sportSettingStruct.id);
            _currentSport.Setup(this, sportSettingStruct, this._bleDataModel, digitalBoardBluetoothManager.DigitalBoardModeView);
            _currentSport.Init();
        }

        public void ResetDigitalBoard()
        {
            Dispose();
            OnBluetoothConnect();
        }

        private void Awake()
        {
            //digitalBoardBluetoothManager.OnConnect += OnBluetoothConnect;
            Hsinpa.Utility.SimpleEventSystem.Dispose();

            this._bleDataModel = new BLEDataModel(
                scoreType: new DigitalBoardDataType.CharacterirticsData(10, digitalBoardBluetoothManager.ScoreCharacteristic, MessageEventFlag.HsinpaBluetoothEvent.ScoreIndexTable),
                timeType: new DigitalBoardDataType.CharacterirticsData(12, digitalBoardBluetoothManager.TimeCharacteristic,  MessageEventFlag.HsinpaBluetoothEvent.TimeIndexTable),
                otherType: new DigitalBoardDataType.CharacterirticsData(14, digitalBoardBluetoothManager.OtherCharacteristic, MessageEventFlag.HsinpaBluetoothEvent.OtherIndexTable)
            );

            this._sportLogicFuncs = new SportLogicFuncs(this, digitalBoardEventSender, digitalBoardBluetoothManager);

            this._bleDataModel.ScoreType.OnValueChange += OnInternalValueChange;
            this._bleDataModel.TimeType.OnValueChange += OnInternalValueChange;
        }

        private void Start()
        {
            SimpleEventSystem.CustomEventListener += OnSimpleEventSystem;
            this._bleDataModel.PrimaryTimer.StartTimer();
            this._bleDataModel.SecondaryTimer.StartTimer(target_second: 30);
        }

        private void Update()
        {
            if (Time.time >= update_record)
            {
                this._bleDataModel.UpdateTime();

                //Debug.Log(p_digital_timer.GetHour());
                //Debug.Log(p_digital_timer.GetMinute());
                //Debug.Log(this._bleDataModel.SecondaryTimer.GetSecond());

                if (this._currentSport != null && this._currentSport.SportStruct.id == MessageEventFlag.HsinpaBluetoothEvent.SportMode.Default)
                    SyncTimeData();

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

            if (uiDataStruct.max_value > 0 && uiDataStruct.max_value <= characteristic_data.GetValue(uiDataStruct.id)) {
                Debug.Log($"SendUIDataStructBLE Warning : { uiDataStruct.id } Max value is reach");
                return;
            }
            
            if (uiDataStruct.is_increment)
            {
                if (uiDataStruct.value >= 0)
                    characteristic_data.Increment_Value(uiDataStruct.id);
                else
                    characteristic_data.Decrement_Value(uiDataStruct.id);
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


            if (uiDataStruct.exclusive) {
                var emptyCharSet = new DigitalBoardDataType.CharacterirticsData(10, digitalBoardBluetoothManager.ScoreCharacteristic, MessageEventFlag.HsinpaBluetoothEvent.ScoreIndexTable);
                SendUIDataStructBLE(uiDataStruct, emptyCharSet);
                return;
            }

            SendUIDataStructBLE(uiDataStruct, this._bleDataModel.ScoreType);
        }

        private void OnTimerUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct) {
            Debug.Log("OnTimerUIChange " + uiDataStruct.id +", value " + uiDataStruct.value);

            switch (uiDataStruct.id) {
                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Start_Timer:
                    this._bleDataModel.PrimaryTimer.SetTimeType(DigitalTimer.Type.Timer_CountUp);
                    this._bleDataModel.PrimaryTimer.StartTimer();
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Stop_Timer:
                    this._bleDataModel.PrimaryTimer.StopTimer();
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.TimeUI.Reset_Timer:
                    this._bleDataModel.PrimaryTimer.ResetTimer();
                    this._bleDataModel.TimeType.Dispose();
                    break;
            }

            if (this._currentSport != null)
                this._currentSport.OnTimerUIChange(uiDataStruct);
        }

        private void OnFunctionUIChange(DigitalBoardDataType.UIDataStruct uiDataStruct)
        {
            Debug.Log("OnFunctionUIChange " + uiDataStruct.id);

            switch (uiDataStruct.id)
            {
                case MessageEventFlag.HsinpaBluetoothEvent.FunctionUI.Next_Turn:
                    _sportLogicFuncs.NextTurn(this._bleDataModel.ScoreType, this._bleDataModel.TimeType);
                    break;
            }
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
            if (boostConfigSRP != null)
                boostConfigSRP.Execute();
        }

        private void OnDestroy()
        {
            SimpleEventSystem.CustomEventListener -= OnSimpleEventSystem;
            digitalBoardBluetoothManager.OnConnect -= OnBluetoothConnect;
        }

        public void Dispose()
        {
            this._bleDataModel.Dispose();
            this._bleDataModel.PrimaryTimer.ResetTimer();

            if (this._currentSport != null)
                this._currentSport.Exist();

            SportLogicFuncs.CleanDigitalBoard(digitalBoardBluetoothManager, digitalBoardEventSender);
        }

        #endregion

        private ISport GetSport(string id) {
            switch (id) {
                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.Badminton:
                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.Volleyball:
                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.TableTennis:

                    var tbvSport = new TBVSport();
                        tbvSport.SetSportSRP(TBV_ConfigSRP);

                    return tbvSport;

                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.Basketball:
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.Soccer:
                    break;

                case MessageEventFlag.HsinpaBluetoothEvent.SportMode.Handball:
                    break;
            
            }

            return new TBVSport();
        }
    }
}
