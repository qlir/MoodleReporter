using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using ReportsGenerator.DataStructures;
using H = UIReporter.Helpers.Helpers;

namespace UIReporter.Validations
{

    public class CourseIdValidationRule : ValidationRule
    {
        public static ValidationResult Check(object value)
        {

            if (value is int)
            {
                return null;
            }
            else if (value is string && !string.IsNullOrEmpty(((string)value).Trim()))
            {
                int temp;
                if (!int.TryParse((string)value, out temp))
                    return new ValidationResult(false, "'ID' должен быть числом.");
            }
            else
            {
                return new ValidationResult(false, "Поле 'ID' должно быть заполненно.");
            }
            return null;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Check(value) ?? ValidationResult.ValidResult;
        }
    }
    public class NonEmptyStringValidationRule : ValidationRule
    {
        public string ErrorText { get; set; }

        public static ValidationResult Check(object value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value as string))
                return new ValidationResult(false, errorMessage);

            return null;
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Check(value, ErrorText) ?? ValidationResult.ValidResult;
        }
    }

    public class EmailValidationRule : ValidationRule
    {
        public static ValidationResult Check(object value)
        {
            if (!(value is string) || !Regex.IsMatch((string)value,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase))
            {
                return new ValidationResult(false, "Некоректный E-mail.");
            }

            return null;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Check(value) ?? ValidationResult.ValidResult;
        }

    }

    public class ReportInfoRowValidateRules : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string)
            {
                if (string.IsNullOrEmpty(value as string)) return new ValidationResult(false, "Поле не может быть пустым.");
                return ValidationResult.ValidResult;
            }
            if (value is BindingGroup)
            {
                var dataGridRow = ((BindingGroup)value).Owner as DataGridRow;
                if (dataGridRow != null)
                {
                    var repInfo = (ReportInfo)(dataGridRow.Item);
                    if (repInfo.CourseID < 0) return new ValidationResult(false, "Курс не выбран.");
                    if (repInfo.GroupID < 0) return new ValidationResult(false, "Группа не выбранна.");
                    /* if (string.IsNullOrEmpty(repInfo.CuratorEmail))
                         return new ValidationResult(false, "Куратор не выбранн.");
                     if (string.IsNullOrEmpty(repInfo.Institution))
                         return new ValidationResult(false, "Учереждение не задано.");*/
                }
            }
            return ValidationResult.ValidResult;
        }
    }

    public class CourseRowValidate : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is BindingGroup)
            {
                var dataGridRow = ((BindingGroup)value).Owner as DataGridRow;
                if (dataGridRow != null)
                {
                    var course = (Course)dataGridRow.Item;
                    return CourseIdValidationRule.Check(course.Id) ??
                          // NonEmptyStringValidationRule.Check(course.ShortName, "Куратор с таким ID не найден на серввере.") ??
                           ValidationResult.ValidResult;
                }
                return ValidationResult.ValidResult;
            }
            return ValidationResult.ValidResult;
        }
    }
    public class CuratorRowValidate : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is BindingGroup)
            {
                var dataGridRow = ((BindingGroup)value).Owner as DataGridRow;
                if (dataGridRow != null)
                {
                    var curator = (Curator)dataGridRow.Item;
                    return EmailValidationRule.Check(curator.Email) ??
                           NonEmptyStringValidationRule.Check(curator.FirstName, "Поле 'Имя Отчество' должно быть заполненно.") ??
                           NonEmptyStringValidationRule.Check(curator.Institution, "Поле 'Организация' должно быть заполненно.") ??
                           ValidationResult.ValidResult;
                }
                return ValidationResult.ValidResult;
            }
            return ValidationResult.ValidResult;
        }
    }

    public class UniqeCuratorsEmailValidate : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var bindingExpression = value as BindingExpression;
            if (bindingExpression == null) return ValidationResult.ValidResult;
            var dg = H.GetParrentWithType<DataGrid>(bindingExpression.BindingGroup == null ? bindingExpression.Target : bindingExpression.BindingGroup.Owner);
            if (dg == null) return ValidationResult.ValidResult;

            var items = (IList<Curator>)dg.ItemsSource;
            var editingItem = ((Curator) bindingExpression.DataItem);
            return items.Any(i => i.Email == editingItem.Email && editingItem != i) ?
                new ValidationResult(false, "Куратор с таким имейлом уже сужествует.") :
                ValidationResult.ValidResult;
        }
    }
    public class UniqeCourseIdValidate : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var bindingExpression = value as BindingExpression;
            if (bindingExpression == null) return ValidationResult.ValidResult;
            var dg = H.GetParrentWithType<DataGrid>(bindingExpression.Target);
            if (dg == null) return ValidationResult.ValidResult;

            var items = (IList<Course>)dg.ItemsSource;
            var editingItem = ((Course)bindingExpression.DataItem);
            return items.Any(i => i.Id == editingItem.Id && editingItem != i) ?
                new ValidationResult(false, "Курс с таким ID уже существует.") :
                ValidationResult.ValidResult;
        }
    }
}
