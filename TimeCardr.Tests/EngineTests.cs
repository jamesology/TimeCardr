using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using log4net;
using NUnit.Framework;
using Shouldly;

namespace TimeCardr.Tests
{
	[TestFixture]
	class EngineTests
	{
		private string _testDirectory;

		[SetUp]
		public void TestSetUp()
		{
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

		[Test]
		public void Execute_NoInputData_NoOutput()
		{
			var log = LogManager.GetLogger("EngineTests");
			var config = BuildConfiguration(0, 0);

			var results = Engine.Execute(config, log);

			var outputFileInfo = new FileInfo(config.DataFile);

			results.ShouldBeEmpty();
			outputFileInfo.Exists.ShouldBe(true);
			outputFileInfo.Length.ShouldBe(0);
		}

		//TODO: Test with data file, output matches data file
		//TODO: Test with import file, output matches import file
		//TODO: Test with data and import file, output combines records
		//TODO: Test with data and import files with collisions, data file wins
		//TODO: Test with no file input and user inputs, output is user inputs
		//TODO: Test with data input and user input, output combines inputs
		//TODO: Test with data input and user input with collisions, user input wins
		//TODO: Test with data input and import input, output combines inputs
		//TODO: Test with data input and import input with collisions, user input wins
		//TODO: Test with multiple runs, data is not duplicated
	}
}