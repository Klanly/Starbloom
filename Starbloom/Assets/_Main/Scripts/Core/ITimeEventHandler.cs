using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

namespace WorldTime
{
	public class OnMinuteSignal : Signal<OnMinuteSignal, int> { }
	public class OnHourSignal : Signal<OnHourSignal, int> { }
	public class OnDaySignal : Signal<OnDaySignal, int> { }
	public class OnMonthSignal : Signal<OnMonthSignal, int> { }
	public class OnYearSignal : Signal<OnYearSignal, int> { }

	public interface ITimeEventHandler
	{
		int Minute { get; }
		int Hour { get; }
		int Day { get; }
		int Month { get; }
		int Year { get; }

		void NotifyMinute(int _minute, bool _fireEvent = true);
		void NotifyHour(int _hour, bool _fireEvent = true);
		void NotifyDay(int _day, bool _fireEvent = true);
		void NotifyMonth(int _month, bool _fireEvent = true);
		void NotifyYear(int _year, bool _fireEvent = true);
	}

	public class DefaultTimeEventHandler : ITimeEventHandler
	{
		readonly OnMinuteSignal OnMinute;
		readonly OnHourSignal OnHour;
		readonly OnDaySignal OnDay;
		readonly OnMonthSignal OnMonth;
		readonly OnYearSignal OnYear;

		public int Minute { get; protected set; }
		public int Hour { get; protected set; }
		public int Day { get; protected set; }
		public int Month { get; protected set; }
		public int Year { get; protected set; }

		public DefaultTimeEventHandler(OnMinuteSignal _onMinute, OnHourSignal _onHour, OnDaySignal _onDay, OnMonthSignal _onMonth, OnYearSignal _onYear)
		{
			OnMinute = _onMinute;
			OnHour = _onHour;
			OnDay = _onDay;
			OnMonth = _onMonth;
			OnYear = _onYear;
		}

		public void NotifyMinute(int _minute, bool _fireEvent = true)
		{
			if (_fireEvent && Minute != _minute)
				OnMinute.Fire(_minute);

			Minute = _minute;
		}

		public void NotifyHour(int _hour, bool _fireEvent = true)
		{
			if (_fireEvent && Hour != _hour)
				OnHour.Fire(_hour);

			Hour = _hour;
		}

		public void NotifyDay(int _day, bool _fireEvent = true)
		{
			if (_fireEvent && Day != _day)
				OnDay.Fire(_day);

			Day = _day;
		}

		public void NotifyMonth(int _month, bool _fireEvent = true)
		{
			if (_fireEvent && Month != _month)
				OnMonth.Fire(_month);

			Month = _month;
		}

		public void NotifyYear(int _year, bool _fireEvent = true)
		{
			if (_fireEvent && Year != _year)
				OnYear.Fire(_year);

			Year = _year;
		}
	}
}