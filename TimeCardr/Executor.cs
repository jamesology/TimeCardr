using System;
using System.Collections.Generic;
using log4net;

namespace TimeCardr
{
	public class Executor
	{
		public static IDictionary<DateTime, ICollection<Entry>> Execute(Configuration config, ILog log)
		{
			var entries = Read.FromFile(config.DataFile, config.ResourceName, log);

			//TODO: Merge with user entries

			//TODO: Remove data older than the month before last

			//TODO: Import old version data
			entries = Read.FromImportFiles(entries, config.ImportDirectory, config.ResourceName, log);


			//TODO: write raw data
			Write.ToFile(config.DataFile, entries, log);
			//TODO: write monthly detail
			//TODO: write monthly summary

			return entries;
		}
	}
}
