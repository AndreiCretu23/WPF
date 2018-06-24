using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Utils
{
    public enum DateTimeReference
    {
        Milliseconds,
        Seconds,
        Minutes,
        Hours,
        Days,
    }

    public static class DateTimeUtils
    {
        public static TimeSpan GetTimeDifference(DateTime dateTime1, DateTime dateTime2)
        {
            TimeSpan timeSpan;
            if (dateTime1.CompareTo(dateTime2) > 0)
            {
                timeSpan = dateTime1 - dateTime2;
            }
            else
            {
                timeSpan = dateTime2 - dateTime1;
            }

            return timeSpan;
        }

        public static bool IsTimeDifferenceGreaterThan(DateTime dateTime1, DateTime dateTime2, DateTimeReference dateTimeReference, double value)
        {
            TimeSpan timeSpan = GetTimeDifference(dateTime1, dateTime2);

            switch (dateTimeReference)
            {
                case DateTimeReference.Milliseconds: return timeSpan.TotalMilliseconds > value;
                case DateTimeReference.Seconds: return timeSpan.TotalSeconds > value;
                case DateTimeReference.Minutes: return timeSpan.TotalMinutes > value;
                case DateTimeReference.Hours: return timeSpan.TotalHours > value;
                case DateTimeReference.Days: return timeSpan.TotalDays > value;
                default: throw new Exception("Error : Unexpected DateTimeReference"); // This can never happen
            }
        }
    }
}
