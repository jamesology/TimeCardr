namespace TimeCardr
{
	public class Project
	{
		public string Id { get; private set; }
		public string Name { get; private set; }
		public bool Active { get; private set; }

		public Project(string id, string name, bool active)
		{
			Name = name;
			Id = id;
			Active = active;
		}
	}
}
