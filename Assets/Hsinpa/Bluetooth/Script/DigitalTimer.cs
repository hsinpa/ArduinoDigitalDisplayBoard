using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hsinpa.Bluetooth
{
    public class DigitalTimer
    {
        private System.DateTime start_datetime;
        private System.TimeSpan leak_datetime;

        private bool timer_state = false;
        public bool TimerState => timer_state;

        public DigitalTimer() {
            Dispose();
        }

        public void StartTimer() {
            start_datetime = System.DateTime.UtcNow;

            timer_state = true;
        }

        public void StopTimer() {
            if (!timer_state) return; 
            timer_state = false;

            leak_datetime = GetTimeDifferent();
        }

        public void ResetTimer()
        {
            Dispose();
        }

        public System.Int32 GetHour()
        {
            return GetTimeDifferent().Hours;
        }

        public System.Int32 GetMinute()
        {
            return GetTimeDifferent().Minutes;
        }

        public System.Int32 GetSecond()
        {
            return GetTimeDifferent().Seconds;
        }

        private System.TimeSpan GetTimeDifferent()
        {
            return (System.DateTime.UtcNow - start_datetime) + leak_datetime;
        }

        private void Dispose() {
            start_datetime = System.DateTime.MinValue;
            leak_datetime = System.TimeSpan.Zero;
            timer_state = false;
        }
    }
}