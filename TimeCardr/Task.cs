namespace TimeCardr
{
	public class Task
	{
		public string Id { get; private set; }
		public string Name { get; private set; }
		public object Description { get; private set; }

		public Task(string id, string name, object description)
		{
			Description = description;
			Name = name;
			Id = id;
		}
	}
}
