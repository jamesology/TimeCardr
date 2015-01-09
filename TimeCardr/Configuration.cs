using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace TimeCardr
{
	public class Configuration
	{
		public ICollection<Project> Projects { get; private set; }
		public ICollection<Task> Tasks { get; private set; }
		public string TimesheetFile { get; private set; }

		public Configuration()
		{
			Projects = new Collection<Project>();
			Tasks = new Collection<Task>();
			TimesheetFile = String.Empty;
		}

		public Configuration(ICollection<Project> projects, ICollection<Task> tasks, string timesheetFile)
		{
			Projects = projects;
			Tasks = tasks;
			TimesheetFile = timesheetFile;
		}
	}
}
