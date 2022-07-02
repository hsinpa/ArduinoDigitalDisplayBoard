using ArduinoBluetoothAPI;
using SimpleEvent.ID;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Bluetooth
{
    public class DigitalBoardDataType
    {
        [System.Serializable]
        public class CharacterirticsData {
            private byte[] raw_data;
            public byte[] Data => raw_data;

            private Dictionary<string, int> index_table;
            private int length;

            public CharacterirticsData(int length, Dictionary<string, int> index_table) {
                this.length = length;
                this.raw_data = new byte[length];
                this.index_table = index_table;
            }

            public void Increment_Value(string key) {
                System.Int32 get_value = GetValue(key);
                Set_Value(key, get_value + 1);
            }

            public void Decrement_Value(string key)
            {
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
                }
            }

            public System.Int32 GetValue(string key) {
                if (index_table.TryGetValue(key, out int index)) {
                    return this.raw_data[index];
                }

                return 0;
            }

            public void DebugLog() {
                string str = "";
                for (int i = 0; i < this.length; i++) {
                    str += raw_data[i];
                }
                Debug.Log("convert_byte \n" + str);
            }

            public void Dispose() {
                this.raw_data = new byte[length];
            }
        }

        [System.Serializable]
        public struct UIDataStruct
        {
            public string id;
            public string category;
            public int value;
            public bool is_increment;
        }

        public struct BluetoothDataStruct
        {
            public BluetoothHelperCharacteristic characteristic;
            public byte[] data;
        }
    }
}