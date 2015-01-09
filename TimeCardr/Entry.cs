using System;

namespace TimeCardr
{
	public class Entry
	{
		public DateTime Date { get; private set; }
		public string ResourceName { get; private set; }
		public string Project { get; private set; }
		public string Task { get; private set; }
		public int Hours { get; private set; }

		public Entry(DateTime date, string project, string task, int hours)
		{
			Date = date;
			ResourceName = "James Gerstmann";
			Project = project;
			Task = task;
			Hours = hours;
		}
	}
}
