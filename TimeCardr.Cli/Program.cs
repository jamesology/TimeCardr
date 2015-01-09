using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Config;

namespace TimeCardr.Cli
{
	class Program
	{
		private static IEnumerable<Project> _projects;
		private static IEnumerable<Task> _tasks;
		static void Main(string[] args)
		{
			XmlConfigurator.Configure(new FileInfo("TimeCardr.Cli.log4net.config"));
			var log = LogManager.GetLogger("main");
			var action = UserAction.Continue;

			try
			{
				while (action != UserAction.Exit)
				{
					var config = Configurator.Initialize(args, log);

					_projects = config.Projects;
					_tasks = config.Tasks;

					foreach (var project in _projects)
					{
						log.DebugFormat("{0} - {1}", project.Id, project.Name);
					}

					foreach (var task in _tasks)
					{
						log.DebugFormat("{0} - {1}", task.Id, task.Name);
						log.Debug(task.Description);
					}

					var entries = Read.FromFile(config.TimesheetFile, log);

					var entryDate = GetEntryDate(log);
					var entry = entries[entryDate];

					entry = GetTasks(entry, log);

					entries[entryDate] = entry;

					action = UserContinue(log);

					Write.ToFile(config.TimesheetFile, entries, log);
				}
			}
			catch (Exception ex)
			{
				log.ErrorFormat("{0}: {1}", ex.GetType(), ex.Message);
			}

			Console.ReadLine();
		}

		private static DateTime GetEntryDate(ILog log)
		{
			log.Error("GetEntryDate not implemented.");
			throw new NotImplementedException();
		}

		private static ICollection<Entry> GetTasks(ICollection<Entry> entry, ILog log)
		{
			log.Error("GetTasks not implemented.");
			throw new NotImplementedException();
		}

		private static UserAction UserContinue(ILog log)
		{
			log.Error("UserContinue not implemented.");
			throw new NotImplementedException();
		}
	}
}
