using System;

namespace TimeCardr
{
	public class Entry : IEquatable<Entry>
	{
		public DateTime Date { get; private set; }
		public string ResourceName { get; private set; }
		public string Project { get; private set; }
		public string Task { get; private set; }
		public double Hours { get; private set; }

		public Entry(DateTime date, string resourceName, string project, string task, double hours)
		{
			Date = date;
			ResourceName = resourceName;
			Project = project;
			Task = task;
			Hours = hours;
		}

		public bool Equals(Entry other)
		{
			var result = (Date.Equals(other.Date)
			        && ResourceName.Equals(other.ResourceName)
			        && Project.Equals(other.Project)
			        && Task.Equals(other.Task)
			        && Hours.Equals(other.Hours));

			return result;
		}

		public override int GetHashCode()
		{
			var hashCode = Date.GetHashCode() + ResourceName.GetHashCode() + Project.GetHashCode() + Task.GetHashCode() + Hours.GetHashCode();
			return hashCode;
		}

		bool IEquatable<Entry>.Equals(Entry other)
		{
			return Equals(other);
		}
	}
}
