﻿namespace Dragonfly.NetModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Object to hold Event Start and End Dates, and provide information about them
/// </summary>
public class EventDateTime
{
#pragma warning disable 1591

	private DateTime _startDate;
	private DateTime _endDate;

	public enum DateTimeStatus
	{
		Past,
		Current,
		Upcoming,
		Unknown,
		All,
		CurrentAndUpcoming,
		CurrentAndPast,
		AllExcludeUnknown
	}

	public DateTime StartDate
	{
		get { return _startDate; }
		set { _startDate = value; }
	}

	public DateTime EndDate
	{
		get { return _endDate; }
		set { _endDate = value; }
	}

	public bool HasStart
	{
		get { return _startDate != DateTime.MinValue; }
	}

	public bool HasEnd
	{
		get { return _endDate != DateTime.MinValue; }
	}

	public EventDateTime()
	{ }

	public EventDateTime(DateTime StartDate)
	{
		_startDate = StartDate;
	}

	public EventDateTime(DateTime StartDate, DateTime EndDate)
	{
		_startDate = StartDate;
		_endDate = EndDate;
	}

	/// <summary>
	/// Gets the status category Enum for the current date
	/// </summary>
	/// <returns></returns>
	public DateTimeStatus CurrentDateTimeStatus()
	{
		var today = DateTime.Today;
		var start = StartDate;
		var end = EndDate != DateTime.MinValue ? EndDate : StartDate;

		if (start == DateTime.MinValue)
		{
			return DateTimeStatus.Unknown;
		}

		var endPast = end < today;
		if (endPast)
		{
			return DateTimeStatus.Past;
		}

		var startUpcoming = start > today;
		if (startUpcoming)
		{
			return DateTimeStatus.Upcoming;
		}

		var startToday = start == today;

		if (startToday || !startUpcoming && !endPast)
		{
			return DateTimeStatus.Current;
		}

		//if we get here... something happened
		return DateTimeStatus.Unknown;
	}
}

