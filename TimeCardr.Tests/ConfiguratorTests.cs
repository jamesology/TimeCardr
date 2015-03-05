using log4net;
using NUnit.Framework;
using Shouldly;

namespace TimeCardr.Tests
{
	[TestFixture]
	class ConfiguratorTests
	{
		private ILog _log;

		[SetUp]
		public void Setup()
		{
			_log = LogManager.GetLogger("test");
		}

		[Ignore("Needs work")]
		public void Initialize_EmptyArguments_NoValuesSet()
		{
			var args = new string[0];

			var actual = Configurator.Initialize(args, _log);

			actual.Projects.ShouldBeEmpty();
			actual.Tasks.ShouldBeEmpty();
			actual.DataFile.ShouldBeEmpty();
		}
	}
}
