using System;
using System.Collections.Generic;
using log4net;

namespace TimeCardr
{
	public class Configuration
	{
		public ICollection<Project> Projects { get; private set; }
		public ICollection<Task> Tasks { get; private set; }
		public string TimesheetFile { get; private set; }

		public static Configuration Initialize(IEnumerable<string> arguments, ILog log)
		{
			log.Error("Configuration.Initialize not implemented.");
			throw new NotImplementedException();
		}
	}
}
