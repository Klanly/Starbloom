using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITimeEventHandler : MonoBehaviour
{
	public event Action<int> OnMinute;
	public event Action<int> OnHour;
	public event Action<int> OnDay;
	public event Action<int> OnMonth;
	public event Action<int> OnYear;

	public int Minute { get; protected set; }
	public int Hour { get; protected set; }
	public int Day { get; protected set; }
	public int Month { get; protected set; }
	public int Year { get; protected set; }

	protected void NotifyMinute(int _minute, bool _fireEvent = true)
	{
		if (_fireEvent && null != OnMinute && Minute != _minute)
			OnMinute(_minute);

		Minute = _minute;
	}

	protected void NotifyHour(int _hour, bool _fireEvent = true)
	{
		if (_fireEvent && null != OnHour && Hour != _hour)
			OnHour(_hour);

		Hour = _hour;
	}

	protected void NotifyDay(int _day, bool _fireEvent = true)
	{
		if (_fireEvent && null != OnDay && Day != _day)
			OnDay(_day);

		Day = _day;
	}

	protected void NotifyMonth(int _month, bool _fireEvent = true)
	{
		if (_fireEvent && null != OnMonth && Month != _month)
			OnMonth(_month);

		Month = _month;
	}

	protected void NotifyYear(int _year, bool _fireEvent = true)
	{
		if (_fireEvent && null != OnYear && Year != _year)
			OnYear(_year);

		Year = _year;
	}
}