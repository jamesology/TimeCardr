using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace TimeCardr
{
	public class Configuration
	{
		public IList<Project> Projects { get; private set; }
		public IList<Task> Tasks { get; private set; }
		public string TimesheetFile { get; private set; }

		public Configuration()
		{
			Projects = new Collection<Project>();
			Tasks = new Collection<Task>();
			TimesheetFile = String.Empty;
		}

		public Configuration(IList<Project> projects, IList<Task> tasks, string timesheetFile)
		{
			Projects = projects;
			Tasks = tasks;
			TimesheetFile = timesheetFile;
		}
	}
}
