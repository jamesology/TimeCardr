using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace TimeCardr
{
	public class Executor
	{
		public static IDictionary<DateTime, ICollection<Entry>> Execute(IDictionary<DateTime, ICollection<Entry>> userEntries, Configuration config, ILog log)
		{
			IDictionary<DateTime, ICollection<Entry>> entries = new Dictionary<DateTime, ICollection<Entry>>();
			
			entries = Read.FromImportFiles(entries, config.ImportDirectory, config.ResourceName, log);

			entries = Read.FromFile(entries, config.DataFile, config.ResourceName, log);

			foreach (var entry in userEntries)
			{
				entries[entry.Key] = entry.Value;
			}

			entries = FreshenData(entries, log);

			Write.ToFile(config.DataFile, entries, log);
			Write.MonthlyDetail(config.OutputDirectory, entries, log);
			Write.MonthlySummary(config.OutputDirectory, entries, log);
			Write.WeeklyDetail(config.OutputDirectory, entries, log);

			return entries;
		}

		public static IDictionary<DateTime, ICollection<Entry>> FreshenData(IDictionary<DateTime, ICollection<Entry>> entries, ILog log)
		{
			var maximumAge = (new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).AddMonths(-2);

			var staleDates = entries.Keys.Where(x => x.Date < maximumAge).ToList();

			foreach (var staleDate in staleDates)
			{
				log.DebugFormat("Removing entries for {0:d}", staleDate);
				entries.Remove(staleDate);
			}

			return entries;
		} 
	}
}
