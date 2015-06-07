using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UIReporter.SettingsViews;

namespace UIReporter
{
    /// <summary>
    /// Interaction logic for FullSettings.xaml
    /// </summary>
    public partial class FullSettings : Window
    {
        private readonly Dictionary<string, Grid> _settingItems;

        public Dictionary<string, Grid> SettingItems
        {
            get { return _settingItems; }
        }

        public Grid CurrentSettingControl { get; set; }

        public FullSettings()
        {
            _settingItems = new Dictionary<string, Grid>();
            _settingItems.Add("Основные", new BaseSettingsGrid());
            _settingItems.Add("Генерация", new GenerationSettingGrid());
            _settingItems.Add("Системные", new SystemDataSettingGrid());
            InitializeComponent();
            ListBox.SelectedIndex = 0;
            ListBox.Focus();
        }
    }
}
