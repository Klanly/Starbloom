using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEventHandler : ITimeEventHandler
{
	private void Awake()
	{
		UpdateTimes(false);
	}

	private void LateUpdate()
	{
		UpdateTimes();
	}

	protected void UpdateTimes( bool _fireEvents = true)
	{
		Tenkoku.Core.TenkokuModule timeModule = QuickFind.WeatherModule;
		NotifyMinute(timeModule.currentMinute, _fireEvents);
		NotifyHour(timeModule.currentHour, _fireEvents);
		NotifyDay(timeModule.currentDay, _fireEvents);
		NotifyMonth(timeModule.currentMonth, _fireEvents);
		NotifyYear(timeModule.currentYear, _fireEvents);
	}
}