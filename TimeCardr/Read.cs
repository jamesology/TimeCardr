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
		public static IDictionary<DateTime, ICollection<Entry>> FromFile(string dataFile, string resourceName, ILog log)
		{
			var result = new Dictionary<DateTime, ICollection<Entry>>();

			if (File.Exists(dataFile))
			{
				using (var stream = new FileStream(dataFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var fileReader = new StreamReader(stream);

					var input = fileReader.ReadToEnd();

					var entryData = input
							.Split('\n')
							.Select(line => line.Trim())
							.Where(line => line.Length > 0);

					var allEntries = CreateEntries(entryData, resourceName);

					foreach (var entry in allEntries)
					{
						if (result.ContainsKey(entry.Date) == false)
						{
							result[entry.Date] = new List<Entry>();
						}
						var entries = result[entry.Date];

						entries.Add(entry);
					}

					stream.Close();
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
					var importEntries = FromFile(importFileInfo.FullName, resourceName, log);

					foreach (var entry in importEntries.Values.SelectMany(importDayEntries => importDayEntries))
					{
						if (result.ContainsKey(entry.Date) == false)
						{
							result[entry.Date] = new List<Entry>();
						}
						var dayEntries = result[entry.Date];

						dayEntries.Add(entry);
					}
				}
			}

			return result;
		} 

		public static IEnumerable<Entry> CreateEntries(IEnumerable<string> entryData, string resourceName)
		{
			var result = new ConcurrentBag<Entry>();

			var task = Parallel.ForEach(entryData, record => CreateEntry(record, resourceName, result));

			while (task.IsCompleted == false)
			{
				Thread.Sleep(10);
			}

			return result;
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
