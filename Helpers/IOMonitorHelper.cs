using System;
using System.IO;
using System.Security.Permissions;
using Cubicles.Events;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;

namespace CubiclesPrinter.Helpers
{
    /// <summary>
    /// This class represents file monitor
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class IOMonitorHelper
    {
        /// Private Variables
        #region Private Variables

        /// <summary> File System Watcher </summary>
        private FileSystemWatcher watcher = null;

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// Starts file monitor inside the current path
        /// </summary>
        /// <param name="path">path to be monitored</param>
        public void StartMonitor(string path)
        {
            if (!Directory.Exists(path))
                IO.CreateHiddenDirectory(path);

            if (Directory.Exists(path))
            {
                // initialize watcher
                watcher = new FileSystemWatcher(path, "*.ps");

                // set filters
                watcher.NotifyFilter = NotifyFilters.DirectoryName;
                watcher.NotifyFilter = watcher.NotifyFilter | NotifyFilters.FileName;
                watcher.NotifyFilter = watcher.NotifyFilter | NotifyFilters.Attributes;

                // set up event handling
                watcher.Created += watcher_OnCreated;

                try
                {
                    watcher.EnableRaisingEvents = true;
                }
                catch (ArgumentException ex)
                {
                    LogHelper.Log(ex);
                    throw;
                }
            }
            else
            {
                LogHelper.Log(string.Format("Unable to monitor folder: {0}. Folder does not exist.", path));
            }
        }

        /// <summary>
        /// Stops monitor
        /// </summary>
        public void StopMonitor()
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
        }

        #endregion

        /// Event Subscriptions
        #region Event Subscriptions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_OnCreated(object sender, System.IO.FileSystemEventArgs e)
        {
            var sleepTimeout = 100;
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    while (IO.IsFileLocked(e.FullPath) == true)
                    {
                        //LogHelper.Log("Locked...Sleeping...");
                        System.Threading.Thread.Sleep(sleepTimeout);
                    }
                    LogHelper.Log(string.Format("Created: {0}", e.FullPath));

                    FileFound(this, new DataEventArgs(e.FullPath));
                    break;
            }
        }

        #endregion

        /// Events
        #region Events

        /// <summary>
        /// File found event
        /// </summary>
        public event EventHandler<DataEventArgs> FileFound;
        
        #endregion
    }
}
