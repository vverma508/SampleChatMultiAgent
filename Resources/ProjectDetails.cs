namespace SampleChatMultiAgent.Resources
{
    public class ProjectDetails
    {
        public string ProjectName { get; set; }
        public string TeamName { get; set; }
        public string Description { get; set; }
        public string DevLeadName { get; set; }
        public string ArchitectName { get; set; }
        public string ProductOwner { get; set; }

        public List<ProjectDetails> GetProjectDetails()
        {
            var projects = new List<ProjectDetails>();
            var random = new Random();

            string[] teamNames = { "Alpha", "Beta", "Gamma", "Delta", "Epsilon" };
            string[] devLeads = { "Alice", "Bob", "Charlie", "Diana", "Ethan" };
            string[] architects = { "Frank", "Grace", "Hank", "Ivy", "Jack" };
            string[] productOwners = { "Karen", "Leo", "Mona", "Nate", "Olivia" };

            for (int i = 1; i <= 100; i++)
            {
                var project = new ProjectDetails
                {
                    ProjectName = $"Project_{i}",
                    TeamName = teamNames[random.Next(teamNames.Length)],
                    Description = $"This is a description for Project_{i}.",
                    DevLeadName = devLeads[random.Next(devLeads.Length)],
                    ArchitectName = architects[random.Next(architects.Length)],
                    ProductOwner = productOwners[random.Next(productOwners.Length)]
                };

                projects.Add(project);
            }

            return projects;
        }

    }
}
