using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ReportsGenerator.Settings;
using MessageBox = UIReporter.AccessoryWindow.MessageBox;

namespace UIReporter.SettingsViews
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class GenerationSettingGrid : Grid
    {

        public GenerationSettingGrid()
        {
            InitializeComponent();
            AddField("Email CC", "CC");
            AddField("Тема письма", "MailSubject");
            AddField("Заголовок таблицы", "TableTitle");
            AddField("Обращение", "Welcome");
            AddField("Окончание обращения для муж.({0})", "WelcomeMalePostfix");
            AddField("Окончание обращения для жен.({0})", "WelcomeFemalePostfix");
            AddField("Формат чисел", "NumberFormat");
            AddField("Заголовок колонки с организацией", "InstitutionColumnTitle");
            AddField("Заголовок колонки ФИО", "FioColumnTite");
            AddField("Заголовок строки проходного балла", "PassedGradeRowHeader");
            AddField("Заголовок строки среднеего по РИЦ", "AVGbyInstitutionRowHeader");
            AddField("Заголовок строки среднеего по РИЦам", "AVGbyInstitutionsRowHeader");
            AddField("Заголовок строки процента успевающих", "ProgressRowHeader");
            AddField("Стиль таблицы", "TableStyle");
            AddField("Стиль заголовка таблицы", "CaptionStyle");
            AddField("Стиль заголовков столбцов таблицы", "HeadersColumnsStyle");
            AddField("Стиль строки с проходным баллом", "PassedGradeRowStyle");
            AddField("Стиль строки со средним по РИЦ", "AVGbyInstitutionsRowStyle");
            AddField("Стиль строки co cредним по РИЦам", "AVGbyInstitutionRowStyle");
            AddField("Стиль строки с процентом успеваемости ", "ProgressRowStyle");
            AddField("Стиль строки с оценками", "GradesRowsStyle");
            AddField("Стиль ячеек", "CellStyle");
            AddField("Стиль колонок пройденных недель", "PassedColumnStyle");
            AddField("Тег для вставки таблиц", "TagToTablesPaste");
            AddField("Тег для вставки обращения", "TagToWelcomePaste");
            AddField("Шаблон для проверки направления курса", "PatternToCheckDirection");
        }

        private void AddField(string title, string fieldName)
        {
            Binding myBinding = new Binding(fieldName);
            myBinding.Source = GenerationSetting.Default;
            var f = new SettingsField();
            f.Label.Content = title;
            BindingOperations.SetBinding(f.TextBox, TextBox.TextProperty, myBinding);
            FieldsContainer.Children.Add(f);
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            GenerationSetting.Default.Save();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (Char.IsNumber(c))
                e.Handled = false;
            else
                e.Handled = true;

            base.OnPreviewTextInput(e);
        }
    }
}
