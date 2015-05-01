using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using NUnit.Framework;
using Shouldly;

namespace TimeCardr.Tests
{
	[TestFixture]
	class ExecutorTests
	{
		private string _testDirectory;
		private readonly Random _randomizer = new Random();
		private ILog _log;

		[SetUp]
		public void TestSetUp()
		{
			_log = LogManager.GetLogger("ExecutorTests");

			_testDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			if (Directory.Exists(_testDirectory) == false)
			{
				Directory.CreateDirectory(_testDirectory);
			}
		}
		[TearDown]
		public void TestTearDown()
		{
			if (Directory.Exists(_testDirectory))
			{
				Directory.Delete(_testDirectory, true);
			}
		}

		[Test]
		public void Execute_NoInputData_NoOutput()
		{
			var config = BuildConfiguration(0, 0);

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>();

			var results = Executor.Execute(userEntries, config, _log);

			var outputFileInfo = new FileInfo(config.DataFile);

			results.ShouldBeEmpty();
			outputFileInfo.Exists.ShouldBe(true);
			outputFileInfo.Length.ShouldBe(0);
		}

		[Test]
		public void Execute_DataInput_OutputMatchesInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);
			CreateDataFile(config.DataFile, expectedEntries);
			var outputFileInfo = new FileInfo(config.DataFile);
			var expectedLength = outputFileInfo.Length;

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>();

			var results = Executor.Execute(userEntries, config, _log);

			results[DateTime.Today].ShouldBe(expectedEntries, true);
			outputFileInfo.Length.ShouldBe(expectedLength);
		}

		[Test]
		public void Execute_ImportInput_OutputMatchesInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);
			CreateImportFile(config.ImportDirectory, expectedEntries);

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>();

			var results = Executor.Execute(userEntries, config, _log);

			results[DateTime.Today].ShouldBe(expectedEntries, true);
			var outputFileInfo = new FileInfo(config.DataFile);
			outputFileInfo.Length.ShouldBeGreaterThan(0);
		}

		[Test]
		public void Execute_ImportAndDataInput_OutputCombinesInputs()
		{
			var config = BuildConfiguration(1, 1);
			var importEntries = BuildEntries(5, DateTime.Today.AddDays(-1), config.ResourceName, config.Projects, config.Tasks, false);
			CreateImportFile(config.ImportDirectory, importEntries);

			var dataEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);
			CreateDataFile(config.DataFile, dataEntries);
			var outputFileInfo = new FileInfo(config.DataFile);
			var initialLength = outputFileInfo.Length;

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>();

			var results = Executor.Execute(userEntries, config, _log);
			outputFileInfo.Refresh();

			results.Keys.Count.ShouldBe(2);
			results[DateTime.Today].ShouldBe(dataEntries, true);
			results[DateTime.Today.AddDays(-1)].ShouldBe(importEntries, true);
			outputFileInfo.Length.ShouldBeGreaterThan(initialLength);
		}

		[Test]
		public void Execute_ImportAndDataInputWithDateCollision_OutputPrefersDataInput()
		{
			var config = BuildConfiguration(1, 1);
			var importEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);
			CreateImportFile(config.ImportDirectory, importEntries);

			var dataEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);
			CreateDataFile(config.DataFile, dataEntries);
			var outputFileInfo = new FileInfo(config.DataFile);
			var initialLength = outputFileInfo.Length;

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>();

			var results = Executor.Execute(userEntries, config, _log);
			outputFileInfo.Refresh();

			results.Keys.Count.ShouldBe(1);
			results[DateTime.Today].ShouldBe(dataEntries, true);
			outputFileInfo.Length.ShouldBe(initialLength);
		}

		[Test]
		public void Execute_UserInput_OutputMatchesInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>
			{
				{DateTime.Today, expectedEntries}
			};

			var results = Executor.Execute(userEntries, config, _log);

			results.Keys.Count.ShouldBe(1);
			results[DateTime.Today].ShouldBe(expectedEntries, true);
			var outputFileInfo = new FileInfo(config.DataFile);
			outputFileInfo.Length.ShouldBeGreaterThan(0);
		}

		[Test]
		public void Execute_UserAndDataInput_OutputMatchesInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>
			{
				{DateTime.Today, expectedEntries}
			};

			var dataEntries = BuildEntries(5, DateTime.Today.AddDays(-1), config.ResourceName, config.Projects, config.Tasks, false);
			CreateDataFile(config.DataFile, dataEntries);
			var outputFileInfo = new FileInfo(config.DataFile);
			var initialLength = outputFileInfo.Length;

			var results = Executor.Execute(userEntries, config, _log);
			outputFileInfo.Refresh();

			results.Keys.Count.ShouldBe(2);
			results[DateTime.Today].ShouldBe(expectedEntries, true);
			results[DateTime.Today.AddDays(-1)].ShouldBe(dataEntries, true);
			outputFileInfo.Length.ShouldBeGreaterThan(initialLength);
		}

		[Test]
		public void Execute_UserAndDataInputWithDateCollision_OutputPrefersUserInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>
			{
				{DateTime.Today, expectedEntries}
			};

			var dataEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);
			CreateDataFile(config.DataFile, dataEntries);

			var results = Executor.Execute(userEntries, config, _log);

			results.Keys.Count.ShouldBe(1);
			results[DateTime.Today].ShouldBe(expectedEntries, true);
			var outputFileInfo = new FileInfo(config.DataFile);
			outputFileInfo.Length.ShouldBeGreaterThan(0);
		}

		[Test]
		public void Execute_UserAndImportInput_OutputMatchesInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>
			{
				{DateTime.Today, expectedEntries}
			};

			var importEntries = BuildEntries(5, DateTime.Today.AddDays(-1), config.ResourceName, config.Projects, config.Tasks, false);
			CreateImportFile(config.ImportDirectory, importEntries);

			var results = Executor.Execute(userEntries, config, _log);

			results.Keys.Count.ShouldBe(2);
			results[DateTime.Today].ShouldBe(expectedEntries, true);
			results[DateTime.Today.AddDays(-1)].ShouldBe(importEntries, true);
			var outputFileInfo = new FileInfo(config.DataFile);
			outputFileInfo.Length.ShouldBeGreaterThan(0);
		}

		[Test]
		public void Execute_UserAndImportInputWithDateCollision_OutputPrefersUserInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>
			{
				{DateTime.Today, expectedEntries}
			};

			var importEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);
			CreateImportFile(config.ImportDirectory, importEntries);

			var results = Executor.Execute(userEntries, config, _log);

			results.Keys.Count.ShouldBe(1);
			results[DateTime.Today].ShouldBe(expectedEntries, true);
			var outputFileInfo = new FileInfo(config.DataFile);
			outputFileInfo.Length.ShouldBeGreaterThan(0);
		}

		[Test]
		public void Execute_UserAndDataAndImportInput_OutputMatchesInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>
			{
				{DateTime.Today, expectedEntries}
			};

			var dataEntries = BuildEntries(5, DateTime.Today.AddDays(-1), config.ResourceName, config.Projects, config.Tasks, false);
			CreateDataFile(config.DataFile, dataEntries);
			var outputFileInfo = new FileInfo(config.DataFile);
			var initialLength = outputFileInfo.Length;

			var importEntries = BuildEntries(5, DateTime.Today.AddDays(-2), config.ResourceName, config.Projects, config.Tasks, false);
			CreateImportFile(config.ImportDirectory, importEntries);

			var results = Executor.Execute(userEntries, config, _log);
			outputFileInfo.Refresh();

			results.Keys.Count.ShouldBe(3);
			results[DateTime.Today].ShouldBe(expectedEntries, true);
			results[DateTime.Today.AddDays(-1)].ShouldBe(dataEntries, true);
			results[DateTime.Today.AddDays(-2)].ShouldBe(importEntries, true);
			outputFileInfo.Length.ShouldBeGreaterThan(initialLength);
		}

		[Test]
		public void Execute_UserAndDataAndImportInputWithDateCollision_OutputPrefersUserInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>
			{
				{DateTime.Today, expectedEntries}
			};

			var dataEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);
			CreateDataFile(config.DataFile, dataEntries);

			var importEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);
			CreateImportFile(config.ImportDirectory, importEntries);

			var results = Executor.Execute(userEntries, config, _log);

			results.Keys.Count.ShouldBe(1);
			results[DateTime.Today].ShouldBe(expectedEntries, true);
			var outputFileInfo = new FileInfo(config.DataFile);
			outputFileInfo.Length.ShouldBeGreaterThan(0);
		}

		[Test]
		public void Execute_DataInputMultipleRuns_OutputMatchesInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);
			CreateDataFile(config.DataFile, expectedEntries);
			var outputFileInfo = new FileInfo(config.DataFile);
			var expectedLength = outputFileInfo.Length;

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>();

			Executor.Execute(userEntries, config, _log);
			var results = Executor.Execute(userEntries, config, _log);

			results[DateTime.Today].ShouldBe(expectedEntries, true);
			outputFileInfo.Length.ShouldBe(expectedLength);
		}

		[Test]
		public void Execute_ImportInputMultipleRuns_OutputMatchesInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today, config.ResourceName, config.Projects, config.Tasks, false);
			CreateImportFile(config.ImportDirectory, expectedEntries);

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>();

			Executor.Execute(userEntries, config, _log);
			var results = Executor.Execute(userEntries, config, _log);

			results[DateTime.Today].ShouldBe(expectedEntries, true);
			var outputFileInfo = new FileInfo(config.DataFile);
			outputFileInfo.Length.ShouldBeGreaterThan(0);
		}

		//TODO: Test that old data is removed
		[Test]
		public void Execute_DataOlderThanTwoMonths_OldDataRemovedFromOutput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, DateTime.Today.AddMonths(-1), config.ResourceName, config.Projects, config.Tasks, false);

			var userEntries = new Dictionary<DateTime, ICollection<Entry>>
			{
				{DateTime.Today.AddMonths(-1), expectedEntries}
			};

			var dataEntries = BuildEntries(5, DateTime.Today.AddMonths(-2), config.ResourceName, config.Projects, config.Tasks, false);
			CreateDataFile(config.DataFile, dataEntries);
			var outputFileInfo = new FileInfo(config.DataFile);
			var initialLength = outputFileInfo.Length;

			var importEntries = BuildEntries(5, DateTime.Today.AddMonths(-3), config.ResourceName, config.Projects, config.Tasks, false);
			CreateImportFile(config.ImportDirectory, importEntries);

			var results = Executor.Execute(userEntries, config, _log);
			outputFileInfo.Refresh();

			results.Keys.Count.ShouldBe(2);
			results[DateTime.Today.AddMonths(-1)].ShouldBe(expectedEntries, true);
			results[DateTime.Today.AddMonths(-2)].ShouldBe(dataEntries, true);
			outputFileInfo.Length.ShouldBeGreaterThan(initialLength);
		}

		private static string GenerateRandomString()
		{
			return Path.GetRandomFileName().Replace(".", "");
		}
		private static IList<Project> BuildProjects(int projectCount)
		{
			var result = new List<Project>();

			while (result.Count < projectCount)
			{
				var id = GenerateRandomString();
				var name = GenerateRandomString();

				result.Add(new Project(id, name, true));
			}

			return result;
		}
		private static IList<Task> BuildTasks(int taskCount)
		{
			var result = new List<Task>();

			while (result.Count < taskCount)
			{
				var id = GenerateRandomString();
				var name = GenerateRandomString();
				var description = GenerateRandomString();

				result.Add(new Task(id, name, description));
			}

			return result;
		}
		private ICollection<Entry> BuildEntries(int entryCount, DateTime entryDate, string resourceName, IList<Project> projects, IList<Task> tasks, bool includeZeroHours)
		{
			var result = new List<Entry>();

			while (result.Count < entryCount)
			{
				var project = projects[result.Count % projects.Count].Id;
				var task = tasks[result.Count % tasks.Count].Id;
				var hours = _randomizer.Next(8);

				result.Add(new Entry(entryDate, resourceName, project, task, hours));
			}

			if (includeZeroHours == false)
			{
				result = result.Where(x => x.Hours > 0).ToList();
			}
			return result;
		} 

		private Configuration BuildConfiguration(int projectCount, int taskCount)
		{
			var projects = BuildProjects(projectCount);
			var tasks = BuildTasks(taskCount);
			var outputDirectory = Path.Combine(_testDirectory, "output");
			var importDirectory = Path.Combine(_testDirectory, "import");

			if (Directory.Exists(outputDirectory) == false)
			{
				Directory.CreateDirectory(outputDirectory);
			}

			return new Configuration(projects, tasks, outputDirectory, importDirectory, string.Empty);
		}
		private static void CreateDataFile(string filePath, IEnumerable<Entry> entries)
		{
			using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				var fileWriter = new StreamWriter(stream);

				foreach (var entry in entries.Where(entry => entry.Hours > 0).OrderBy(x => x.Date))
				{
					fileWriter.WriteLine("{0},{1},{2:d},{3},{4}", entry.ResourceName, entry.Project, entry.Date, entry.Task, entry.Hours);
				}

				fileWriter.Close();
				stream.Close();
			}
		}
		private static void CreateImportFile(string fileDirectory, IEnumerable<Entry> entries)
		{
			if (Directory.Exists(fileDirectory) == false)
			{
				Directory.CreateDirectory(fileDirectory);
			}
			var filePath = Path.Combine(fileDirectory, Path.GetRandomFileName());
			using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				var fileWriter = new StreamWriter(stream);

				foreach (var entry in entries.Where(entry => entry.Hours > 0).OrderBy(x => x.Date))
				{
					fileWriter.WriteLine("{0},{1},{2:d},{3},{4}", entry.ResourceName, entry.Project, entry.Date, entry.Task, entry.Hours);
				}

				fileWriter.Close();
				stream.Close();
			}
		}
	}
}