using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace FalconOrchestrator.Forensics
{
    public class TaskManagement
    {
        private PSRemoting _psr;

        public TaskManagement(PSRemoting psr)
        {
            _psr = psr;
        }

        public List<FalconOrchestrator.Forensics.Tasks> ListTasks(string command)
        {
            List<Tasks> result = new List<Tasks>();

            foreach (PSObject line in _psr.ExecuteCommand(command))
            {
                    Tasks task = new Tasks();
                if (line.Properties["NextRunTime"].Value != null)
                {
                    task.NextRunTime = line.Properties["NextRunTime"].Value.ToString();
                }
                if (line.Properties["Author"].Value != null)
                {
                    task.Author = line.Properties["Author"].Value.ToString();
                }
                if (line.Properties["Trigger"].Value != null)
                {
                    task.Trigger = line.Properties["Trigger"].Value.ToString();
                }
                if (line.Properties["State"].Value != null)
                {
                    task.State = line.Properties["State"].Value.ToString();
                }
                if (line.Properties["UserId"].Value != null)
                {
                    task.UserId = line.Properties["UserId"].Value.ToString();
                }
                if (line.Properties["ComputerName"].Value != null)
                {
                    task.ComputerName = line.Properties["ComputerName"].Value.ToString();
                }
                if (line.Properties["Name"].Value != null)
                {
                    task.Name = line.Properties["Name"].Value.ToString();
                }
                if (line.Properties["LastRunTime"].Value != null)
                {
                    task.LastRunTime = line.Properties["LastRunTime"].Value.ToString();
                }
                if (line.Properties["LastTaskResult"].Value != null)
                {
                    task.LastTaskResult = line.Properties["LastTaskResult"].Value.ToString();
                }
                if (line.Properties["Description"].Value != null)
                {
                    task.Description = line.Properties["Description"].Value.ToString();
                }
                if (line.Properties["NumberOfMissedRuns"].Value != null)
                {
                    task.NumberofMissedRuns = line.Properties["NumberOfMissedRuns"].Value.ToString();
                }
                if (line.Properties["Enabled"].Value != null)
                {
                    task.Enabled = line.Properties["Enabled"].Value.ToString();
                }
                if (line.Properties["Path"].Value != null)
                {
                    task.Path = line.Properties["Path"].Value.ToString();
                }


                result.Add(task);
            }
            return result;
        }

        public void Kill(string path)
        {
            _psr.ExecuteCommand("C:\\Windows\\System32\\schtasks.exe /Change / TN "+ path + " /DISABLE");
        }
        public void Revive(string path)
        {
            _psr.ExecuteCommand("C:\\Windows\\System32\\schtasks.exe /Change / TN " + path + " /ENABLE");
        }

    }
}
