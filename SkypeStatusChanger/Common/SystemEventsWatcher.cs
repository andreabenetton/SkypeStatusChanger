using System;
using System.Collections.Generic;
using System.Management;

namespace SkypeStatusChanger.Common
{

    public sealed class SystemEventsWatcher : IDisposable
    {
        private const string PROCESS_INSTANCE_CREATION_EVENT = "__InstanceCreationEvent";
        private const string PROCESS_INSTANCE_DELETION_EVENT = "__InstanceDeletionEvent";
        private const string PROCESS_QUERY_CONDITION = "TargetInstance isa \"Win32_Process\" AND TargetInstance.Name=\"{0}\"";

        private List<ManagementEventWatcher> _watchers = new List<ManagementEventWatcher>();
        private readonly TimeSpan _eventQueryWithinInterval = new TimeSpan(0, 0, 1);
        private bool _disposed;

        /// <summary>
        /// Adds event handler to process start event.
        /// </summary>
        /// <param name="processName">process name with extension (*.exe)</param>
        /// <param name="handler">event handled</param>
        public void AddProcessStartEventHandler(string processName, Action handler)
        {
            WqlEventQuery query = new WqlEventQuery(PROCESS_INSTANCE_CREATION_EVENT, _eventQueryWithinInterval,
                                      string.Format(PROCESS_QUERY_CONDITION, processName));

            _watchers.Add(StartNewWatcher(query, handler));
        }

        public void AddProcessStopEventHandler(string processName, Action handler)
        {
            WqlEventQuery query = new WqlEventQuery(PROCESS_INSTANCE_DELETION_EVENT, _eventQueryWithinInterval,
                                      string.Format(PROCESS_QUERY_CONDITION, processName));

            _watchers.Add(StartNewWatcher(query, handler));
        }

        private ManagementEventWatcher StartNewWatcher(WqlEventQuery query, Action handler)
        {
            var watcher = new ManagementEventWatcher { Query = query };
            watcher.EventArrived += (s, e) => handler();
            watcher.Start();

            return watcher;
        }

        public void Dispose()
        {
            if (_disposed) return;

            foreach (ManagementEventWatcher t in _watchers)
            {
                if (t != null)
                {
                    t.Stop();
                    t.Dispose();
                }
            }

            _watchers = null;
            _disposed = true;
        }
    }
}
