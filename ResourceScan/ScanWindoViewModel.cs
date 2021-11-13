using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using EnvDTE;
using EnvDTE80;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.Shell;

namespace ResourceScan
{
    public class ScanWindoViewModel
    {
        public string WWWROOTPath { get; set; }
        public string SolutionPath { get; set; }
        public string ProjectPath { get; set; }

        public ObservableCollection<ProjectEntity> Projects { get; set; } = new ObservableCollection<ProjectEntity>();

        public ICommand ScanProjectCommand { get; set; }

        public ICommand SyncCommand { get; set; }

        public ScanWindoViewModel()
        {
            this.ScanProjectCommand = new RelayCommand(ScanCmdAction);
            this.SyncCommand = new RelayCommand(SyncCmdAction);

            var dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var sol = dte2.Solution;
            this.SolutionPath = Path.GetDirectoryName(sol.FullName);
            this.ProjectPath = Path.Combine(this.SolutionPath, "shared", "HiYms.WebCharts.Shared");
            this.WWWROOTPath = Path.Combine(this.ProjectPath, "wwwroot");
        }

        public void SyncCmdAction()
        {
            try
            {
                var dir = new DirectoryInfo(WWWROOTPath);
                var selectedProjects = this.Projects.Where(p => p.IsChecked).ToList();
                var files = GetFileInfos(dir).Select(f => new LinkFileEntity(f)).ToList();

                foreach (var pro in selectedProjects)
                {
                    var xdoc = XDocument.Load(pro.FullName);
                    var xels = xdoc.Element("Project").Elements("ItemGroup");
                    var xel = xels.FirstOrDefault(x => x.Attribute("Label")?.Value == "EmbeddedResource");
                    if (xel == null)
                    {
                        xel = new XElement("ItemGroup", new XAttribute("Label", "EmbeddedResource"));
                        xdoc.Element("Project").Add(xel);
                    }
                    else
                    {
                        xel.RemoveNodes();
                    }

                    foreach (var file in files)
                    {
                        var path = file.FullName.Replace(this.ProjectPath, string.Empty);
                        xel.Add(new XElement("EmbeddedResource",
                                new XAttribute("Include", $"..\\HiYms.WebCharts.Shared\\{path}"),
                                new XAttribute("Link", $"{path}"),
                                new XElement("CopyToOutputDirectory", "Always")
                                ));
                    }

                    xdoc.Save(pro.FullName);
                }

                System.Windows.MessageBox.Show("内嵌资源文件同步完成。");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        public void ScanCmdAction()
        {
            var dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var sol = dte2.Solution;
            var projs = sol.Projects;
            foreach (var proj in sol)
            {
                var project = proj as Project;
                var projects = GetSolutionProjects(project);

                foreach (var item in projects)
                {
                    Projects.Add(item);
                }
            }
        }

        private IEnumerable<ProjectEntity> GetSolutionProjects(Project project)
        {
            var projects = new List<ProjectEntity>();
            if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
            {
                var innerProjects = GetSolutionFolderProjects(project);

                foreach (var pro in innerProjects)
                {
                    projects.AddRange(GetSolutionProjects(pro));
                }
            }
            else
            {
                projects.Add(new ProjectEntity
                {
                    Name = project.Name,
                    CurrProject = project,
                    FileName = project.FileName,
                    FullName = project.FullName,
                    IsChecked = false
                });
            }

            return projects;
        }

        private IEnumerable<Project> GetSolutionFolderProjects(Project project)
        {
            var projects = new List<Project>();
            var y = (project.ProjectItems as ProjectItems).Count;
            for (var i = 1; i <= y; i++)
            {
                var x = project.ProjectItems.Item(i).SubProject;
                var subProject = x as Project;
                if (subProject != null)
                {
                    projects.Add(subProject);
                }
            }

            return projects;
        }

        private IEnumerable<FileInfo> GetFileInfos(DirectoryInfo directory)
        {
            var files = new List<FileInfo>();

            var subDirs = directory.GetDirectories();
            var subFiles = directory.GetFiles();
            files.AddRange(subFiles);

            foreach (var dir in subDirs)
                files.AddRange(GetFileInfos(dir));

            return files;
        }
    }
}
