﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReportsGenerator.Settings {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    public sealed partial class GenerationSetting : global::System.Configuration.ApplicationSettingsBase {
        
        private static GenerationSetting defaultInstance = ((GenerationSetting)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new GenerationSetting())));
        
        public static GenerationSetting Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string CC {
            get {
                return ((string)(this["CC"]));
            }
            set {
                this["CC"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Результаты {0}-й недели обучения по курсу \"{1}\"")]
        public string TableTitle {
            get {
                return ((string)(this["TableTitle"]));
            }
            set {
                this["TableTitle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ФИО")]
        public string FioColumnTite {
            get {
                return ((string)(this["FioColumnTite"]));
            }
            set {
                this["FioColumnTite"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Учреждение  (Организация)")]
        public string InstitutionColumnTitle {
            get {
                return ((string)(this["InstitutionColumnTitle"]));
            }
            set {
                this["InstitutionColumnTitle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"ИО\"=\"background-color:#ffeeee;\"&\"Продажи\"=\"background-color:#fff888;\"")]
        public string PassedColumnStyle {
            get {
                return ((string)(this["PassedColumnStyle"]));
            }
            set {
                this["PassedColumnStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("color:brown;font-weight:bold;")]
        public string PassedGradeRowStyle {
            get {
                return ((string)(this["PassedGradeRowStyle"]));
            }
            set {
                this["PassedGradeRowStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Проходной балл")]
        public string PassedGradeRowHeader {
            get {
                return ((string)(this["PassedGradeRowHeader"]));
            }
            set {
                this["PassedGradeRowHeader"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.00")]
        public string NumberFormat {
            get {
                return ((string)(this["NumberFormat"]));
            }
            set {
                this["NumberFormat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("font-weight:bold;")]
        public string AVGbyInstitutionRowStyle {
            get {
                return ((string)(this["AVGbyInstitutionRowStyle"]));
            }
            set {
                this["AVGbyInstitutionRowStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Среднее по РИЦ")]
        public string AVGbyInstitutionRowHeader {
            get {
                return ((string)(this["AVGbyInstitutionRowHeader"]));
            }
            set {
                this["AVGbyInstitutionRowHeader"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("font-weight:bold;color:red;display:none;")]
        public string AVGbyInstitutionsRowStyle {
            get {
                return ((string)(this["AVGbyInstitutionsRowStyle"]));
            }
            set {
                this["AVGbyInstitutionsRowStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Среднее по всем РИЦам")]
        public string AVGbyInstitutionsRowHeader {
            get {
                return ((string)(this["AVGbyInstitutionsRowHeader"]));
            }
            set {
                this["AVGbyInstitutionsRowHeader"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("font-weight:bold;")]
        public string ProgressRowStyle {
            get {
                return ((string)(this["ProgressRowStyle"]));
            }
            set {
                this["ProgressRowStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Процент успевающих")]
        public string ProgressRowHeader {
            get {
                return ((string)(this["ProgressRowHeader"]));
            }
            set {
                this["ProgressRowHeader"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string GradesRowsStyle {
            get {
                return ((string)(this["GradesRowsStyle"]));
            }
            set {
                this["GradesRowsStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("[Edu] результаты обучения сотрудников вашего РИЦ за период обучения с {0} по {1} " +
            "года")]
        public string MailSubject {
            get {
                return ((string)(this["MailSubject"]));
            }
            set {
                this["MailSubject"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("{{tables}}")]
        public string TagToTablesPaste {
            get {
                return ((string)(this["TagToTablesPaste"]));
            }
            set {
                this["TagToTablesPaste"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("{{welcome}}")]
        public string TagToWelcomePaste {
            get {
                return ((string)(this["TagToWelcomePaste"]));
            }
            set {
                this["TagToWelcomePaste"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Уважаем{0} {1}")]
        public string Welcome {
            get {
                return ((string)(this["Welcome"]));
            }
            set {
                this["Welcome"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ый")]
        public string WelcomeMalePostfix {
            get {
                return ((string)(this["WelcomeMalePostfix"]));
            }
            set {
                this["WelcomeMalePostfix"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ая")]
        public string WelcomeFemalePostfix {
            get {
                return ((string)(this["WelcomeFemalePostfix"]));
            }
            set {
                this["WelcomeFemalePostfix"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("margin: 10px; border-collapse: collapse; border: 2px solid dimgray;")]
        public string TableStyle {
            get {
                return ((string)(this["TableStyle"]));
            }
            set {
                this["TableStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string CaptionStyle {
            get {
                return ((string)(this["CaptionStyle"]));
            }
            set {
                this["CaptionStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("font-weight:bold;text-align: center;")]
        public string HeadersColumnsStyle {
            get {
                return ((string)(this["HeadersColumnsStyle"]));
            }
            set {
                this["HeadersColumnsStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("padding:5px;")]
        public string CellStyle {
            get {
                return ((string)(this["CellStyle"]));
            }
            set {
                this["CellStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("^{0} .*")]
        public string PatternToCheckDirection {
            get {
                return ((string)(this["PatternToCheckDirection"]));
            }
            set {
                this["PatternToCheckDirection"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4,9")]
        public string CuratorsRoles {
            get {
                return ((string)(this["CuratorsRoles"]));
            }
            set {
                this["CuratorsRoles"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("background-color: #ffcccc;")]
        public string BadGradeStyle {
            get {
                return ((string)(this["BadGradeStyle"]));
            }
            set {
                this["BadGradeStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("color:green;font-weight:bold;")]
        public string MaxByInstitutionsRowStyle {
            get {
                return ((string)(this["MaxByInstitutionsRowStyle"]));
            }
            set {
                this["MaxByInstitutionsRowStyle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Максимальный балл по РИЦам")]
        public string MaxByInstitutionsRowHeader {
            get {
                return ((string)(this["MaxByInstitutionsRowHeader"]));
            }
            set {
                this["MaxByInstitutionsRowHeader"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Ваш РИЦ занимает {0} место среди {1} участвующих РИЦ.")]
        public string RaitingText {
            get {
                return ((string)(this["RaitingText"]));
            }
            set {
                this["RaitingText"] = value;
            }
        }
    }
}
