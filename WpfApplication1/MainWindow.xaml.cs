using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ReportsGenerator;
using ReportsGenerator.DataStructures;
using ReportsGenerator.Mail;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ReportsGenerator.UserData;
using UIReporter.AccessoryWindow;
using UIReporter.Annotations;
using ComboBox = System.Windows.Controls.ComboBox;
using Group = ReportsGenerator.DataStructures.Group;
using MessageBox = UIReporter.AccessoryWindow.MessageBox;
using H = UIReporter.Helpers.Helpers;


namespace UIReporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static readonly string AllCuratorString = "Все";
        public static readonly string MoodleCuratorString = "Из Moodle";

        private Reporter _reporter;
        public Reporter Reporter { get { return _reporter; } }

        public ObservableCollection<ReportInfo> ReportsInfos { get { return _reportsInfos; } }
        private readonly ObservableCollection<ReportInfo> _reportsInfos;
        public ObservableCollection<Curator> Curators { get { return _curators; } }
        private readonly ObservableCollection<Curator> _curators;

        public IEnumerable<Object> CbCuratorsIntems
        {
            get
            {
                return new Object[] { AllCuratorString, MoodleCuratorString }.Concat(Curators);
            }
        }

        public ObservableCollection<Course> Courses { get { return _cources; } }
        private readonly ObservableCollection<Course> _cources;



        private bool _reportInfoIsActual;

        public bool HasGeneratedMessages
        {
            get
            {
                return _reporter != null && _reporter.MessagesPreview.Count > 0;
            }
        }

        public IEnumerable<Group> CurrentGroups { get; set; }

        public MainWindow()
        {
            _reportsInfos = new ObservableCollection<ReportInfo>();
            _cources = new ObservableCollection<Course>();
            _curators = new ObservableCollection<Curator>();
            InitializeComponent();
            _reportsInfos.CollectionChanged += (sender, args) => { ReportInfoIsActual = false; };
            InitializeData();
        }

        private void ReporterSeting()
        {
            _reporter = new Reporter
            {
                MailSettings = new MailSettings
                {
                    Email = Settings.Mail.Default.Email,
                    EnableSsl = Settings.Mail.Default.EnableSsl,
                    Password = Settings.Mail.Default.Password,
                    Port = Settings.Mail.Default.Port,
                    SmtpServer = Settings.Mail.Default.SmtpServer,
                    Timeout = Settings.Mail.Default.Timeout
                },
                Token = Settings.Moodle.Default.Token,
                MoodleServer = Settings.Moodle.Default.Server,
                ReportInfo = ReportsInfos,
                Curators = Curators
            };
        }

        private async void InitializeData()
        {
            /*ProgressDialog.ShowDialog(null, "Запуск...", async (ProgressDialog pd) =>
            {*/
                try
                {
                    var coursesTask = loadCources();
                    var reportIndoTask = UserDataCtrl.LoadReportInfo();
                    var curatorsTask = UserDataCtrl.LoadCurators();
                    Courses.Clear();
                    ReportsInfos.Clear();
                    Curators.Clear();
                    ReporterSeting();
                    AddRange(Courses, await coursesTask);
                    AddRange(ReportsInfos, await reportIndoTask);
                    AddRange(Curators, await curatorsTask);
                    ReportInfoIsActual = true;
                }
                catch (Exception e)
                {
                    ErrorWindow.ShowError(e);
                }

           /* });*/
        }

        public static void AddRange<T>(IList<T> observableCollection, IEnumerable<T> enumerable)
        {
            foreach (var element in enumerable)
            {
                observableCollection.Add(element);
            }
        }

        private async Task<IEnumerable<Course>> loadCources()
        {
            return await UserDataCtrl.LoadCourses();
        }

        private async Task<IEnumerable<Group>> GetGroups(int courseid)
        {
            return await _reporter.GetGroups(courseid);
        }

        private async void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var cb = (ComboBox)sender;
            var reportInfo = (ReportInfo)cb.DataContext;
            if (reportInfo.CourseID >= 0)
                try
                {
                    cb.ItemsSource = (await GetGroups(reportInfo.CourseID)).Where(g =>
                        Regex.IsMatch(g.Name, "^\\d*_\\d*_.*$")
                    );
                }
                catch (Exception ex)
                {
                    ErrorWindow.ShowError(ex);
                }
            Mouse.OverrideCursor = null;
        }

        private void GroupsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = (ComboBox)sender;
            ReportInfo reportInfo = ((ReportInfo)cb.DataContext);
            if (e.AddedItems.Count == 0) return;
            Group newGroup = (Group)e.AddedItems[0];
            reportInfo.GroupID = newGroup.Id;
            reportInfo.GroupName = newGroup.Name;
        }

        private void ReportInfoDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if ((string)e.Column.Header == "Группа")
            {
                ReportInfoDataGrid.CommitEdit();
            }
        }

        public bool ReportInfoIsActual
        {
            get { return _reportInfoIsActual; }
            set
            {
                _reportInfoIsActual = value;
                OnPropertyChanged();
            }
        }

        private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private bool Save()
        {
            try
            {
                if (H.DataGridHasErrors(ReportInfoDataGrid))
                {
                    MessageBox.Show(this, "Для сохранения нужно разрешить все конфликты.");
                    return false;
                }
                SaveReportInfo();
                return true;
            }
            catch (Exception e)
            {
                ErrorWindow.ShowError(new ReporterException("При сохранении данных возникла ошибка: \"" + e.Message + "\""));
                return false;
            }
            finally
            {
                H.RefreshDataGridIfNotEditing(ReportInfoDataGrid);
            }
        }

        private async void SaveReportInfo()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            await UserDataCtrl.SaveReportInfo(ReportsInfos);
            ReportInfoIsActual = true;
            Mouse.OverrideCursor = null;
        }

        private void DiscardBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DiscardReportInfo();
        }

        private async void DiscardReportInfo()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var reportIndoTask = UserDataCtrl.LoadReportInfo();
                ReportsInfos.Clear();
                AddRange(ReportsInfos, await reportIndoTask);
                ReloadDataGridItems();
                ReportInfoIsActual = true;
            }
            catch (Exception e)
            {
                ErrorWindow.ShowError(new ReporterException("При сбросе данных возникла ошибка: \"" + e.Message + "\""));
            }
            finally
            {
                H.RefreshDataGridIfNotEditing(ReportInfoDataGrid);
                Mouse.OverrideCursor = null;
            }
        }

        private void BtnGnerate_OnClick(object sender, RoutedEventArgs e)
        {
            bool isSuccess = false;
            ProgressDialog.ShowDialog(this, "Генерация отчетов...", async (ProgressDialog pd) =>
            {
                Reporter.GenerationProgress progressHandler = (progress) =>
                {
                    pd.Value = progress;
                };
                _reporter.GenerationProgressEvent += progressHandler;
                try
                {
                    _reporter.Template = await UserDataCtrl.LoadTemplate();
                    await GenerateReportAsync();
                    isSuccess = true;
                }
                catch (FileNotFoundException ex)
                {
                    ErrorWindow.ShowError(new ReporterException("Шаблон для писем не найден."));
                }
                catch (Exception ex)
                {
                    ErrorWindow.ShowError(ex);
                }
                _reporter.GenerationProgressEvent -= progressHandler;
            });
            if (isSuccess) MessageBox.Show(this, String.Format("Генерация успешно завершена.\nСгенерированные письма можно найти в каталоге:\n{0}", _reporter.MailsPath), "Генерация");
        }

        public async Task GenerateReportAsync()
        {
            await _reporter.GenerateReportMessages();
            OnPropertyChanged("HasGeneratedMessages");

        }

        private async void BtnSend_OnClick(object sender, RoutedEventArgs e)
        {
            Dictionary<MailMessage, string> details = null;
            ProgressDialog.ShowDialog(this, "Отправка отчетов...", async (pd) =>
            {
                Reporter.SendingProgress progressHandler = (progress) =>
                {
                    pd.Value = progress;
                };
                try
                {
                    _reporter.SendingProgressEvent += progressHandler;
                    details = await SendReports();
                }
                catch (Exception ex)
                {
                    ErrorWindow.ShowError(ex);
                }
                _reporter.SendingProgressEvent -= progressHandler;
            });
            if (details != null) new SendReportWindow(details).ShowDialog();
        }

        private async Task<Dictionary<MailMessage, string>> SendReports()
        {
            return await _reporter.SendReports();
        }

        private void BtnPreview_OnClick(object sender, RoutedEventArgs e)
        {
            var messagesPreview = _reporter.MessagesPreview.Select(m => m).ToList();
            if (messagesPreview.Count == 0)
            {
                ErrorWindow.ShowError(new ReporterException("Нет данных для просмотра."));
                return;
            }
            new Previewer(messagesPreview).Show();
        }

        private void CuratorsCtrl_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyEditing();
                new CuratorsWindow(this).ShowDialog();
                ReloadDataGridItems();
            }
            catch (Exception ex)
            {
                ErrorWindow.ShowError(ex);
            }
        }

        private void CoursesCtrl_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyEditing();
                new CoursesWindow(this).ShowDialog();
                ReloadDataGridItems();
            }
            catch (Exception ex)
            {
                ErrorWindow.ShowError(ex);
            }
        }

        public void ReloadDataGridItems()
        {
            H.RefreshDataGridIfNotEditing(ReportInfoDataGrid);
        }
        public void ApplyEditing()
        {
            ReportInfoDataGrid.CommitEdit();
            ReportInfoDataGrid.CommitEdit();
        }

        private void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
            ReporterSeting();
        }

        private void ReportInfoDataGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction == DataGridEditAction.Cancel || !(e.Row.Item is ReportInfo)) return;
                ReportInfoIsActual = false;
                ReportInfo ri = (ReportInfo)e.Row.Item;
                if (e.Column.Header.ToString() == "Курс")
                {
                    var selectedItem = (Course)((ComboBox)e.EditingElement).SelectedItem;
                    int selectedId = selectedItem == null ? -1 : selectedItem.Id;
                    if (selectedId != ri.CourseID)
                    {
                        ri.CourseID = selectedId;
                        ri.GroupID = -1;
                        ri.GroupName = null;
                        ri.OnPropertyChanged("GroupTitle");
                    }
                    //GetGroups(ri.CourseID);
                }
                if (e.Column.Header.ToString() != "Группа")
                    TrySelectGroupByDate(ri);
            }
            catch (Exception ex)
            {
                ErrorWindow.ShowError(ex);
            }
        }

        public async void TrySelectGroupByDate(ReportInfo ri)
        {
            try
            {
                if (ri == null || ri.CourseID == -1 || ri.GroupID != -1) return;
                string pattern = String.Format("{0}_0?{1}_.*|{2}_0?{3}_.*", ri.StartDate.Year, ri.StartDate.Month,
                    ri.EndDate.Year, ri.EndDate.Month);

                var grTask = GetGroups(ri.CourseID);
                Group g = (await grTask).FirstOrDefault(gr => Regex.IsMatch(gr.Name, pattern));
                if (g == null) return;
                ri.GroupName = g.Name;
                ri.GroupID = g.Id;
            }
            catch (Exception ex)
            {
                ErrorWindow.ShowError(ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ReportInfoDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            if (!ReportInfoIsActual)
            {
                MessageBoxResult result = MessageBox.Show(this, "Данные не были сохранены. Сохранить?", "Подтверждение",
                    MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        if (Save()) break;
                        e.Cancel = true;
                        return;
                    case MessageBoxResult.No:
                        DiscardReportInfo();
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
        }

        private void CuratorCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var context = ((ComboBox)sender).DataContext as ReportInfo;
            if (e.AddedItems.Count < 1 || context == null) return;

            if (e.AddedItems[0] is Curator)
            {
                context.CuratorsGenerationType = ReportInfo.CuratorsGenerationTypeEnum.Custom;
                context.CuratorsEmail = ((Curator)e.AddedItems[0]).Email;
            }
            else if (MoodleCuratorString.Equals(e.AddedItems[0]))
            {
                context.CuratorsGenerationType = ReportInfo.CuratorsGenerationTypeEnum.MoodleCurators;
                context.CuratorsEmail = null;
            }
            else if (AllCuratorString.Equals(e.AddedItems[0]))
            {
                context.CuratorsGenerationType = ReportInfo.CuratorsGenerationTypeEnum.All;
                context.CuratorsEmail = null;
            }
        }

        private void ReportInfoDataGrid_OnAddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            if (ReportsInfos.Count == 0) return;

            ReportInfo lastInfo = ReportsInfos[ReportsInfos.Count - 1];
            e.NewItem = new ReportInfo()
            {
                StartDate = lastInfo.StartDate,
                EndDate = lastInfo.EndDate
            };

        }

        private void Cell_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DataGridCell)
            {
                //DataGridCell dgc = (DataGridCell)e.OriginalSource;

                ReportInfoDataGrid.BeginEdit(e);
            }
        }

        private void DeletItem_onClick(object sender, RoutedEventArgs e)
        {
            DeleteSelectedItems();
        }
        public void DeleteSelectedItems()
        {
            var curators = (from c in (IEnumerable<Object>)ReportInfoDataGrid.SelectedItems
                            where c is ReportInfo
                            select (ReportInfo)c).ToArray();
            /*var result = MessageBox.Show(curators.Length > 1 ? "Удалить запись?" : "Удалить куратора?", "Удаление",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)*/
            {
                foreach (var c in curators)
                {
                    ReportsInfos.Remove(c);
                }
                //Save();
                H.RefreshDataGridIfNotEditing(ReportInfoDataGrid);
            }
        }
    }
}
