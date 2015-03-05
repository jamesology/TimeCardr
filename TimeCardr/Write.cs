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
			using (var stream = new FileStream(dataFile, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				var fileWriter = new StreamWriter(stream);

				foreach (var entry in entries.SelectMany(dateEntries => dateEntries.Value).Where(entry => entry.Hours > 0).OrderBy(x => x.Date))
				{
					fileWriter.WriteLine("{0},{1},{2:d},{3},{4}", entry.ResourceName, entry.Project, entry.Date, entry.Task, entry.Hours);
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
				var monthFileName = String.Format("{0}.{1}.Detail.txt", month.Year, month.Day);
				var filePath = Path.Combine(outputDirectory, monthFileName);
				using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					var fileWriter = new StreamWriter(stream);

					var monthEntries =
						entries.SelectMany(dateEntries => dateEntries.Value)
							.Where(entry => entry.Date.Year == month.Year && entry.Date.Month == month.Month)
							.OrderBy(x => x.Date)
							.ToList();

					foreach (var entry in monthEntries)
					{
						fileWriter.WriteLine("{0},{1},{2:d},{3},{4}", entry.ResourceName, entry.Project, entry.Date, entry.Task, entry.Hours);
					}

					fileWriter.Close();
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
				var monthFileName = String.Format("{0}.{1}.Summary.txt", month.Year, month.Day);
				var filePath = Path.Combine(outputDirectory, monthFileName);
				using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					var fileWriter = new StreamWriter(stream);

					var monthEntries =
						entries.SelectMany(dateEntries => dateEntries.Value)
							.Where(entry => entry.Date.Year == month.Year && entry.Date.Month == month.Month)
							.Aggregate(new Dictionary<string, Entry>(), (dictionary, entry) =>
							{
								var key = String.Format("{0}{1}", entry.Project, entry.Task);
								var currentHours = 0;
								if (dictionary.ContainsKey(key))
								{
									currentHours = dictionary[key].Hours;
								}

								dictionary[key] = new Entry(month, entry.ResourceName, entry.Project, entry.Task, entry.Hours + currentHours);

								return dictionary;
							})
							.Select(x => x.Value)
							.OrderBy(x => x.Date)
							.ToList();

					foreach (var entry in monthEntries)
					{
						fileWriter.WriteLine("{0},{1},{2:d},{3},{4}", entry.ResourceName, entry.Project, entry.Date, entry.Task, entry.Hours);
					}

					fileWriter.Close();
					stream.Close();
				}
			}
		}
	}
}
