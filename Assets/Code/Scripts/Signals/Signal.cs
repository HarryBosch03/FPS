using System.Collections.Generic;
using Code.Scripts.Utility;
using UnityEngine;

namespace Code.Scripts.Signals
{
    public static class Signal
    {
        private static readonly Dictionary<string, List<SignalListener>> Listeners = new();

        public static void Register(string messageName, SignalListener listener)
        {
            Util.SimplifyName(ref messageName);
            
            if (!Listeners.ContainsKey(messageName)) Listeners.Add(messageName, new List<SignalListener>());
            Listeners[messageName].Add(listener);
            
            listener.messageName = messageName;
        }

        public static void Deregister(SignalListener listener)
        {
            if (!Listeners.ContainsKey(listener.messageName)) return;
            Listeners[listener.messageName].Remove(listener);
            if (Listeners[listener.messageName].Count == 0) Listeners.Remove(listener.messageName);
        }

        public static void Call(string message, GameObject callContext)
        {
            Util.SimplifyName(ref message);
            if (!Listeners.ContainsKey(message)) return;

            foreach (var listener in Listeners[message])
            {
                listener.TryCall(callContext);
            }
        } 
    }
}