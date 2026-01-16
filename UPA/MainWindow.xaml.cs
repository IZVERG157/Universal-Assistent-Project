using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml.Linq;

namespace UPA
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Directory.CreateDirectory(_projectsPath);
            LoadAllLists();
        }
        // Block paths
        private static string _roaming = Environment.GetEnvironmentVariable("APPDATA");
        private string _currentProjectPath = string.Empty;
        private string _projectsPath = _roaming + "\\UPA\\Projects";
        private string _projectsListPath = _roaming + "\\UPA\\ProjectsList.txt";
        private string _masterToDoTasksPath = _roaming + "\\UPA\\MasterToDoTasks.txt";

        // Block selects
        private TextBlock _chosenMasterToDoTask = null;
        private StackPanel _chosenProjectListsProject = null;
        private Border _chosenProjectSomeList = null;
        private TextBlock _chosenProjectSomeListTask = null;
        private StackPanel _chosenProjectVersion = null;

        private void LoadAllLists()
        {
            // Master to-do list
            if (File.Exists(_masterToDoTasksPath))
            {
                foreach (string text in File.ReadAllLines(_masterToDoTasksPath))
                {
                    CreateMasterToDoTask(text);
                }
            }

            // Projects list
            if (File.Exists(_projectsListPath))
            {
                string[] projects = File.ReadAllLines(_projectsListPath);
                if (projects.Length > 1)
                {
                    string projectName = string.Empty;
                    string projectDescription = string.Empty;
                    for (int i = 0; i < projects.Length; i++)
                    {
                        if (i % 2 == 0)
                        {
                            projectName = projects[i];
                            projectDescription = projects[i + 1];
                            CreateProject(projectName, projectDescription);
                        }
                        i++;
                    }
                }
                if (itemsControlProjectsList.HasItems) textBlockNoProjectsFoundProjectsList.Visibility = Visibility.Collapsed;
            }
        }
        private void SaveTasks(ItemsControl itemsControl, string path)
        {
            string text = string.Empty;
            foreach (Border border in itemsControl.Items)
            {
                TextBlock textBlock = border.Child as TextBlock;
                text += textBlock.Text + "\n";
            }
            File.WriteAllText(path, text);
        }
        private void BorderDrive_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
        }

        // Block Exit/Collapse app
        private void Exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void Collapse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Block Master to-do
        private void OpenMasterToDoAddTask_Click(object sender, RoutedEventArgs e)
        {
            OpenBorderOverWindow(borderMasterToDoAddTask);
        }
        private void AcceptMasterToDoAddTask_Click(object sender, RoutedEventArgs e)
        {
            CreateMasterToDoTask(textBoxMasterToDoTextAddTask.Text);
            SaveTasks(itemsControlMasterToDoTasks, _masterToDoTasksPath);
        }
        private void CancelMasterToDoAddTask_Click(object sender, RoutedEventArgs e)
        {
            CloseBorderOverWindow(borderMasterToDoAddTask);
        }

        private void OpenMasterToDoEditTask_Click(object sender, RoutedEventArgs e)
        {
            if (_chosenMasterToDoTask == null)
            {
                MessageBox.Show("For edit choose task");
                return;
            }

            textBoxMasterToDoTextEditTask.Text = _chosenMasterToDoTask.Text;
            OpenBorderOverWindow(borderMasterToDoEditTask);
        }
        private void AcceptMasterToDoEditTask_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxMasterToDoTextEditTask.Text.Replace(" ", "") == "")
            {
                MessageBox.Show("Task text can't be empty");
                return;
            }

            _chosenMasterToDoTask.Text = textBoxMasterToDoTextEditTask.Text;
            textBoxMasterToDoTextEditTask.Text = string.Empty;

            Border parentBorder = _chosenMasterToDoTask.Parent as Border;
            parentBorder.ClearValue(Border.BorderBrushProperty);
            parentBorder.ClearValue(Border.BorderThicknessProperty);

            _chosenMasterToDoTask = null;

            CloseBorderOverWindow(borderMasterToDoEditTask);

            SaveTasks(itemsControlMasterToDoTasks, _masterToDoTasksPath);
        }
        private void CancelMasterToDoEditTask_Click(object sender, RoutedEventArgs e)
        {
            CloseBorderOverWindow(borderMasterToDoEditTask);
        }

        private void MasterToDoDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (_chosenMasterToDoTask == null)
            {
                MessageBox.Show("For delete choose task");
                return;
            }

            itemsControlMasterToDoTasks.Items.Remove(_chosenMasterToDoTask.Parent as Border);

            if (!itemsControlMasterToDoTasks.HasItems) textBlockNoTasksFoundMasterToDo.Visibility = Visibility.Visible;
            SaveTasks(itemsControlMasterToDoTasks, _masterToDoTasksPath);
        }

        private void MasterToDoTask_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SomeTask_MouseLeftButtonDown(sender, ref _chosenMasterToDoTask);
        }

        private void CreateMasterToDoTask(string text)
        {
            if (text.Replace(" ", "") == "") return;

            Border border = new Border()
            {
                Style = (Style)this.FindResource("PinkGradientBorderStyle"),
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#121212"),
                Margin = new Thickness(5, 3, 5, 0),
                CornerRadius = new CornerRadius(6)
            };
            border.MouseLeftButtonDown += MasterToDoTask_MouseLeftButtonDown;

            TextBlock textBlock = new TextBlock()
            {
                FontFamily = new FontFamily("Verdana"),
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                Text = text
            };

            border.Child = textBlock;
            itemsControlMasterToDoTasks.Items.Add(border);

            CloseBorderOverWindow(borderMasterToDoAddTask);

            textBoxMasterToDoTextAddTask.Text = string.Empty;

            if (itemsControlMasterToDoTasks.HasItems) textBlockNoTasksFoundMasterToDo.Visibility = Visibility.Collapsed;
        }

        // Block Projects list
        private void OpenProjectsListAddProject_Click(object sender, RoutedEventArgs e)
        {
            OpenBorderOverWindow(borderProjectListAddProject);
        }
        private void AcceptProjectsListAddProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < itemsControlProjectsList.Items.Count; i++)
                {
                    if ((((itemsControlProjectsList.Items[i] as Border).Child as StackPanel).Children[0] as TextBlock).Text == textBoxProjectsListAddProjectName.Text)
                    {
                        MessageBox.Show("Already there is such project");
                        return;
                    }
                }

                if (!CreateProject(textBoxProjectsListAddProjectName.Text, textBoxProjectsListAddProjectDescription.Text)) return;

                SaveProjects(itemsControlProjectsList, _projectsListPath);

                CloseBorderOverWindow(borderProjectListAddProject);

                textBoxProjectsListAddProjectName.Text = string.Empty;
                textBoxProjectsListAddProjectDescription.Text = "This field is not required";

                if (itemsControlProjectsList.HasItems) textBlockNoProjectsFoundProjectsList.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void CancelProjectsListAddProject_Click(object sender, RoutedEventArgs e)
        {
            CloseBorderOverWindow(borderProjectListAddProject);
        }

        private bool CreateProject(string textName, string textDescription)
        {
            if (textName.Replace(" ", "") == "")
            {
                MessageBox.Show("Project name can't be empty");
                return false;
            }

            if (char.IsDigit(textName[0]))
            {
                MessageBox.Show("Project name can't startswith number");
                return false;
            }

            Border border = new Border()
            {
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#121212"),
                Style = (Style)this.FindResource("PinkGradientBorderStyle"),
                Margin = new Thickness(5, 4, 5, 0),
                CornerRadius = new CornerRadius(6),
                Name = textName.Replace(" ", "")
            };
            border.MouseLeftButtonDown += ProjectsListProject_MouseLeftButtonDown;

            StackPanel stackPanel = new StackPanel();

            TextBlock projectName = new TextBlock()
            {
                FontFamily = new FontFamily("Verdana"),
                FontWeight = FontWeights.DemiBold,
                Style = (Style)this.FindResource("PinkGradientTextStyle"),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 8, 5, 0),
                Text = textName
            };

            TextBlock projectDescription = new TextBlock()
            {
                FontFamily = new FontFamily("Verdana"),
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 5, 5, 8),
                Text = textDescription
            };

            if (textDescription.Replace(" ", "") == ""
                || textDescription == "This field is not required")
            {
                stackPanel.Children.Add(projectName);
                projectName.Margin = new Thickness(5, 8, 5, 8);
            }
            else
            {
                stackPanel.Children.Add(projectName);
                stackPanel.Children.Add(projectDescription);
            }

            border.Child = stackPanel;
            itemsControlProjectsList.Items.Add(border);

            return true;
        }
        private void SaveProjects(ItemsControl itemsControl, string path)
        {
            string text = string.Empty;
            foreach (Border border in itemsControl.Items)
            {
                StackPanel stackPanel = border.Child as StackPanel;
                TextBlock textBlockName = stackPanel.Children[0] as TextBlock;
                if (stackPanel.Children.Count > 1)
                {
                    TextBlock textBlockDescription = stackPanel.Children[1] as TextBlock;
                    text += textBlockName.Text.Replace(" ", "") + "\n" + textBlockDescription.Text + "\n";
                }
                else
                {
                    text += textBlockName.Text + "\n\n";
                }
            }
            File.WriteAllText(path, text);
        }

        private void OpenProjectsListEditProject_Click(object sender, RoutedEventArgs e)
        {
            if (_chosenProjectListsProject == null)
            {
                MessageBox.Show("Please choose project for edit");
                return;
            }

            textBoxProjectsListEditProjectName.Text = (_chosenProjectListsProject.Children[0] as TextBlock).Text;
            if (_chosenProjectListsProject.Children.Count > 1) textBoxProjectsListEditProjectDescription.Text = (_chosenProjectListsProject.Children[1] as TextBlock).Text;
            else textBoxProjectsListEditProjectDescription.Text = string.Empty;

            OpenBorderOverWindow(borderProjectListEditProject);
        }
        private void AcceptProjectsListEditProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textBoxProjectsListEditProjectName.Text.Replace(" ", "") == "")
                {
                    MessageBox.Show("Project name can't be empty");
                    return;
                }
                else if (char.IsDigit(textBoxProjectsListEditProjectName.Text[0]))
                {
                    MessageBox.Show("Project name dont startswith number");
                    return;
                }

                string oldName = (_chosenProjectListsProject.Children[0] as TextBlock).Text.Replace(" ", "");
                string newName = textBoxProjectsListEditProjectName.Text.Replace(" ", "");
                for (int i = 0; i < itemsControlProjectsList.Items.Count; i++)
                {
                    if ((_chosenProjectListsProject.Children[0] as TextBlock).Text == (((itemsControlProjectsList.Items[i] as Border).Child as StackPanel).Children[0] as TextBlock).Text) continue;
                    if ((((itemsControlProjectsList.Items[i] as Border).Child as StackPanel).Children[0] as TextBlock).Text == newName)
                    {
                        MessageBox.Show("Already there is such project");
                        return;
                    }
                }

                Directory.CreateDirectory(_projectsPath + "\\" + newName);
                if (Directory.Exists(_projectsPath + "\\" + oldName)) MoveAllFiles(_projectsPath + "\\" + oldName, _projectsPath + "\\" + newName);
                if (Directory.Exists(_projectsPath + "\\" + oldName)) Directory.Delete(_projectsPath + "\\" + oldName);

                for (int i = 0; i < itemsControlProjectsList.Items.Count; i++)
                {
                    if ((itemsControlProjectsList.Items[i] as Border).Name == oldName)
                    {
                        (itemsControlProjectsList.Items[i] as Border).Name = newName;
                    }
                }

                TextBlock projectName = _chosenProjectListsProject.Children[0] as TextBlock;
                TextBlock projectDescription = null;
                if (_chosenProjectListsProject.Children.Count > 1) projectDescription = _chosenProjectListsProject.Children[1] as TextBlock;
                StackPanel stackPanel = _chosenProjectListsProject;

                if (textBoxProjectsListEditProjectDescription.Text.Replace(" ", "") == "")
                {
                    projectName.Text = textBoxProjectsListEditProjectName.Text;
                    projectName.Margin = new Thickness(5, 8, 5, 8);
                    stackPanel.Children.Remove(projectDescription);
                }
                else if (projectDescription == null)
                {
                    projectName.Margin = new Thickness(5, 8, 5, 0);

                    projectDescription = new TextBlock()
                    {
                        FontFamily = new FontFamily("Verdana"),
                        Foreground = Brushes.White,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(5, 5, 5, 8),
                        Text = textBoxProjectsListEditProjectDescription.Text
                    };

                    _chosenProjectListsProject.Children.Add(projectDescription);
                }
                else
                {
                    projectName.Text = textBoxProjectsListEditProjectName.Text;
                    projectDescription.Text = textBoxProjectsListEditProjectDescription.Text;
                }

                textBoxProjectsListEditProjectName.Text = string.Empty;
                textBoxProjectsListEditProjectDescription.Text = string.Empty;

                Border parentBorder = _chosenProjectListsProject.Parent as Border;
                parentBorder.ClearValue(Border.BorderBrushProperty);
                parentBorder.ClearValue(Border.BorderThicknessProperty);

                CloseBorderOverWindow(borderProjectListEditProject);

                SaveProjects(itemsControlProjectsList, _projectsListPath);

                _chosenProjectListsProject = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void CancelProjectsListEditProject_Click(object sender, RoutedEventArgs e)
        {
            CloseBorderOverWindow(borderProjectListEditProject);
        }

        private void ProjectsListDeleteProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_chosenProjectListsProject == null)
                {
                    MessageBox.Show("Please choose project for delete");
                    return;
                }

                if (Directory.Exists(_projectsPath + "\\" + (_chosenProjectListsProject.Parent as Border).Name.Replace(" ", ""))) Directory.Delete(_projectsPath + "\\" + (_chosenProjectListsProject.Parent as Border).Name.Replace(" ", ""), true);
                itemsControlProjectsList.Items.Remove(_chosenProjectListsProject.Parent);

                if (!itemsControlProjectsList.HasItems) textBlockNoProjectsFoundProjectsList.Visibility = Visibility.Visible;
                SaveProjects(itemsControlProjectsList, _projectsListPath);

                itemsControlMainProjectMadeTasks.Items.Clear();
                textBlockNoTasksFoundMainProjectMade.Visibility = Visibility.Visible;

                _chosenProjectListsProject = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ProjectsListProject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel nowChosenProjectsListProject = null;

            try
            {
                if (sender is TextBlock) nowChosenProjectsListProject = (sender as TextBlock).Parent as StackPanel;
                else nowChosenProjectsListProject = (sender as Border).Child as StackPanel;
            }
            catch { }

            if (_chosenProjectListsProject == null)
            {
                Border parentBorder = nowChosenProjectsListProject.Parent as Border;
                parentBorder.BorderBrush = (SolidColorBrush)this.FindResource("MaxDarkPink");
                parentBorder.BorderThickness = new Thickness(1);

                _chosenProjectListsProject = nowChosenProjectsListProject;

                itemsControlMainProjectMadeTasks.Items.Clear();
                textBlockNoTasksFoundMainProjectMade.Visibility = Visibility.Visible;

                List<string> strings = null;
                string chosenProjectMadePath = $"{_projectsPath}\\{(_chosenProjectListsProject.Parent as Border).Name.Replace(" ", "")}\\Made.txt";
                if (File.Exists(chosenProjectMadePath)) strings = File.ReadAllLines(chosenProjectMadePath).ToList();
                if (strings != null) foreach (string str in strings) CreateLastMadeTask(str);
                if (itemsControlMainProjectMadeTasks.HasItems) textBlockNoTasksFoundMainProjectMade.Visibility = Visibility.Collapsed;
            }
            else if (_chosenProjectListsProject != null && nowChosenProjectsListProject != _chosenProjectListsProject)
            {
                Border oldParentBorder = _chosenProjectListsProject.Parent as Border;
                oldParentBorder.ClearValue(Border.BorderBrushProperty);
                oldParentBorder.ClearValue(Border.BorderThicknessProperty);

                Border nowParentBorder = nowChosenProjectsListProject.Parent as Border;
                nowParentBorder.BorderBrush = (SolidColorBrush)this.FindResource("MaxDarkPink");
                nowParentBorder.BorderThickness = new Thickness(1);

                _chosenProjectListsProject = nowChosenProjectsListProject;

                itemsControlMainProjectMadeTasks.Items.Clear();
                textBlockNoTasksFoundMainProjectMade.Visibility = Visibility.Visible;

                List<string> strings = null;
                string chosenProjectMadePath = $"{_projectsPath}\\{(_chosenProjectListsProject.Parent as Border).Name.Replace(" ", "")}\\Made.txt";
                if (File.Exists(chosenProjectMadePath)) strings = File.ReadAllLines(chosenProjectMadePath).ToList();
                if (strings != null) foreach (string str in strings) CreateLastMadeTask(str);
                if (itemsControlMainProjectMadeTasks.HasItems) textBlockNoTasksFoundMainProjectMade.Visibility = Visibility.Collapsed;
            }
            else
            {
                string[] projectsNames = File.ReadAllLines(_projectsListPath);
                for(int i = 0; i < projectsNames.Length; i++)
                {
                    if (projectsNames[i].Replace(" ", "") == (_chosenProjectListsProject.Parent as Border).Name)
                    {
                        textBlockCurrentProject.Text = "Current project: " + projectsNames[i];
                        i = projectsNames.Length - 1;
                    }
                }

                _currentProjectPath = $"{_projectsPath}\\{(_chosenProjectListsProject.Parent as Border).Name}";
                Directory.CreateDirectory(_currentProjectPath);
                LoadProjectLists();

                stackPanelMainWindow.Visibility = Visibility.Collapsed;
                GridOpenProjectWindow.Visibility = Visibility.Visible;

                itemsControlMainProjectMadeTasks.Items.Clear();
                textBlockNoTasksFoundMainProjectMade.Visibility = Visibility.Visible;

                Border parentBorder = _chosenProjectListsProject.Parent as Border;
                parentBorder.ClearValue(Border.BorderBrushProperty);
                parentBorder.ClearValue(Border.BorderThicknessProperty);

                _chosenProjectListsProject = null;

            }
        }

        private void LoadProjectLists()
        {
            if (File.Exists($"{_currentProjectPath}\\ToDo.txt"))
            {
                foreach (string text in File.ReadAllLines($"{_currentProjectPath}\\ToDo.txt"))
                {
                    CreateSomeTask(text, (((ProjectToDoList.Child as Grid).Children[1] as StackPanel).Children[2] as ScrollViewer).Content as ItemsControl, textBlockNoTasksFoundProjectToDo);
                }
            }

            if (File.Exists($"{_currentProjectPath}\\ToFix.txt"))
            {
                foreach (string text in File.ReadAllLines($"{_currentProjectPath}\\ToFix.txt"))
                {
                    CreateSomeTask(text, (((ProjectToFixList.Child as Grid).Children[1] as StackPanel).Children[2] as ScrollViewer).Content as ItemsControl, textBlockNoTasksFoundProjectToFix);
                }
            }

            if (File.Exists($"{_currentProjectPath}\\Made.txt"))
            {
                foreach (string text in File.ReadAllLines($"{_currentProjectPath}\\Made.txt"))
                {
                    CreateSomeTask(text, (((ProjectMadeList.Child as Grid).Children[1] as StackPanel).Children[2] as ScrollViewer).Content as ItemsControl, textBlockNoTasksFoundProjectMade);
                }
            }

            if (File.Exists($"{_currentProjectPath}\\Versions.txt"))
            {
                string[] text = File.ReadAllLines($"{_currentProjectPath}\\Versions.txt");
                string textVersion = string.Empty;
                string textDescription = string.Empty;
                for (int i = 0; i < text.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        textVersion = text[i];
                        textDescription = text[i + 1];
                        i++;
                    }
                    CreateVersion(textVersion, textDescription, (((ProjectVersionsList.Child as Grid).Children[1] as StackPanel).Children[2] as ScrollViewer).Content as ItemsControl, textBlockNoVersionsFoundProject);
                }

            }
        }

        private void CreateLastMadeTask(string text)
        {
            if (text.Replace(" ", "") == "") return;

            Border border = new Border()
            {
                Style = (Style)this.FindResource("PinkGradientBorderStyle"),
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#121212"),
                Margin = new Thickness(5, 3, 5, 0),
                CornerRadius = new CornerRadius(6)
            };

            TextBlock textBlock = new TextBlock()
            {
                FontFamily = new FontFamily("Verdana"),
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                Text = text
            };

            border.Child = textBlock;
            itemsControlMainProjectMadeTasks.Items.Add(border);

            if (itemsControlMasterToDoTasks.HasItems) textBlockNoTasksFoundMasterToDo.Visibility = Visibility.Collapsed;
        }
        private void MoveAllFiles(string sourceDirectory, string destDirectory)
        {
            foreach (string file in Directory.GetFiles(sourceDirectory)) File.Move(file, $"{destDirectory}\\{Path.GetFileName(file)}");
        }

        // Block border over window
        private void OpenBorderOverWindow(Border border)
        {
            GeneralBorder.IsEnabled = false;
            GeneralBorder.Effect = new BlurEffect()
            {
                Radius = 5,
            };

            border.Visibility = Visibility.Visible;
        }
        private void CloseBorderOverWindow(Border border)
        {
            GeneralBorder.IsEnabled = true;
            GeneralBorder.ClearValue(Border.EffectProperty);

            border.Visibility = Visibility.Collapsed;
        }

        // Block opened project (Project some list)
        private void ProjectOpenSomeListAddSomething_Click(object sender, RoutedEventArgs e)
        {
            if (_chosenProjectSomeList == null)
            {
                MessageBox.Show("For adding please choose one of the lists");
                return;
            }

            if (_chosenProjectSomeList.Name == "ProjectVersionsList")
                OpenBorderOverWindow(borderProjectVersionsListAddVersion);
            else
                OpenBorderOverWindow(borderProjectSomeListAddSomething);
        }
        private void AcceptProjectSomeListAddSomething_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl itemsControl = (((_chosenProjectSomeList.Child as Grid).Children[1] as StackPanel).Children[2] as ScrollViewer).Content as ItemsControl;
            TextBlock textBlockNoResultFoundSomething = (_chosenProjectSomeList.Child as Grid).Children[0] as TextBlock;

            if (!CreateSomeTask(textBoxProjectTextAddSomething.Text, itemsControl, textBlockNoResultFoundSomething)) return;
            switch (_chosenProjectSomeList.Name)
            {
                case "ProjectToDoList":
                    SaveTasks(itemsControl, $"{_currentProjectPath}\\ToDo.txt");
                    break;
                case "ProjectToFixList":
                    SaveTasks(itemsControl, $"{_currentProjectPath}\\ToFix.txt");
                    break;
                case "ProjectMadeList":
                    SaveTasks(itemsControl, $"{_currentProjectPath}\\Made.txt");
                    break;
            }
            _chosenProjectSomeList.ClearValue(Border.StyleProperty);
            _chosenProjectSomeList = null;
        }
        private void CancelProjectSomeListAddSomething_Click(object sender, RoutedEventArgs e)
        {
            CloseBorderOverWindow(borderProjectSomeListAddSomething);
        }

        private void ProjectEditSomething_Click(object sender, RoutedEventArgs e)
        {
            if (_chosenProjectSomeListTask == null && _chosenProjectVersion == null)
            {
                MessageBox.Show("For edit choose task or version!");
                return;
            }
            else if (_chosenProjectSomeListTask == null && _chosenProjectVersion != null)
            {
                textBoxProjectVersionsListVersionEditTextVersion.Text = (_chosenProjectVersion.Children[0] as TextBlock).Text;
                textBoxProjectVersionsListVersionEditTextDescription.Text = (_chosenProjectVersion.Children[1] as TextBlock).Text;
                OpenBorderOverWindow(borderProjectVersionsListEditVersion);
            }
            else if (_chosenProjectSomeListTask != null && _chosenProjectVersion == null)
            {
                textBoxProjectTextEditSomething.Text = _chosenProjectSomeListTask.Text;
                OpenBorderOverWindow(borderProjectSomeListEditSomething);
            }
        }
        private void AcceptProjectSomeListEditSomething_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textBoxProjectTextEditSomething.Text.Replace(" ", "") == "")
                {
                    MessageBox.Show("Field shouldn't be empty!");
                    return;
                }
                _chosenProjectSomeListTask.Text = textBoxProjectTextEditSomething.Text;

                ItemsControl itemsControl = (_chosenProjectSomeListTask.Parent as Border).Parent as ItemsControl;

                _chosenProjectSomeList = (((((_chosenProjectSomeListTask.Parent as Border).Parent as ItemsControl).Parent as ScrollViewer).Parent as StackPanel).Parent as Grid).Parent as Border;
                switch (_chosenProjectSomeList.Name)
                {
                    case "ProjectToDoList":
                        SaveTasks(itemsControl, $"{_currentProjectPath}\\ToDo.txt");
                        break;
                    case "ProjectToFixList":
                        SaveTasks(itemsControl, $"{_currentProjectPath}\\ToFix.txt");
                        break;
                    case "ProjectMadeList":
                        SaveTasks(itemsControl, $"{_currentProjectPath}\\Made.txt");
                        break;
                }
                (_chosenProjectSomeListTask.Parent as Border).ClearValue(Border.BorderThicknessProperty);
                (_chosenProjectSomeListTask.Parent as Border).ClearValue(Border.BorderBrushProperty);
                (_chosenProjectSomeListTask.Parent as Border).Style = (Style)FindResource("PinkGradientBorderStyle");
                _chosenProjectSomeListTask = null;
                _chosenProjectSomeList = null;

                CloseBorderOverWindow(borderProjectSomeListEditSomething);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void CancelProjectSomeListEditSomething_Click(object sender, RoutedEventArgs e)
        {
            CloseBorderOverWindow(borderProjectSomeListEditSomething);
        }

        private void ProjectDeleteSomething_Click(object sender, RoutedEventArgs e)
        {
            if (_chosenProjectSomeListTask == null && _chosenProjectVersion == null)
            {
                MessageBox.Show("Выберите для начало задачу либо версию!");
                return;
            }
            else if (_chosenProjectSomeListTask != null && _chosenProjectVersion == null)
            {
                Border border = _chosenProjectSomeListTask.Parent as Border;
                ItemsControl itemsControl = border.Parent as ItemsControl;

                itemsControl.Items.Remove(border);

                if (!itemsControl.HasItems) ((((itemsControl.Parent as ScrollViewer).Parent as StackPanel).Parent as Grid).Children[0] as TextBlock).Visibility = Visibility.Visible;

                _chosenProjectSomeList = (((itemsControl.Parent as ScrollViewer).Parent as StackPanel).Parent as Grid).Parent as Border;
                switch (_chosenProjectSomeList.Name)
                {
                    case "ProjectToDoList":
                        SaveTasks(itemsControl, $"{_currentProjectPath}\\ToDo.txt");
                        break;
                    case "ProjectToFixList":
                        SaveTasks(itemsControl, $"{_currentProjectPath}\\ToFix.txt");
                        break;
                    case "ProjectMadeList":
                        SaveTasks(itemsControl, $"{_currentProjectPath}\\Made.txt");
                        break;
                }
                _chosenProjectSomeList = null;
                _chosenProjectSomeListTask = null;
            }
            else
            {
                Border border = _chosenProjectVersion.Parent as Border;
                ItemsControl itemsControl = border.Parent as ItemsControl;

                itemsControl.Items.Remove(border);

                if (!itemsControl.HasItems) ((((itemsControl.Parent as ScrollViewer).Parent as StackPanel).Parent as Grid).Children[0] as TextBlock).Visibility = Visibility.Visible;
                SaveVersions(itemsControl, $"{_currentProjectPath}\\Versions.txt");

                _chosenProjectVersion = null;
            }
        }

        private void AcceptProjectVersionsListVersionAdd_Click(object sender, RoutedEventArgs e)
        {
            ItemsControl itemsControl = (((_chosenProjectSomeList.Child as Grid).Children[1] as StackPanel).Children[2] as ScrollViewer).Content as ItemsControl;
            TextBlock textBlockNoResultFoundVersions = (_chosenProjectSomeList.Child as Grid).Children[0] as TextBlock;

            if (!CreateVersion(textBoxProjectVersionsListVersionAddTextVersion.Text, textBoxProjectVersionsListVersionAddTextDescription.Text, itemsControl, textBlockNoResultFoundVersions)) return;
            SaveVersions(itemsControl, $"{_currentProjectPath}\\Versions.txt");
            _chosenProjectSomeList.ClearValue(Border.StyleProperty);
            _chosenProjectSomeList = null;

            CloseBorderOverWindow(borderProjectVersionsListAddVersion);
        }
        private void CancelProjectVersionsListVersionAdd_Click(object sender, RoutedEventArgs e)
        {
            CloseBorderOverWindow(borderProjectVersionsListAddVersion);
        }

        private void AcceptProjectVersionsListVersionEdit_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxProjectVersionsListVersionEditTextVersion.Text.Replace(" ", "") == "" || textBoxProjectVersionsListVersionEditTextDescription.Text.Replace(" ", "") == "")
            {
                MessageBox.Show("Field shouldn't be empty!");
                return;
            }

            (_chosenProjectVersion.Children[0] as TextBlock).Text = textBoxProjectVersionsListVersionEditTextVersion.Text;
            (_chosenProjectVersion.Children[1] as TextBlock).Text = textBoxProjectVersionsListVersionEditTextDescription.Text;
            CloseBorderOverWindow(borderProjectVersionsListEditVersion);
            SaveVersions((_chosenProjectVersion.Parent as Border).Parent as ItemsControl, $"{_currentProjectPath}\\Versions.txt");

            (_chosenProjectVersion.Parent as Border).ClearValue(Border.BorderThicknessProperty);
            (_chosenProjectVersion.Parent as Border).ClearValue(Border.BorderBrushProperty);
            (_chosenProjectVersion.Parent as Border).Style = (Style)FindResource("PinkGradientBorderStyle");
            _chosenProjectVersion = null;
        }
        private void CancelProjectVersionsListVersionEdit_Click(object sender, RoutedEventArgs e)
        {
            CloseBorderOverWindow(borderProjectVersionsListEditVersion);
        }

        private void SaveVersions(ItemsControl itemsControl, string path)
        {
            string text = string.Empty;
            foreach (Border border in itemsControl.Items)
            {
                StackPanel stackPanel = border.Child as StackPanel;
                TextBlock textBlockVersion = stackPanel.Children[0] as TextBlock;
                if (stackPanel.Children.Count > 1)
                {
                    TextBlock textBlockDescription = stackPanel.Children[1] as TextBlock;
                    text += textBlockVersion.Text + "\n" + textBlockDescription.Text + "\n";
                }
                else
                {
                    text += textBlockVersion.Text + "\n\n";
                }
            }
            File.WriteAllText(path, text);
        }

        private void ProjectList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;

            if (_chosenProjectSomeList == null)
            {
                _chosenProjectSomeList = border;
                border.Style = (Style)this.FindResource("PinkGradientBorderStyle");
            }
            else if (_chosenProjectSomeList != null && border != _chosenProjectSomeList)
            {
                _chosenProjectSomeList.ClearValue(Border.StyleProperty);
                border.Style = (Style)this.FindResource("PinkGradientBorderStyle");
                _chosenProjectSomeList = border;
            }
            else if (_chosenProjectSomeList != null)
            {
                _chosenProjectSomeList.ClearValue(Border.StyleProperty);
                _chosenProjectSomeList = null;
            }
        }
        private void ProjectVersionsListVersion_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel nowChosenProjectVersion = null;

            if (sender is TextBlock) nowChosenProjectVersion = (sender as TextBlock).Parent as StackPanel;
            else nowChosenProjectVersion = (sender as Border).Child as StackPanel;

            if (_chosenProjectVersion == null)
            {
                Border parentBorder = nowChosenProjectVersion.Parent as Border;
                parentBorder.BorderBrush = (SolidColorBrush)this.FindResource("MaxDarkPink");
                parentBorder.BorderThickness = new Thickness(1);

                _chosenProjectVersion = nowChosenProjectVersion;
            }
            else if (_chosenProjectVersion != null && nowChosenProjectVersion != _chosenProjectVersion)
            {
                Border oldParentBorder = _chosenProjectVersion.Parent as Border;
                oldParentBorder.ClearValue(Border.BorderBrushProperty);
                oldParentBorder.ClearValue(Border.BorderThicknessProperty);

                Border nowParentBorder = nowChosenProjectVersion.Parent as Border;
                nowParentBorder.BorderBrush = (SolidColorBrush)this.FindResource("MaxDarkPink");
                nowParentBorder.BorderThickness = new Thickness(1);

                _chosenProjectVersion = nowChosenProjectVersion;
            }
            else
            {
                Border parentBorder = _chosenProjectVersion.Parent as Border;
                parentBorder.ClearValue(Border.BorderBrushProperty);
                parentBorder.ClearValue(Border.BorderThicknessProperty);

                _chosenProjectVersion = null;
            }

            if (_chosenProjectSomeListTask != null)
            {
                Border border = _chosenProjectSomeListTask.Parent as Border;
                border.ClearValue(Border.BorderBrushProperty);
                border.ClearValue(Border.BorderThicknessProperty);

                _chosenProjectSomeListTask = null;
            }
        }

        private void ProjectSomeListTask_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SomeTask_MouseLeftButtonDown(sender, ref _chosenProjectSomeListTask);
        }

        private bool CreateVersion(string textVersion, string textDescription, ItemsControl itemsControl, TextBlock textBlockNoResultFoundVersions)
        {
            if (textVersion.Replace(" ", "") == "" || textDescription.Replace(" ", "") == "")
            {
                MessageBox.Show("One of the fields is incorrect");
                return false;
            }

            Border border = new Border()
            {
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#121212"),
                Style = (Style)this.FindResource("PinkGradientBorderStyle"),
                Margin = new Thickness(5, 4, 5, 0),
                CornerRadius = new CornerRadius(6)
            };
            border.MouseLeftButtonDown += ProjectVersionsListVersion_MouseLeftButtonDown;

            StackPanel stackPanel = new StackPanel();

            TextBlock version = new TextBlock()
            {
                FontFamily = new FontFamily("Verdana"),
                FontWeight = FontWeights.DemiBold,
                Style = (Style)this.FindResource("PinkGradientTextStyle"),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 8, 5, 0),
                Text = textVersion
            };

            TextBlock versionDescription = new TextBlock()
            {
                FontFamily = new FontFamily("Verdana"),
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 5, 5, 8),
                Text = textDescription
            };

            stackPanel.Children.Add(version);
            stackPanel.Children.Add(versionDescription);

            border.Child = stackPanel;
            itemsControl.Items.Add(border);

            CloseBorderOverWindow(borderProjectVersionsListAddVersion);

            textBoxProjectVersionsListVersionAddTextVersion.Text = string.Empty;
            textBoxProjectVersionsListVersionAddTextDescription.Text = string.Empty;

            if (itemsControl.HasItems) textBlockNoResultFoundVersions.Visibility = Visibility.Collapsed;

            return true;
        }
        private bool CreateSomeTask(string text, ItemsControl itemsControl, TextBlock textBlockNoResultFoundSomething)
        {
            if (text.Replace(" ", "") == "") return false;

            Border border = new Border()
            {
                Style = (Style)this.FindResource("PinkGradientBorderStyle"),
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#121212"),
                Margin = new Thickness(5, 3, 5, 0),
                CornerRadius = new CornerRadius(6)
            };
            border.MouseLeftButtonDown += ProjectSomeListTask_MouseLeftButtonDown;
            TextBlock textBlock = new TextBlock()

            {
                FontFamily = new FontFamily("Verdana"),
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                Text = text
            };

            border.Child = textBlock;
            itemsControl.Items.Add(border);

            CloseBorderOverWindow(borderProjectSomeListAddSomething);

            textBoxProjectTextAddSomething.Text = string.Empty;

            if (itemsControl.HasItems) textBlockNoResultFoundSomething.Visibility = Visibility.Collapsed;

            return true;
        }

        private void SomeTask_MouseLeftButtonDown(object sender, ref TextBlock chosenSomeTask)
        {
            TextBlock nowChosenSomeTask = null;

            if (sender is TextBlock) nowChosenSomeTask = sender as TextBlock;
            else nowChosenSomeTask = (sender as Border).Child as TextBlock;

            if (chosenSomeTask == null)
            {
                Border parentBorder = nowChosenSomeTask.Parent as Border;
                parentBorder.BorderBrush = (SolidColorBrush)this.FindResource("MaxDarkPink");
                parentBorder.BorderThickness = new Thickness(1);

                chosenSomeTask = nowChosenSomeTask;
            }
            else if (chosenSomeTask != null && nowChosenSomeTask != chosenSomeTask)
            {
                Border oldParentBorder = chosenSomeTask.Parent as Border;
                oldParentBorder.ClearValue(Border.BorderBrushProperty);
                oldParentBorder.ClearValue(Border.BorderThicknessProperty);

                Border nowParentBorder = nowChosenSomeTask.Parent as Border;
                nowParentBorder.BorderBrush = (SolidColorBrush)this.FindResource("MaxDarkPink");
                nowParentBorder.BorderThickness = new Thickness(1);

                chosenSomeTask = nowChosenSomeTask;
            }
            else
            {
                Border parentBorder = chosenSomeTask.Parent as Border;
                parentBorder.ClearValue(Border.BorderBrushProperty);
                parentBorder.ClearValue(Border.BorderThicknessProperty);

                chosenSomeTask = null;
            }

            if (_chosenProjectVersion != null)
            {
                Border parentBorder = _chosenProjectVersion.Parent as Border;
                parentBorder.ClearValue(Border.BorderBrushProperty);
                parentBorder.ClearValue(Border.BorderThicknessProperty);

                _chosenProjectVersion = null;
            }
        }

        // Block end
        private void BackToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            GridOpenProjectWindow.Visibility = Visibility.Collapsed;
            stackPanelMainWindow.Visibility = Visibility.Visible;

            textBlockNoTasksFoundProjectToDo.Visibility = Visibility.Visible;
            itemsControlProjectToDoTasks.Items.Clear();
            textBlockNoTasksFoundProjectToFix.Visibility = Visibility.Visible;
            itemsControlProjectToFixTasks.Items.Clear();
            textBlockNoTasksFoundProjectMade.Visibility = Visibility.Visible;
            itemsControlProjectMadeTasks.Items.Clear();
            textBlockNoVersionsFoundProject.Visibility = Visibility.Visible;
            itemsControlProjectVersionsList.Items.Clear();

            _chosenProjectSomeList = null;
            _chosenProjectSomeListTask = null;
            _chosenProjectVersion = null;

            textBlockCurrentProject.Text = string.Empty;
        }
    }
}
