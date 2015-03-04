using System.Runtime.InteropServices;
using log4net;
using NUnit.Framework;
using Shouldly;

namespace TimeCardr.Tests
{
	[TestFixture]
	class ConfiguratorTests
	{
		private ILog log;

		[SetUp]
		public void Setup()
		{
			log = LogManager.GetLogger("test");
		}

		[Test]
		public void Initialize_EmptyArguments_NoValuesSet()
		{
			var args = new string[0];

			var actual = Configurator.Initialize(args, log);

			actual.Projects.ShouldBeEmpty();
			actual.Tasks.ShouldBeEmpty();
			actual.TimesheetFile.ShouldBeEmpty();
		}
	}
}
