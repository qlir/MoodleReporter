using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UIReporter.Helpers
{
    static public class Helpers
    {
        public static bool DataGridIsEditing(DataGrid dg)
        {
            for (int i = 0; i < dg.Items.Count; i++)
            {
                var row = dg.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                if (row != null && row.IsEditing)
                    return true;
            }
            return false;
        }
        public static void DataGridCommitIfEditing(DataGrid dg)
        {
            if (DataGridIsEditing(dg)) dg.CommitEdit();
        }

        public static bool DataGridHasErrors(DataGrid dg)
        {
            var errors = (from c in
                              (from object i in dg.ItemsSource
                               select dg.ItemContainerGenerator.ContainerFromItem(i))
                          where c != null
                          select Validation.GetHasError(c))
             .FirstOrDefault(x => x);
            return errors;
        }

        public static T GetParrentWithType<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject result = null;
            while (child != null && !((result = VisualTreeHelper.GetParent(child)) is T))
            {
                child = result;
            }
            return (T)result;
        }

        public static void RefreshDataGridIfNotEditing(DataGrid dg)
        {
            if (!DataGridIsEditing(dg))
            {
                dg.Items.Refresh();
                dg.CanUserAddRows = false;
                dg.CanUserAddRows = true;
            }
        }
    }
}
