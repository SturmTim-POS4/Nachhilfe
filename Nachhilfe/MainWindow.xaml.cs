using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Nachhilfe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        List<Student> students = new();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadStudents();
            
            GenerateButtons();

        }

        private void GenerateButtons()
        {
            foreach (int level in Configuration.Levels)
            {
                RadioButton levelButton = new RadioButton()
                {
                    Content = level,
                    GroupName = "Level"
                };
                if (level == 1)
                {
                    levelButton.IsChecked = true;
                }

                SchoolLevels.Children.Add(levelButton);
            }

            bool isFirstSubject = true;

            foreach (String subject in Configuration.Subject)
            {
                RadioButton subjectButton = new RadioButton()
                {
                    Content = subject,
                    GroupName = "Subjects"
                };
                if (isFirstSubject)
                {
                    subjectButton.IsChecked = true;
                    isFirstSubject = false;
                }

                Subjects.Children.Add(subjectButton);
            }

            List<String> classes = new();
            foreach (Student student in students)
            {
                classes.Add(student.Clazz);
            }

            classes = classes.Distinct().ToList();

            foreach (String clazz in classes)
            {
                CheckBox checkBox = new CheckBox()
                {
                    Content = clazz,
                    Name = $"Class{clazz}"
                };
                checkBox.Click += CheckBoxChanged;
                ToolBar.Items.Add(checkBox);
            }

        }

        private void LoadStudents() 
            => students.AddRange(File.ReadAllText(@"csv_Images/csv/students.csv").Split('\n').Where(item => !item.Equals("")).Select(item => Student.Parse(item.Split('\r')[0])));

        private void ShowOfClass(string classString)
        {
            foreach (Student student in students)
            {
                if (student.Clazz.ToUpper().Equals(classString.ToUpper()))
                {
                    StudentBox.Items.Add(student);
                }
            }
        }
        
        private void RefreshStudent()
        {
            if (students.Contains(StudentBox.SelectedItem as Student))
            {
                Student selectedStudent = students[students.IndexOf(StudentBox.SelectedItem as Student)];
                StudentName.Content = selectedStudent;
                Tutorings.Items.Clear();
                foreach (Service service in selectedStudent.Services)
                {
                    Tutorings.Items.Add(service);
                }

                Tutorings.Items.Refresh();

                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(selectedStudent.ImagePath, UriKind.Relative);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                StudentImage.Source = image;

                TutoringCount.Content = $"{selectedStudent.Services.Count} Nachhilfen:";
            }
        }
        
        private void LoadServices(object sender, RoutedEventArgs e)
        {
            List<string> tutoring = new();
            tutoring.AddRange(File.ReadAllText(@"csv_Images/csv/tutorings.csv").Split('\n').Where(item => !item.Equals("")));
            List<Student> tempStudents = new();
            tempStudents.AddRange(students);
            foreach (String tutoringString in tutoring)
            {
                String[] splitTutoring = tutoringString.Split(';');
                foreach (Student student in tempStudents)
                {
                    if (student.Name.Equals(splitTutoring[0]))
                    {
                        student.Services.Add(new Service(){Level = Int32.Parse(splitTutoring[2].Split('\r')[0]), Subject = splitTutoring[1]});
                        students[students.IndexOf(student)] = student;
                        break;
                    }
                }
            }
            
            RefreshStudent();
        }

        private void SaveServices(object sender, RoutedEventArgs e)
        {
            StringBuilder tutoringString = new();
            foreach (Student student in students)
            {
                if (student.Services.Count > 0)
                {
                    tutoringString.AppendLine(student.ToCsvString());
                }
            }
            File.WriteAllText(@"csv_Images/csv/tutorings.csv",tutoringString.ToString());
        }
        
        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            StudentBox.Items.Clear();
            foreach (var toolbarObject in ToolBar.Items)
            {
                if (toolbarObject.GetType() == new CheckBox().GetType())
                {
                    CheckBox checkBox = (CheckBox) toolbarObject;
                    if (checkBox.IsChecked is true)
                    {
                        ShowOfClass(checkBox.Content.ToString());
                    }
                }
            }

        }

        private void StudentBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshStudent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            int level = 0;
            String subject = "";

            foreach (RadioButton radioButton in SchoolLevels.Children)
            {
                if (radioButton.IsChecked is true)
                {
                    level = Int32.Parse(radioButton.Content.ToString());
                    break;
                }
            }
            foreach (RadioButton radioButton in Subjects.Children)
            {
                if (radioButton.IsChecked is true)
                {
                    subject = radioButton.Content.ToString();
                    break;
                }
            }

            Service service = new Service()
            {
                Level = level,
                Subject = subject
            };
            if (students.Contains(StudentBox.SelectedItem as Student))
            {
                students[students.IndexOf(StudentBox.SelectedItem as Student)].Services.Add(service);
            }
            
            RefreshStudent();
        }
    }
}