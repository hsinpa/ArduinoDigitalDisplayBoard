using ArduinoBluetoothAPI;
using SimpleEvent.ID;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Hsinpa.Bluetooth
{
    public class DigitalBoardDataType
    {
        [System.Serializable]
        public class CharacterirticsData {
            private byte[] raw_data;
            public byte[] Data => raw_data;

            public int[] IntData => this.raw_data.Select(x => (int)x).ToArray();


            private Dictionary<string, int> index_table;
            Dictionary<int, string> key_table;
            private int length;

            public System.Action<string, int> OnValueChange;

            private BluetoothHelperCharacteristic _bleCharacteristic;
            public BluetoothHelperCharacteristic BLECharacteristic => _bleCharacteristic;

            public CharacterirticsData(int length, BluetoothHelperCharacteristic bleCharacteristic, Dictionary<string, int> index_table, Dictionary<int, string> key_table) {
                this.length = length;
                this._bleCharacteristic = bleCharacteristic;
                this.raw_data = new byte[length];
                this.index_table = index_table;
                this.key_table = key_table;
            }

            public void Set_DataSet(int[] dataset) {
                if (dataset == null) return;

                for (int i = 0; i < dataset.Length; i++) {
                    Set_Raw_Value(i, dataset[i]);
                }
            }

            public void Increment_Value(string key, int max = -1) {
                if (max >= 0 && GetValue(key) >= max) {
                    return;
                }

                System.Int32 get_value = GetValue(key);
                Set_Value(key, get_value + 1);
            }

            public void Decrement_Value(string key, int min = 0)
            {
                if (min > 0 && GetValue(key) < min)
                {
                    return;
                }

                System.Int32 get_value = GetValue(key);
                Set_Value(key, get_value - 1);
            }

            public void Set_Value(string key, System.Int32 value)
            {
                if (index_table.TryGetValue(key, out int index))
                {
                    byte convert_byte = Convert.ToByte(value);
                    //Debug.Log("convert_byte " + convert_byte);
                    this.raw_data[index] = convert_byte;

                    if (OnValueChange != null)
                        OnValueChange(key, value);
                }
            }

            public System.Int32 GetValue(string key) {
                if (index_table.TryGetValue(key, out int index)) {
                    return this.raw_data[index];
                }

                return 0;
            }

            public void Set_Raw_Value(int index, System.Int32 value) {
                if (index >= 0 && index < length) {
                    this.raw_data[index] = Convert.ToByte(value);

                    if (this.key_table != null && OnValueChange != null && this.key_table.TryGetValue(index, out string key)) {
                        OnValueChange(key, value);
                    }
                }
            }

            public void DebugLog() {
                string str = "";
                for (int i = 0; i < this.length; i++) {
                    str += raw_data[i] +", ";
                }
                Debug.Log("convert_byte \n" + str);
            }

            public void Dispose() {
                this.raw_data = new byte[length];

                if (OnValueChange != null) {
                    foreach (var indexKeyPair in index_table) {
                        OnValueChange(indexKeyPair.Key, 0);
                    }
                }
            }
        }

        [System.Serializable]
        public struct UIDataStruct
        {
            public string id;
            public string category;
            public int value;
            public int max_value;
            public bool is_increment;
            public bool hide_bluetooth_event;
            public bool sync_struct_table;
            public bool exclusive;
        }

        public struct BluetoothDataStruct
        {
            public BluetoothHelperCharacteristic characteristic;
            public byte[] data;
        }
    }
}