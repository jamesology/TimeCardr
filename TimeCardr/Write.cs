using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;

namespace TimeCardr
{
	public class Write
	{
		public static void ToFile(string timesheetFile, IDictionary<DateTime, ICollection<Entry>> entries, ILog log)
		{
			using (var stream = new FileStream(timesheetFile, FileMode.Create, FileAccess.Write, FileShare.Read))
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
	}
}
