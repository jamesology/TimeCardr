using System;
using System.Collections.Generic;
using log4net;

namespace TimeCardr
{
	public class Write
	{
		public static void ToFile(string timesheetFile, IDictionary<DateTime, IEnumerable<Entry>> entries, ILog log)
		{
			log.Error("Write.ToFile not implemented.");
			throw new NotImplementedException();
		}
	}
}
