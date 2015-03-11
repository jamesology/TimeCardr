using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using log4net.Config;

namespace TimeCardr.Cli
{
	class Program
	{
		//TODO: make this private variable unused
		private static IList<Project> _projects;
		//TODO: make this private variable unused
		private static IList<Task> _tasks;

		static void Main(string[] args)
		{
			XmlConfigurator.Configure(new FileInfo("TimeCardr.Cli.log4net.config"));
			var log = LogManager.GetLogger("timecardr");
			var action = UserAction.Continue;

			try
			{
				var config = Configurator.Initialize(args, log);

				_projects = config.Projects;
				foreach (var project in _projects)
				{
					log.DebugFormat("{0} - {1}", project.Id, project.Name);
				}

				_tasks = config.Tasks;
				foreach (var task in _tasks)
				{
					log.DebugFormat("{0} - {1}", task.Id, task.Name);
					log.Debug(task.Description);
				}

				IDictionary<DateTime, ICollection<Entry>> userEntries = new Dictionary<DateTime, ICollection<Entry>>();
				while (action != UserAction.Exit)
				{
					userEntries = UserInput.Retrieve(userEntries, config.ResourceName, _projects, _tasks, config.DefaultDateFile, log);
					action = UserContinue(log);
				}

				Executor.Execute(userEntries, config, log);
			}
			catch (Exception ex)
			{
				log.ErrorFormat("{0}: {1}", ex.GetType(), ex.Message);
			}

			Console.Write("Press Enter to exit.");
			Console.ReadLine();
		}

		private static UserAction UserContinue(ILog log)
		{
			Console.Write("Enter another date? (Y/N) ");
			var entry = Console.ReadLine().ToLowerInvariant();

			var result = (entry == "y") ? UserAction.Continue : UserAction.Exit;

			return result;
		}
	}
}
