// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using Newtonsoft.Json;
using TKUtils;
using System.Windows;
#endregion

namespace MyScheduledTasks
{
    public class MySettings : INotifyPropertyChanged
    {
        public MySettings()
        {

        }
        /////////////////////////////  Properties  //////////////////////////////

        #region Backing fields
        private bool alertCol;
        private double windowTop;
        private double windowLeft;
        #endregion

        [DefaultValue(100)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public double WindowTop
        {
            get { return windowTop; }
            set
            {
                windowTop = value;
                RaisePropertyChanged("WindowTop");
            }
        }

        [DefaultValue(100)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public double WindowLeft
        {
            get { return windowLeft; }
            set
            {
                windowLeft = value;
                RaisePropertyChanged("WindowLeft");
            }
        }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool AlertCol
        {
            get { return alertCol; }
            set
            {
                alertCol = value;
                RaisePropertyChanged("AlertCol");
            }
        }


        //////////////////////////  Property Changed  /////////////////////////

        #region Property changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
                Debug.WriteLine($">>> Property Changed: {name}");
            }
        }
        #endregion

        /////////////////////////////  Methods  //////////////////////////////

        #region Read settings
        /// <summary>
        /// Reads settings from the specified JSON file
        /// </summary>
        /// <param name="filename">If filename parameter is empty, the default settings.json will be used</param>
        /// <returns>MySettings object</returns>
        public static MySettings Read(string filename = "")
        {
            if (string.IsNullOrEmpty(filename))
            {
                filename = DefaultSettingsFile();
            }
            if (!File.Exists(filename))
            {
                Debug.WriteLine($"Settings file not found - Creating");
                CreateEmptySettingsFile(filename);
            }
            try
            {
                string rawJSON = File.ReadAllText(filename);
                return JsonConvert.DeserializeObject<MySettings>(rawJSON);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Settings file could not be read - cannot continue");
                Debug.WriteLine($"Error reading {filename}");
                Debug.WriteLine($"{ex}");
                Application.Current.Shutdown();
                return null;
            }
        }
        #endregion

        #region Save settings
        /// <summary>
        /// Saves settings to the specified JSON file
        /// </summary>
        /// <param name="s">name of object containing settings</param>
        /// <param name="filename">If filename parameter is empty, the default settings.json will be used</param>
        public static void Save(MySettings s, string filename = "")
        {
            if (string.IsNullOrEmpty(filename))
            {
                filename = DefaultSettingsFile();
            }
            if (!File.Exists(filename))
            {
                Debug.WriteLine($"Settings file not found - cannot save settings");
                Debug.WriteLine($"{filename} - not found");
                return;
            }
            else
            {
                Debug.WriteLine($"Saving settings to {filename}");
            }
            try
            {
                string jsonOut = JsonConvert.SerializeObject(s, Formatting.Indented);
                File.WriteAllText(filename, jsonOut);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Settings could not be saved");
                Debug.WriteLine($"{ex}");
            }
        }
        #endregion

        #region List settings
        /// <summary>
        /// Lists current settings
        /// </summary>
        /// <param name="s">name of object containing settings</param>
        /// <returns>List of strings </returns>
        public static List<string> List(MySettings s)
        {
            List<string> list = new List<string>();

            foreach (PropertyInfo prop in s.GetType().GetProperties())
            {
                Debug.WriteLine($"{prop.Name} : {prop.GetValue(s) }");
                list.Add($"{prop.Name} = {prop.GetValue(s)}");
            }
            return list;
        }
        #endregion

        #region Helper Methods
        private static string DefaultSettingsFile()
        {
            string dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return Path.Combine(dir, "settings.json");
        }

        private static void CreateEmptySettingsFile(string filename)
        {
            try
            {
                File.WriteAllText(filename, "{ }");
                Debug.WriteLine($"{filename} - Created");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Settings file could not be created - cannot continue");
                Debug.WriteLine($"{ex}");
                Environment.Exit(1);
            }
        }
        #endregion
    }
}
