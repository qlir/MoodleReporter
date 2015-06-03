using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ReportsGenerator.DataStructures;
using ReportsGenerator.UserData;
using UIReporter.AccessoryWindow;
using H = UIReporter.Helpers.Helpers;
using MessageBox = UIReporter.AccessoryWindow.MessageBox;

namespace UIReporter
{
    /// <summary>
    /// Interaction logic for CuratorsWindow.xaml
    /// </summary>
    public partial class CuratorsWindow : Window, INotifyPropertyChanged
    {
        private bool _isActualData;
        private MainWindow _context;

        public ObservableCollection<Curator> Curators
        {
            get { return _context.Curators; }
        }

        public CuratorsWindow(MainWindow context)
        {
            _context = context;
            Curators.CollectionChanged += (a, e) => { IsActualData = false; };
            InitializeComponent();
            IsActualData = true;
        }

        private async void SaveAndClose()
        {
            try
            {
                await (Save());
                Close();
            }
            catch (Exception e)
            {
                ErrorWindow.ShowError(e);
            }
        }
        private async Task<bool> Save()
        {
            try
            {
                CuratorsDG.CommitEdit();
                CuratorsDG.CommitEdit();
                if (H.DataGridHasErrors(CuratorsDG))
                {
                    MessageBox.Show(this, "Для сохранения нужно разрешить все конфликты.");
                    return false;
                }
                IsEnabled = false;
                if (IsActualData) return true;

                if (Curators.Any())
                    foreach (var keyValuePair in _changedEmails)
                    {
                        var newValue = Curators.First(c => c.GetHashCode() == keyValuePair.Key).Email;
                        foreach (
                            ReportInfo reportInfo in
                                _context.ReportsInfos.Where(ri => ri.CuratorsEmail == keyValuePair.Value))
                        {
                            reportInfo.CuratorsEmail = newValue;
                        }
                    }
                await UserDataCtrl.SaveCurators(Curators);
                IsActualData = true;
                return true;
            }
            catch (Exception e)
            {
                ErrorWindow.ShowError(e);
                return false;
            }
            finally
            {
                IsEnabled = true;
                CuratorsDG.Focus();
                H.RefreshDataGridIfNotEditing(CuratorsDG);
            }
        }

        private async void Discard()
        {
            try
            {
                var oldCourses = await UserDataCtrl.LoadCurators();
                Curators.Clear();
                MainWindow.AddRange(Curators, oldCourses);
                IsActualData = true;
                Close();
            }
            catch (Exception e)
            {
                ErrorWindow.ShowError(e);
            }
        }

        public bool IsActualData
        {
            get { return _isActualData; }
            set
            {
                _isActualData = value;
                OnPropertyChanged();
            }
        }

        private void CuratorsWindow_OnClosing(object sender, CancelEventArgs e)
        {
            try
            {
                CuratorsDG.CommitEdit(DataGridEditingUnit.Row, true);
                CuratorsDG.CancelEdit(DataGridEditingUnit.Row);
                CuratorsDG.SelectedItem = null;
                if (!IsActualData)
                {
                    e.Cancel = true;
                    MessageBoxResult result = MessageBox.Show(this, "Данные не были сохранены. Сохранить?", "Подтверждение",
                        MessageBoxButton.YesNoCancel);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            SaveAndClose();
                            break;
                        case MessageBoxResult.No:
                            Discard();
                            IsEnabled = false;
                            break;
                    }
                }
                else CuratorsDG.CanUserAddRows = false;
            }
            catch (Exception ex)
            {
                ErrorWindow.ShowError(ex);
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private IDictionary<int, string> _changedEmails = new Dictionary<int, string>();

        private void CuratorsDG_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel) return;
            IsActualData = false;
            if (e.Column.Header != null && e.Column.Header.ToString() == "E-mail" && e.Row.DataContext as Curator != null)
            {
                var oldCuratorsEmail = ((Curator)e.Row.DataContext).Email;
                if (_context.ReportsInfos.Any(c => c.CuratorsEmail == oldCuratorsEmail) && !_changedEmails.ContainsKey(e.Row.DataContext.GetHashCode()))
                {
                    _changedEmails.Add(e.Row.DataContext.GetHashCode(), oldCuratorsEmail);
                }
            }
        }

        public void DeleteSelectedItems()
        {
            var curators = (from c in (IEnumerable<Object>)CuratorsDG.SelectedItems
                            where c is Curator
                            select (Curator)c).ToArray();
            string email = null;
            bool can = !_context.ReportsInfos.Any(ri => curators.Any(c =>
            {
                if (c.Email == ri.CuratorsEmail && !string.IsNullOrEmpty(c.Email) && ri.CuratorsGenerationType == ReportInfo.CuratorsGenerationTypeEnum.Custom)
                {
                    email = c.Email;
                    return true;
                }
                return false;
            }));
            if (can)
            {
                /*var result = MessageBox.Show(curators.Length > 1 ? "Удалить кураторов?" : "Удалить куратора?", "Удаление",
                    MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)*/
                {
                    foreach (var c in curators)
                    {
                        Curators.Remove(c);
                    }
                    //Save();
                    H.RefreshDataGridIfNotEditing(CuratorsDG);
                }
            }
            else
                MessageBox.Show(this, String.Format("Невозможно удалить куратора '{0}' так как он используется.", email), "Удаление");
        }

        private void DeletItem_onClick(object sender, RoutedEventArgs e)
        {
            DeleteSelectedItems();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Fio_LostFocus(object sender, RoutedEventArgs e)
        {
            var cur = ((TextBox)sender).DataContext as Curator;
            if (cur != null) cur.TryParseGender(((TextBox)sender).Text);
        }

        private void Cell_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DataGridCell)
            {
                CuratorsDG.BeginEdit(e);
            }
        }

        private void CuratorsDG_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel) return;
            // Save();
        }

        private void LostFocus_Handler(object sender, RoutedEventArgs routedEventArgs)
        {
            CuratorsDG.CommitEdit(DataGridEditingUnit.Row, true);
        }
    }
}
