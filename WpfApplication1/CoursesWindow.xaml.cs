using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReportsGenerator.Annotations;
using ReportsGenerator.DataStructures;
using UIReporter.AccessoryWindow;
using UIReporter.UserData;
using MessageBox = UIReporter.AccessoryWindow.MessageBox;
using H = UIReporter.Helpers.Helpers;

namespace UIReporter
{
    /// <summary>
    /// Interaction logic for CoursesWindow.xaml
    /// </summary>
    public partial class CoursesWindow : Window, INotifyPropertyChanged
    {
        public int NewId { get; set; }
        public ObservableCollection<Course> Courses
        {
            get { return _context.Courses; }
        }

        private MainWindow _context;
        public CoursesWindow(MainWindow context)
        {
            _context = context;
            Courses.CollectionChanged += (a, e) => { IsActualData = false; };
            InitializeComponent();
            IsActualData = true;
        }

        private async void Save()
        {
            try
            {
                await UserDataCtrl.SaveCourses(Courses);
                IsActualData = true;
            }
            catch (Exception e)
            {
                ErrorWindow.ShowError(e);
            }
        }

        private async void Discard()
        {
            try
            {
                var oldCourses = await UserDataCtrl.LoadCourses();
                Courses.Clear();
                MainWindow.AddRange(Courses, oldCourses);
                IsActualData = true;
                Close();
            }
            catch (Exception e)
            {
                ErrorWindow.ShowError(e);
            }
        }

        private bool _isActualData;
        public bool IsActualData
        {
            get { return _isActualData; }
            set
            {
                _isActualData = value;
                OnPropertyChanged();
            }
        }

        private void CoursesWindow_OnClosing(object sender, CancelEventArgs e)
        {
            try
            {
                CoursesDG.CommitEdit();
                CoursesDG.SelectedItem = null;
                if (!IsActualData)
                {
                    e.Cancel = true;
                    MessageBoxResult result = MessageBox.Show(this, "Данные не были сохранены. Сохранить?", "Подтверждение", MessageBoxButton.YesNoCancel);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            SaveButton_OnClick(this, null);
                            break;
                        case MessageBoxResult.No:
                            Discard();
                            IsEnabled = false;
                            break;
                    }
                }
                else CoursesDG.CanUserAddRows = false;
            }
            catch (Exception ex)
            {
                ErrorWindow.ShowError(ex);
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (H.DataGridHasErrors(CoursesDG))
            {
                MessageBox.Show(this, "Для сохранения нужно разрешить все конфликты.");
                return;
            }
            IsEnabled = false;
            Save();
        }
        private bool ReportsInfosContainsCourse(IEnumerable<ReportInfo> reportsInfos, Course course)
        {
            return reportsInfos.Any(ri => ri.CourseID == course.Id);
        }

        public void DeleteSelectedItems()
        {
            try
            {
                CoursesDG.CommitEdit();
                CoursesDG.CommitEdit();
                CoursesDG.CancelEdit();
                CoursesDG.CancelEdit();
                var courses = ((from c in (IEnumerable<Object>)CoursesDG.SelectedItems
                                where c is Course
                                select (Course)c).ToArray()).ToArray();
                string courseName = null;
                bool can = !_context.ReportsInfos.Any(ri => courses.Any(c =>
                {
                    if (c.Id == ri.CourseID)
                    {
                        courseName = c.ShortName;
                        return true;
                    }
                    return false;
                }));
                if (can)
                {
                    var result = MessageBox.Show(this, courses.Length > 1 ? "Удалить курсы?" : "Удалить курс?", "Удаление",
                        MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        foreach (var c in courses)
                        {
                            Courses.Remove(c);
                        }
                        Save();
                    }
                }
                else
                    MessageBox.Show(this, String.Format("Невозможно удалить курс '{0}' так как он используется.", courseName), "Удаление");
            }
            catch (Exception ex)
            {
                ErrorWindow.ShowError(ex);
            }
        }

        private void CoursesDG_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void CoursesDG_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel) return;
            IsActualData = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        private void Cell_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DataGridCell)
            {
                CoursesDG.BeginEdit(e);
            }
        }

        private void CoursesDG_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel) return;
            Save();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Courses.Any(c => c.Id == NewId))
                {
                    MessageBox.Show(this, "Курс с таким ID уже добавлен.");
                    return;
                }
                Mouse.OverrideCursor = Cursors.Wait;
                IsEnabled = false;
                Course cFromMoodle = await _context.Reporter.GetCourseById(NewId);
                if (cFromMoodle != null)
                {
                    Courses.Add(cFromMoodle);
                    Save();
                }
                else
                {
                    MessageBox.Show(this, "Курс с таким ид не найден на сервере.", "Ошибка добавления");
                }
            }
            catch (Exception ex)
            {
                ErrorWindow.ShowError(ex);
            }
            finally
            {
                Mouse.OverrideCursor = null;
                IsEnabled = true;
            }
        }

        private void DeletItem_onClick(object sender, RoutedEventArgs e)
        {
            DeleteSelectedItems();
        }
    }
}
