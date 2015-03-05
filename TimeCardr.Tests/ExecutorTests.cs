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
		private ILog _log;

		[SetUp]
		public void TestSetUp()
		{
			_log = LogManager.GetLogger("ExecutorTests");

			_testDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			var testDirectoryInfo = new DirectoryInfo(_testDirectory);
			if (testDirectoryInfo.Exists == false)
			{
				testDirectoryInfo.Create();
			}
		}
		[TearDown]
		public void TestTearDown()
		{
			var testDirectoryInfo = new DirectoryInfo(_testDirectory);
			if (testDirectoryInfo.Exists)
			{
				testDirectoryInfo.Delete(true);
			}
		}

		[Test]
		public void Execute_NoInputData_NoOutput()
		{
			var config = BuildConfiguration(0, 0);

			var results = Executor.Execute(config, _log);

			var outputFileInfo = new FileInfo(config.DataFile);

			results.ShouldBeEmpty();
			outputFileInfo.Exists.ShouldBe(true);
			outputFileInfo.Length.ShouldBe(0);
		}

		[Test]
		public void Execute_DataInput_OutputMatchesInput()
		{
			var config = BuildConfiguration(1, 1);
			var expectedEntries = BuildEntries(5, config.ResourceName, config.Projects, config.Tasks, false);
			CreateDataFile(config.DataFile, expectedEntries);
			var outputFileInfo = new FileInfo(config.DataFile);
			var expectedLength = outputFileInfo.Length;

			var results = Executor.Execute(config, _log);

			results[DateTime.Today].ShouldBe(expectedEntries, true);
			outputFileInfo.Length.ShouldBe(expectedLength);
		}

		//TODO: Test with import file, output matches import file
		//TODO: Test with data and import file, output combines records
		//TODO: Test with data and import files with collisions, data file wins
		//TODO: Test with no file input and user inputs, output is user inputs
		//TODO: Test with data input and user input, output combines inputs
		//TODO: Test with data input and user input with collisions, user input wins
		//TODO: Test with data input and import input, output combines inputs
		//TODO: Test with data input and import input with collisions, user input wins
		//TODO: Test with multiple runs, data is not duplicated

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

				result.Add(new Project(id, name));
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
		private static ICollection<Entry> BuildEntries(int entryCount, string resourceName, IList<Project> projects, IList<Task> tasks, bool includeZeroHours)
		{
			var result = new List<Entry>();

			while (result.Count < entryCount)
			{
				var date = DateTime.Today;
				var project = projects[result.Count % projects.Count].Id;
				var task = tasks[result.Count % tasks.Count].Id;
				var hours = result.Count % 8;

				result.Add(new Entry(date, resourceName, project, task, hours));
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

			var outputDirectoryInfo = new DirectoryInfo(outputDirectory);
			if (outputDirectoryInfo.Exists == false)
			{
				outputDirectoryInfo.Create();
			}

			return new Configuration(projects, tasks, outputDirectory, importDirectory);
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
	}
}