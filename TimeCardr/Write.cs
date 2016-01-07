using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;

namespace TimeCardr
{
	public class Write
	{
		public static void ToFile(string dataFile, IDictionary<DateTime, ICollection<Entry>> entries, ILog log)
		{
			if (File.Exists(dataFile))
			{
				string copyPath = String.Format("{0}.copy", dataFile);
				File.Copy(dataFile, copyPath, true);
			}

			using (var stream = new FileStream(dataFile, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				var fileWriter = new StreamWriter(stream);

				foreach (var entry in entries.SelectMany(dateEntries => dateEntries.Value).Where(entry => entry.Hours > 0).OrderBy(x => x.Date).ThenBy(x => x.Project).ThenBy(x => x.Task))
				{
					fileWriter.WriteLine("{0},{1},{2:d},{3},{4:F2}", entry.ResourceName, entry.Project, entry.Date, entry.Task, entry.Hours);
				}

				fileWriter.Close();
				stream.Close();
			}
		}

		public static void MonthlyDetail(string outputDirectory, IDictionary<DateTime, ICollection<Entry>> entries, ILog log)
		{
			if (Directory.Exists(outputDirectory) == false)
			{
				Directory.CreateDirectory(outputDirectory);
			}

			var months = entries.Select(x => new DateTime(x.Key.Year, x.Key.Month, 1)).Distinct();

			foreach (var month in months)
			{
				var monthEntries =
					entries.SelectMany(dateEntries => dateEntries.Value)
						.Where(entry => entry.Date.Year == month.Year && entry.Date.Month == month.Month)
						.OrderBy(x => x.Date)
						.ThenBy(x => x.Project)
						.ThenBy(x => x.Task)
						.ToList();

				var monthFileName = String.Format("{0:yyyy.MM.MMMM}.Detail.txt", month);
				var filePath = Path.Combine(outputDirectory, monthFileName);
				using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					using (var fileWriter = new StreamWriter(stream))
					{
						foreach (var entry in monthEntries)
						{
							fileWriter.WriteLine("{0},{1},{2:d},{3},{4:F2}", entry.ResourceName, entry.Project, entry.Date, entry.Task, entry.Hours);
						}

						fileWriter.Close();
					}
					stream.Close();
				}
			}
		}

		public static void MonthlySummary(string outputDirectory, IDictionary<DateTime, ICollection<Entry>> entries, ILog log)
		{
			if (Directory.Exists(outputDirectory) == false)
			{
				Directory.CreateDirectory(outputDirectory);
			}

			var months = entries.Select(x => new DateTime(x.Key.Year, x.Key.Month, 1)).Distinct();

			foreach (var month in months)
			{
				var monthEntries =
					entries.SelectMany(dateEntries => dateEntries.Value)
						.Where(entry => entry.Date.Year == month.Year && entry.Date.Month == month.Month)
						.Aggregate(new Dictionary<string, Entry>(), (dictionary, entry) =>
						{
							var key = String.Format("{0}{1}", entry.Project, entry.Task);
							var currentHours = 0.0;
							if (dictionary.ContainsKey(key))
							{
								currentHours = dictionary[key].Hours;
							}

							dictionary[key] = new Entry(month, entry.ResourceName, entry.Project, entry.Task, entry.Hours + currentHours);

							return dictionary;
						})
						.Select(x => x.Value)
						.OrderBy(x => x.Date)
						.ThenBy(x => x.Project)
						.ThenBy(x => x.Task)
						.ToList();

				var monthFileName = String.Format("{0:yyyy.MM.MMMM}.Summary.txt", month);
				var filePath = Path.Combine(outputDirectory, monthFileName);
				using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					using (var fileWriter = new StreamWriter(stream))
					{
						foreach (var entry in monthEntries)
						{
							fileWriter.WriteLine("{0},{1},{2:d},{3},{4:F2},{5:F2}%", entry.ResourceName, entry.Project, entry.Date, entry.Task, entry.Hours, CalculatePercentage(entry, monthEntries));
						}

						fileWriter.Close();
					}
					stream.Close();
				}
			}
		}

		public static void WeeklyDetail(string outputDirectory, IDictionary<DateTime, ICollection<Entry>> entries, ILog log)
		{
			if (Directory.Exists(outputDirectory) == false)
			{
				Directory.CreateDirectory(outputDirectory);
			}

			var weekEntries =
				entries.SelectMany(dateEntries => dateEntries.Value)
					.Where(entry => entry.Date > DateTime.Today.AddDays(-7))
					.OrderBy(x => x.Date)
					.ThenBy(x => x.Project)
					.ThenBy(x => x.Task)
					.ToList();

			var weekFileName = ("Week.Detail.txt");
			var filePath = Path.Combine(outputDirectory, weekFileName);
			using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				using (var fileWriter = new StreamWriter(stream))
				{
					var dates = weekEntries.Select(x => x.Date).Distinct().ToList();
					foreach (var date in dates)
					{
						foreach (var entry in weekEntries.Where(x => x.Date == date))
						{
							fileWriter.WriteLine("{0},{1},{2:d},{3},{4:F2}", entry.ResourceName, entry.Project, entry.Date, entry.Task, entry.Hours);
						}

						fileWriter.WriteLine();
					}

					fileWriter.Close();
				}
				stream.Close();
			}
		}

		private static double CalculatePercentage(Entry entry, IEnumerable<Entry> monthEntries)
		{
			var ratio = (entry.Hours / (monthEntries.Sum(x => x.Hours)));
			return ratio * 100.0;
		}
	}
}
