using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Newtonsoft.Json;

namespace TimeCardr
{
	public class Configurator
	{
		private enum Property
		{
			SettingsFile,
			ProjectsFile,
			TasksFile,
			OutputDirectory,
			ImportDirectory,
			DefaultDateFile
		}

		private static readonly IDictionary<string, Property> Properties = new Dictionary<string, Property>
		{
			{"s", Property.SettingsFile},
			{"settings", Property.SettingsFile},
			{"p", Property.ProjectsFile},
			{"projects", Property.ProjectsFile},
			{"t", Property.TasksFile},
			{"tasks", Property.TasksFile},
			{"o", Property.OutputDirectory},
			{"output", Property.OutputDirectory},
			{"i", Property.ImportDirectory},
			{"import", Property.ImportDirectory},
			{"d", Property.DefaultDateFile},
			{"default", Property.DefaultDateFile},
		};

		public static Configuration Initialize(IEnumerable<string> arguments, ILog log)
		{
			var result = new Configuration();
			foreach (var argument in arguments.Where(x => String.IsNullOrWhiteSpace(x) == false))
			{
				log.Debug(argument);
				var pair = MakeKeyValue(argument);

				var property = Properties[pair.Key];

				switch (property)
				{
					case Property.SettingsFile:
						if (File.Exists(pair.Value))
						{
							var settings = ReadSettingsFromFile(pair.Value);
							result = Initialize(settings, log);
						}
						else
						{
							throw new FileNotFoundException(pair.Value);
						}
						break;
					case Property.ProjectsFile:
						if (File.Exists(pair.Value))
						{
							var projects = ReadProjectsFromFile(pair.Value);
							result = new Configuration(projects, result.Tasks, result.OutputDirectory, result.ImportDirectory, result.DefaultDateFile);
						}
						break;
					case Property.TasksFile:
						if (File.Exists(pair.Value))
						{
							var tasks = ReadTasksFromFile(pair.Value);
							result = new Configuration(result.Projects, tasks, result.OutputDirectory, result.ImportDirectory, result.DefaultDateFile);
						}
						break;
					case Property.OutputDirectory:
						result = new Configuration(result.Projects, result.Tasks, pair.Value, result.ImportDirectory, result.DefaultDateFile);
						break;
					case Property.ImportDirectory:
						result = new Configuration(result.Projects, result.Tasks, result.OutputDirectory, pair.Value, result.DefaultDateFile);
						break;
					case Property.DefaultDateFile:
						result = new Configuration(result.Projects, result.Tasks, result.OutputDirectory, result.ImportDirectory, pair.Value);
						break;
				}
			}

			log.DebugFormat("Project Count: {0}", result.Projects.Count);
			log.DebugFormat("Task Count: {0}", result.Tasks.Count);
			log.DebugFormat("Import Directory: {0}", result.ImportDirectory);
			log.DebugFormat("Output Directory: {0}", result.OutputDirectory);

			return result;
		}

		private static KeyValuePair<string, string> MakeKeyValue(string argument)
		{
			KeyValuePair<string, string> result;
			var splitArgument = argument.Split('=');

			if (splitArgument.Length > 1)
			{
				result = new KeyValuePair<string, string>(splitArgument[0].ToLower().Trim(), splitArgument[1].ToLower().Trim());
			}
			else
			{
				result = new KeyValuePair<string, string>(splitArgument[0].ToLower().Trim(), String.Empty);
			}

			return result;
		}

		private static IEnumerable<string> ReadSettingsFromFile(string filePath)
		{
			var result = new List<string>();

			if (File.Exists(filePath))
			{
				using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var fileReader = new StreamReader(stream);

					var input = fileReader.ReadToEnd();

					result.AddRange(input
						.Split('\n')
						.Select(line => line.Trim())
						.Where(line => line.Length > 0)
						);
				}
			}

			return result;
		}

		private static IList<Project> ReadProjectsFromFile(string filePath)
		{
			var result = new List<Project>();

			if (File.Exists(filePath))
			{
				using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var fileReader = new StreamReader(stream);

					var input = fileReader.ReadToEnd();

					result.AddRange(JsonConvert.DeserializeObject<ICollection<Project>>(input));

				}
			}

			return result;
		}

		private static IList<Task> ReadTasksFromFile(string filePath)
		{
			var result = new List<Task>();

			if (File.Exists(filePath))
			{
				using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var fileReader = new StreamReader(stream);

					var input = fileReader.ReadToEnd();

					result.AddRange(JsonConvert.DeserializeObject<ICollection<Task>>(input));

				}
			}

			return result;
		}
	}
}