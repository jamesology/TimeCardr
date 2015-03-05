using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace TimeCardr
{
	public class Read
	{
		public static IDictionary<DateTime, ICollection<Entry>> FromFile(IDictionary<DateTime, ICollection<Entry>> entries, string dataFile, string resourceName, ILog log)
		{
			var result = entries;

			if (File.Exists(dataFile))
			{
				string input;
				using (var stream = new FileStream(dataFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var fileReader = new StreamReader(stream);

					input = fileReader.ReadToEnd();

					stream.Close();
				}

				var entryData = input
					.Split('\n')
					.Select(line => line.Trim())
					.Where(line => line.Length > 0);

				var allEntries = CreateEntries(entryData, resourceName);

				foreach (var entryDate in allEntries.Select(x => x.Date).Distinct())
				{
					var dateEntries = allEntries.Where(x => x.Date == entryDate).ToList();
					result[entryDate] = dateEntries;
				}
			}

			return result;
		}

		public static IDictionary<DateTime, ICollection<Entry>> FromImportFiles(IDictionary<DateTime, ICollection<Entry>> entries, string importDirectory, string resourceName, ILog log)
		{
			var result = entries;

			var importDirectoryInfo = new DirectoryInfo(importDirectory);
			if (importDirectoryInfo.Exists)
			{
				foreach (var importFileInfo in importDirectoryInfo.EnumerateFiles())
				{
					log.InfoFormat("Importing from {0}", importFileInfo.Name);
					var importEntries = FromFile(new Dictionary<DateTime, ICollection<Entry>>(), importFileInfo.FullName, resourceName, log);

					foreach (var entry in importEntries)
					{
						result[entry.Key] = entry.Value;
					}
				}
			}

			return result;
		} 

		public static IList<Entry> CreateEntries(IEnumerable<string> entryData, string resourceName)
		{
			var result = new ConcurrentBag<Entry>();

			var task = Parallel.ForEach(entryData, record => CreateEntry(record, resourceName, result));

			while (task.IsCompleted == false)
			{
				Thread.Sleep(10);
			}

			return result.ToList();
		}

		private static void CreateEntry(string record, string resourceName, ConcurrentBag<Entry> result)
		{
			var split = record.Split(',');

			var project = split[1];
			var date = DateTime.Parse(split[2]);
			var task = split[3];
			var hours = Int32.Parse(split[4]);
			result.Add(new Entry(date, resourceName, project, task, hours));
		}
	}
}
