using System;
using System.Collections.Generic;
using log4net;

namespace TimeCardr
{
	public class Read
	{
		public static IDictionary<DateTime, IEnumerable<Entry>> FromFile(string timesheetFile, ILog log)
		{
			log.Error("Read.FromFile not implemented.");
			throw new System.NotImplementedException();
		}
	}
}
