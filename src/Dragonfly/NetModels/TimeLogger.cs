
namespace Dragonfly.NetModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

/// <summary>
/// Object to hold timing information for a process
/// </summary>
public class TimeLogger
{
#pragma warning disable 1591
	public Stopwatch MainTimer { get; }
	public List<TimeInterval> Intervals { get; }

	public TimeLogger(bool StartNow = true)
	{
		MainTimer = new Stopwatch();
		if (StartNow)
		{
			MainTimer.Start();
		}

		//Do Stuff
		//stopWatch.Stop();
		//TimeSpan ts = stopWatch.Elapsed;
	}

	public void StartMainTimer()
	{
		MainTimer.Start();
	}

	public void StopMainTimer()
	{
		MainTimer.Stop();
	}

	/// <summary>
	/// Begin a new interval (split)
	/// </summary>
	/// <param name="IntervalName"></param>
	/// <param name="IsLastInterval"></param>
	public void StartInterval(string IntervalName, bool IsLastInterval = false)
	{
		var tempName = DateTime.UtcNow.Ticks.ToString();
		var interval = new TimeInterval(tempName);
		if (!Intervals.Any())
		{
			interval.IsFirstInterval = true;
		}

		if (IsLastInterval)
		{
			interval.IsLastInterval = true;
		}

		//Check for unique name
		var matches = this.Intervals.Where(n => n.Name == IntervalName).ToList();
		if (!matches.Any())
		{
			interval.Name = IntervalName;
		}
		else
		{
			var increment = matches.Count() + 1;
			interval.Name = $"{IntervalName} - {increment}";
		}
	}

	/// <summary>
	/// End an interval
	/// </summary>
	/// <param name="IntervalName"></param>
	public void StopInterval(string IntervalName)
	{
		var matched = GetIntervalByName(IntervalName);
		if (matched != null)
		{
			matched.Timer.Stop();
		}
		else
		{
			//Throw error
			var message = $"Unable to Stop Interval '{IntervalName}'. No existing Interval with that name was found.";
			var newException = new Exception(message);
			throw newException;
		}
	}

	/// <summary>
	/// Retrieve an interval using its name
	/// </summary>
	/// <param name="IntervalName"></param>
	/// <returns></returns>
	public TimeInterval GetIntervalByName(string IntervalName)
	{
		var matches = this.Intervals.Where(n => n.Name == IntervalName);
		if (matches.Any())
		{
			return matches.First();
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Retrieve an interval using its ID
	/// </summary>
	/// <param name="Id"></param>
	/// <returns></returns>
	public TimeInterval GetIntervalById(Guid Id)
	{
		var matches = this.Intervals.Where(n => n.Id == Id).ToList();
		if (matches.Any())
		{
			return matches.First();
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Retrieve an interval by GUID
	/// </summary>
	/// <param name="Id"></param>
	/// <returns></returns>
	public TimeInterval GetIntervalById(string Id)
	{
		var guid = new Guid(Id);
		var matches = this.Intervals.Where(n => n.Id == guid).ToList();
		if (matches.Any())
		{
			return matches.First();
		}
		else
		{
			return null;
		}
	}

}

public class TimeInterval
{
	public Guid Id { get; }
	public bool IsFirstInterval { get; internal set; }
	public bool IsLastInterval { get; internal set; }
	public string Name { get; internal set; }
	public Stopwatch Timer { get; }
	//public int OrderNum { get; set;}

	public TimeInterval(string IntervalName)
	{
		Timer = new Stopwatch();
		Timer.Start();

		Id = new Guid();
		Name = IntervalName;
	}
}

