using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hsinpa.Bluetooth
{
    public class DigitalTimer
    {
        private System.DateTime start_datetime;

        private System.TimeSpan target_second;
        private System.TimeSpan leak_datetime;

        private bool timer_state = false;
        public bool TimerState => timer_state;

        public enum Type {RealTime, Timer_CountUp, Timer_CountDown };
        private Type time_type = Type.Timer_CountUp;

        public System.Action TimeUpEvent;

        public void SetTimeType(Type time_type) {
            this.time_type = time_type;
        }

        public void StartTimer(int target_second = 0) {
            this.target_second = System.TimeSpan.FromSeconds(target_second);

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
            if (time_type == Type.RealTime)
                return System.DateTime.Now.Hour;

            return GetTimeDifferent().Hours;
        }

        public System.Int32 GetMinute()
        {
            if (time_type == Type.RealTime)
                return System.DateTime.Now.Minute;

            return GetTimeDifferent().Minutes;
        }

        public System.Int32 GetSecond()
        {
            if (time_type == Type.RealTime)
                return System.DateTime.Now.Second;

            return GetTimeDifferent().Seconds;
        }

        private System.TimeSpan GetTimeDifferent()
        {
            var time_leap = (System.DateTime.UtcNow - start_datetime) + leak_datetime;

            if (time_type == Type.Timer_CountDown) {
                return (leak_datetime + this.target_second) - (time_leap);
            }

            return time_leap;
        }

        private void Dispose() {
            start_datetime = System.DateTime.MinValue;
            leak_datetime = System.TimeSpan.Zero;
            timer_state = false;
            target_second = System.TimeSpan.Zero;
        }
    }
}