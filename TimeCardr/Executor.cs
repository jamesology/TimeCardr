using System;
using System.Collections.Generic;
using log4net;

namespace TimeCardr
{
	public class Executor
	{
		public static IDictionary<DateTime, ICollection<Entry>> Execute(Configuration config, ILog log)
		{
			IDictionary<DateTime, ICollection<Entry>> entries = new Dictionary<DateTime, ICollection<Entry>>();
			
			entries = Read.FromImportFiles(entries, config.ImportDirectory, config.ResourceName, log);

			entries = Read.FromFile(entries, config.DataFile, config.ResourceName, log);

			//TODO: Merge with user entries

			//TODO: Remove data older than the month before last

			//TODO: Import old version data


			//TODO: write raw data
			Write.ToFile(config.DataFile, entries, log);
			//TODO: write monthly detail
			//TODO: write monthly summary

			return entries;
		}
	}
}
