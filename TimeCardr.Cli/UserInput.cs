using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using log4net;

namespace TimeCardr.Cli
{
	static class UserInput
	{
		public static IDictionary<DateTime, ICollection<Entry>> Retrieve(IDictionary<DateTime, ICollection<Entry>> entries, string resourceName, ICollection<Project> projects, ICollection<Task> tasks, string defaultDataFile, ILog log)
		{
			var entryDate = GetEntryDate(log);
			ICollection<Entry> entry = new Collection<Entry>();

			//TODO: Offer default entry

			var dayHours = GetHours(log);
			var isDayValid = (dayHours == 0);

			while (isDayValid == false)
			{
				entry = new Collection<Entry>();
				var totalProjectHours = 0;
				foreach (var project in projects.Where(x => x.Active))
				{
					Console.WriteLine("Project: {0}", project.Name);
					var projectEntries = GetProject(project, entryDate, resourceName, tasks, defaultDataFile, log);

					var projectHours = 0;
					foreach (var projectEntry in projectEntries)
					{
						entry.Add(projectEntry);
						projectHours += projectEntry.Hours;
					}
					totalProjectHours += projectHours;
				}

				isDayValid = (dayHours == totalProjectHours);

				if (isDayValid == false)
				{
					Console.WriteLine("Hours entered for projects({0}) do not match daily total({1}). Please reenter.", totalProjectHours, dayHours);
				}
			}

			entries[entryDate] = entry.Where(x => x.Hours > 0).ToList();
			return entries;
		}

		private static DateTime GetEntryDate(ILog log)
		{
			var result = DateTime.Today;
			var validDate = false;

			while (validDate == false)
			{
				Console.Write("Enter Date (leave blank if today): ");
				var entry = Console.ReadLine();

				validDate = String.IsNullOrWhiteSpace(entry) || DateTime.TryParse(entry, out result);
			}

			log.DebugFormat("Date entered: {0:d}", result);
			return result;
		}

		private static int GetHours(ILog log)
		{
			var result = 0;
			var validHours = false;
			while (validHours == false)
			{
				Console.Write("Total Hours (blank for 0): ");
				var hours = Console.ReadLine();
				hours = (hours != null && hours.Length == 0) ? "0" : hours;

				validHours = Int32.TryParse(hours, out result);
			}

			log.DebugFormat("Hours entered: {0}", result);
			return result;
		}

		private static IEnumerable<Entry> GetProject(Project project, DateTime entryDate, string resourceName, ICollection<Task> tasks, string defaultDataFile, ILog log)
		{
			var projectEntries = AllowDefaultEntry(defaultDataFile, project.Id, entryDate, resourceName, log);

			if (projectEntries.Any() == false)
			{
				var projectHours = GetHours(log);
				var isProjectValid = (projectHours == 0);

				while (isProjectValid == false)
				{
					projectEntries = new Collection<Entry>();
					var totalTaskHours = 0;
					foreach (
						var taskEntry in tasks.Select(task => GetTask(task, new Entry(entryDate, resourceName, project.Id, "", 0))))
					{
						projectEntries.Add(taskEntry);
						totalTaskHours += taskEntry.Hours;
					}

					isProjectValid = (projectHours == totalTaskHours);

					if (isProjectValid == false)
					{
						Console.WriteLine("Hours entered for tasks({0}) do not match project total({1}). Please reenter.", totalTaskHours,
							projectHours);
					}
				}
			}

			return projectEntries;
		}

		private static ICollection<Entry> AllowDefaultEntry(string defaultDateFile, string projectId, DateTime entryDate, string resourceName, ILog log)
		{
			Console.Write("Use default day? ");
			var useDefault = Console.ReadLine();
			var defaultDay = ((useDefault != null) && (useDefault.ToLower() == "y"));

			ICollection<Entry> result = new Collection<Entry>();
			if (defaultDay)
			{
				var defaultEntries = Read.FromFile(new Dictionary<DateTime, ICollection<Entry>>(), defaultDateFile, resourceName, log);

				result = defaultEntries[defaultEntries.Keys.First()]
					.Select(x => new Entry(entryDate, resourceName, projectId, x.Task, x.Hours))
					.ToList();
			}

			return result;
		} 

		private static Entry GetTask(Task task, Entry entry)
		{
			var taskHours = 0;
			var validTaskHours = false;

			while (validTaskHours == false)
			{
				Console.Write("{0} hours (? for description, blank for 0): ", task.Name);
				var taskHoursEntry = Console.ReadLine();

				if (taskHoursEntry == "?")
				{
					Console.WriteLine(task.Description);
				}
				else if (String.IsNullOrWhiteSpace(taskHoursEntry))
				{
					validTaskHours = true;
					taskHours = 0;
				}
				else
				{
					validTaskHours = Int32.TryParse(taskHoursEntry, out taskHours);

				}
			}

			return new Entry(entry.Date, entry.ResourceName, entry.Project, task.Id, taskHours);
		}
	}
}