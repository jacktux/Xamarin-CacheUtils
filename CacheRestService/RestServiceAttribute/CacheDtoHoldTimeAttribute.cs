using System;
using System.Collections.Generic;
using System.Text;

namespace Ib.Xamarin.CacheUtils.RestServiceAttribute.CacheRestService
{
    public class CacheDtoHoldTimeAttribute : Attribute
    {
        private int _holdHours;
        private int _holdMinutes;
        private int _holdSeconds;

        public CacheDtoHoldTimeAttribute(int hours,int minutes, int seconds)
        {
            _holdHours = hours;
            _holdMinutes = minutes;
            _holdSeconds = seconds;
        }

        public TimeSpan CacheHoldTime
        {
            get
            {
                return new TimeSpan(_holdHours, _holdMinutes, _holdSeconds);
            }

        }
    }
}
