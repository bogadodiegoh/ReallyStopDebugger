﻿using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace lggomez.ReallyStopDebugger
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        internal Package currentPackage { get; set; }
        internal SettingsManager settingsManager { get; set; }

        public MyControl()
        {
            InitializeComponent();
            StatusLabel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            #region Stop debug mode

            try
            {
                var dte = ((ReallyStopDebuggerPackage)currentPackage).GetDte();
                //Stop local VS debug
                if (dte != null)
                {
                    dte.ExecuteCommand("Debug.StopDebugging");
                }
            }
            catch (COMException)
            { //The command Debug.StopDebugging is not available
            }

            #endregion

            string[] processNames = ProcessesTextBox.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            var result = Common.ProcessHelper.KillProcesses(currentPackage, processNames);

            #region UI update

            StatusLabel.Visibility = System.Windows.Visibility.Visible;

            string returnMessage;

            switch (result)
            {
                case 0:
                    {
                        returnMessage = "Processes killed.";
                        StatusLabel.Foreground = System.Windows.Media.Brushes.Green;
                        break;
                    }
                case 1:
                    {
                        returnMessage = "Could not find any matching processes.";
                        StatusLabel.Foreground = System.Windows.Media.Brushes.Black;
                        break;
                    }
                default:
                    {
                        returnMessage = "Could not close orphaned processes due to an error.";
                        StatusLabel.Foreground = System.Windows.Media.Brushes.Red;
                        break;
                    }
            }

            StatusLabel.Content = returnMessage;

            #endregion
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            #region Load settings

            SettingsStore configurationSettingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);
            bool collectionExists = configurationSettingsStore.CollectionExists("ReallyStopDebugger");

            if (collectionExists)
            {
                ProcessesTextBox.Text = configurationSettingsStore.GetString("ReallyStopDebugger", "ProcessList") ?? string.Empty;
            }

            #endregion
        }

        private void StackPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            #region Save settings

            WritableSettingsStore userSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            bool collectionExists = userSettingsStore.CollectionExists("ReallyStopDebugger");

            if (!collectionExists)
            {
                userSettingsStore.CreateCollection("ReallyStopDebugger");
            }

            userSettingsStore.SetString("ReallyStopDebugger", "ProcessList", ProcessesTextBox.Text);

            #endregion
        }
    }
}