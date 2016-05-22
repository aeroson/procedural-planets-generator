﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEngine.Events
{
    // TODO: add WeakReference (weak event pattern) probably WeakEventManager https://msdn.microsoft.com/en-us/library/system.windows.weakeventmanager(v=vs.100).aspx
    public class EventSystem 
    {
        Dictionary<Type, Delegate> typeToCallbacks = new Dictionary<Type, Delegate>();
        HashSet<Delegate> allDelegates = new HashSet<Delegate>();
        List<EventSystem> passEventsTo = new List<EventSystem>();
        public event Action<IEvent> OnAnyEventCalled;


        public void Raise(IEvent evt)
        {
            Delegate delegat;
            var type = evt.GetType();
            if (typeToCallbacks.TryGetValue(type, out delegat) == true)
            {
                delegat.DynamicInvoke(evt);
            }
            if (OnAnyEventCalled != null) OnAnyEventCalled(evt);
            foreach (var e in passEventsTo) e.Raise(evt);
        }
        public void PassEventsTo(EventSystem eventSystem)
        {
            passEventsTo.Add(eventSystem);
        }

        public void Register<T>(Action<T> callback) where T : IEvent
        {
            lock(allDelegates)
            {
                if (allDelegates.Contains(callback)) return;
                allDelegates.Add(callback);

                Delegate callbackToCombine;
                var type = typeof(T);
                if (typeToCallbacks.TryGetValue(type, out callbackToCombine) == false)
                {
                    typeToCallbacks[type] = callback;
                }
                else {
                    typeToCallbacks[type] = System.Delegate.Combine(callbackToCombine, callback);
                }
            }
        }

        public void Unregister<T>(Action<T> callback) where T : IEvent
        {
            lock(allDelegates)
            {
                if (allDelegates.Contains(callback) == false) return;
                allDelegates.Remove(callback);

                Delegate callbackToCombineTo;
                var type = typeof(T);
                if (typeToCallbacks.TryGetValue(type, out callbackToCombineTo))
                {                    
                    System.Delegate.Remove(callbackToCombineTo, callback);
                }
            }
        }

    }    
}
