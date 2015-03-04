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
		public string OutputDirectory { get; private set; }
		public string ImportDirectory { get; private set; }

		public Configuration()
		{
			Projects = new Collection<Project>();
			Tasks = new Collection<Task>();
			OutputDirectory = String.Empty;
			ImportDirectory = string.Empty;
		}

		public Configuration(IList<Project> projects, IList<Task> tasks, string outputDirectory, string importDirectory)
		{
			Projects = projects;
			Tasks = tasks;
			OutputDirectory = outputDirectory;
			ImportDirectory = importDirectory;
		}
	}
}
