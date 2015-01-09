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
		public static IDictionary<DateTime, ICollection<Entry>> FromFile(string timesheetFile, ILog log)
		{
			var result = new Dictionary<DateTime, ICollection<Entry>>();

			if (File.Exists(timesheetFile))
			{
				using (var stream = new FileStream(timesheetFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var fileReader = new StreamReader(stream);

					var input = fileReader.ReadToEnd();

					var entryData = input
							.Split('\n')
							.Select(line => line.Trim())
							.Where(line => line.Length > 0)
							.Skip(1);

					var allEntries = CreateEntries(entryData);

					foreach (var entry in allEntries)
					{
						var entries = result[entry.Date];

						if (entries == null)
						{
							entries = new List<Entry>();
							result[entry.Date] = entries;
						}

						entries.Add(entry);
					}
				}
			}

			return result;
		}

		public static IEnumerable<Entry> CreateEntries(IEnumerable<string> entryData)
		{
			var result = new ConcurrentBag<Entry>();

			var task = Parallel.ForEach(entryData, record => CreateEntry(record, result));

			while (task.IsCompleted == false)
			{
				Thread.Sleep(10);
			}

			return result;
		}

		private static void CreateEntry(string record, ConcurrentBag<Entry> result)
		{
			var split = record.Split(',');

			var project = split[1];
			var date = DateTime.Parse(split[2]);
			var task = split[3];
			var hours = Int32.Parse(split[3]);
			result.Add(new Entry(date, project, task, hours));
		}
	}
}
