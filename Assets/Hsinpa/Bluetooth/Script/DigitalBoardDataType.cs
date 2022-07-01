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
                    Debug.Log("convert_byte " + convert_byte);
                    this.raw_data[index] = convert_byte;
                }
            }

            public System.Int32 GetValue(string key) {
                if (index_table.TryGetValue(key, out int index)) {
                    return this.raw_data[index];
                }

                return 0;
            }

            public void Dispose() {
                this.raw_data = new byte[length];
            }
        }

        [System.Serializable]
        public struct TimeType
        {
            public System.Int32 Year;
            public System.Int32 Month;
            public System.Int32 Day;
            public System.Int32 Hour;
            public System.Int32 Minute;
            public System.Int32 Second;

            public System.Int32 Weekday;
            public System.Int32 Subsecond;

            public System.Int32 adjust_reason;
            public System.Int32 Counting_mode;

            public System.Int32 Time_display_mode;
            public System.Int32 Other_Second;
        }

        [System.Serializable]
        public struct OtherType
        {
            public System.Int32 Data1;///�|�P�Ҧ�1
            public System.Int32 Data2;//�|�P�Ҧ�2
            public System.Int32 Data3;//�|�P�Ҧ�3
            public System.Int32 Data4;//�|�P�Ҧ�4
            public System.Int32 FoulMode;//�x�y�ǳW�Ҧ�
            public System.Int32 player;//�ǳW���

            public System.Int32 Foul;//�ǳW����
            public System.Int32 Substitution;//���H�Ҧ�

            public System.Int32 team;//����
            public System.Int32 player2;//�W�����
            public System.Int32 player3;//�U�����

            public System.Int32 end;//�W�U��
            public System.Int32 OtherData1;//��LDATA
            public System.Int32 OtherData2;//��LDATA
        }
    }
}