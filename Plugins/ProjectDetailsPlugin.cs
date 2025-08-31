using Microsoft.SemanticKernel;
using SampleChatMultiAgent.Resources;
using System.ComponentModel;

namespace SampleChatMultiAgent.Plugins
{
    public class ProjectDetailsPlugin
    {
        [KernelFunction("GetProjectDetails"), Description("This method gets the list of all the project details available")]
        public List<ProjectDetails> GetProjectDetails()
        {
            ProjectDetails projectDetails = new ProjectDetails();

            return projectDetails.GetProjectDetails();
        }

        [KernelFunction("GetProjectDetailsByName"), Description("This method gets the project details by project name")]
        public ProjectDetails GetProjectDetailsByName([Description("Name of the project")]string projectName)
        {
            ProjectDetails projectDetails = new ProjectDetails();

            return projectDetails.GetProjectDetails().FirstOrDefault(x=>x.ProjectName.Equals(projectName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
