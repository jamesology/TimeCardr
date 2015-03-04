using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

				var entries = Read.FromFile(config.DataFile, config.ResourceName, log);

				//TODO: Remove data older than the month before last

				//TODO: Import old version data
				entries = Read.FromImportFiles(entries, config.ImportDirectory, config.ResourceName, log);

				while (action != UserAction.Exit)
				{
					//TODO: Get date for entry
					//var entryDate = GetEntryDate(log);
					//ICollection<Entry> entry = new Collection<Entry>();

					//TODO: Offer default entry
					//TODO: get day hours
					//TODO: Iterate projects
					//TODO: get project hours
					//TODO: Iterate tasks
					//TODO: get task hours
					//entry = GetTasks(entry, entryDate, log);
					//entries[entryDate] = entry;

					//TODO: Compare Total Task hours to project hours
					//TODO: Compare total project hours to total day hours
					//TODO: allow new day entry
					action = UserContinue(log);
				}

				//TODO: write raw data
				//Write.ToFile(config.OutputDirectory, entries, log);
				//TODO: write monthly detail
				//TODO: write monthly summary
			}
			catch (Exception ex)
			{
				log.ErrorFormat("{0}: {1}", ex.GetType(), ex.Message);
			}

			Console.Write("Press Enter to exit.");
			Console.ReadLine();
		}

		private static DateTime GetEntryDate(ILog log)
		{
			DateTime result = DateTime.Today;
			var validDate = false;

			while (validDate == false)
			{
				Console.Write("Enter Date (leave blank if today): ");
				var entry = Console.ReadLine();

				if (String.IsNullOrWhiteSpace(entry))
				{
					validDate = true;
				}
				else
				{
					validDate = DateTime.TryParse(entry, out result);
				}
			}

			return result;
		}

		private static ICollection<Entry> GetTasks(ICollection<Entry> entry, DateTime entryDate, string resourceName, ILog log)
		{
			//TODO: This is hideous. Clean it up asshole.
			Project currentProject = null;
			var validProject = false;
			while (validProject == false)
			{
				Console.WriteLine("Select a Project:");
				for (int i = 0; i < _projects.Count(); i++)
				{
					Console.WriteLine("\t{0}: {1}", i, _projects[i].Name);
				}
				var project = Console.ReadLine();
				int projectIndex;
				validProject = Int32.TryParse(project, out projectIndex);
				if(validProject) { currentProject = _projects[projectIndex];}
			}

			var totalHours = 0;
			var validHours = false;
			while (validHours == false)
			{
				Console.Write("Total Hours: ");
				var hours = Console.ReadLine();
				validHours = Int32.TryParse(hours, out totalHours);
			}

			var validEntry = false;
			while (validEntry == false)
			{
				var entryHours = 0;

				foreach (var task in _tasks)
				{
					var taskHours = 0;
					var validTaskHours = false;
					while (validTaskHours == false)
					{
						Console.Write("{0} hours (? for description, blank for 0): ", task.Name);
						var taskEntry = Console.ReadLine();

						if (taskEntry == "?")
						{
							Console.WriteLine(task.Description);
						}
						else if(String.IsNullOrWhiteSpace(taskEntry))
						{
							validTaskHours = true;
							taskHours = 0;
						}
						else
						{
							validTaskHours = Int32.TryParse(taskEntry, out taskHours);

						}
					}

					entry.Add(new Entry(entryDate, resourceName, currentProject.Id, task.Id, taskHours));
					entryHours += taskHours;
				}

				validEntry = (totalHours == entryHours);

				if (validEntry == false)
				{
					Console.WriteLine("Sum of task hours ({0}) not equal to total hours ({1}). Please reenter.", entryHours, totalHours);
					entry = new Collection<Entry>();
				}
			}

			return entry;
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
