using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace TimeCardr
{
	public class Configuration
	{
		private const string DataFileName = "timeCardr.data";

		public readonly string ResourceName = "James Gerstmann";
		public IList<Project> Projects { get; private set; }
		public IList<Task> Tasks { get; private set; }
		public string OutputDirectory { get; private set; }
		public string ImportDirectory { get; private set; }
		public string DefaultDateFile { get; private set; }

		public string DataFile
		{
			get { return Path.Combine(OutputDirectory, DataFileName); }
		}

		public Configuration()
		{
			Projects = new Collection<Project>();
			Tasks = new Collection<Task>();
			OutputDirectory = String.Empty;
			ImportDirectory = string.Empty;
		}

		public Configuration(IList<Project> projects, IList<Task> tasks, string outputDirectory, string importDirectory, string defaultDateFile)
		{
			Projects = projects;
			Tasks = tasks;
			OutputDirectory = outputDirectory;
			ImportDirectory = importDirectory;
			DefaultDateFile = defaultDateFile;
		}
	}
}
