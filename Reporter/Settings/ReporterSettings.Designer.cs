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
    public sealed partial class ReporterSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static ReporterSettings defaultInstance = ((ReporterSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ReporterSettings())));
        
        public static ReporterSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MoodleReporter")]
        public string DirrectoryForEmails {
            get {
                return ((string)(this["DirrectoryForEmails"]));
            }
            set {
                this["DirrectoryForEmails"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Windows-1251")]
        public string CsvEncoding {
            get {
                return ((string)(this["CsvEncoding"]));
            }
            set {
                this["CsvEncoding"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(";")]
        public string CsvDelimiter {
            get {
                return ((string)(this["CsvDelimiter"]));
            }
            set {
                this["CsvDelimiter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("yyyy-MM-dd HH.mm.ss")]
        public string DateFormatForFolderName {
            get {
                return ((string)(this["DateFormatForFolderName"]));
            }
            set {
                this["DateFormatForFolderName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("{0}-{1}.eml")]
        public string EmailName {
            get {
                return ((string)(this["EmailName"]));
            }
            set {
                this["EmailName"] = value;
            }
        }
    }
}
