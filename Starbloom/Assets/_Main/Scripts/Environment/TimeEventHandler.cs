using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEventHandler
{
	readonly WorldTime.ITimeEventHandler mTimeEvents;

	public TimeEventHandler(WorldTime.ITimeEventHandler _timeEvents)
	{
		mTimeEvents = _timeEvents;
	}

	private void Awake()
	{
		UpdateTimes(false);
	}

	private void LateUpdate()
	{
		UpdateTimes();
	}

	protected void UpdateTimes( bool _fireEvents = true )
	{
        Debug.Log("Replaced With Enviro. Fix later");

		//Tenkoku.Core.TenkokuModule timeModule = QuickFind.WeatherModule;
		//mTimeEvents.NotifyMinute(timeModule.currentMinute, _fireEvents);
		//mTimeEvents.NotifyHour(timeModule.currentHour, _fireEvents);
		//mTimeEvents.NotifyDay(timeModule.currentDay, _fireEvents);
		//mTimeEvents.NotifyMonth(timeModule.currentMonth, _fireEvents);
		//mTimeEvents.NotifyYear(timeModule.currentYear, _fireEvents);
	}
}