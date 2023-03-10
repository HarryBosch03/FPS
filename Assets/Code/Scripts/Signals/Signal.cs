using UnityEngine;

namespace Code.Scripts.Signals
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Signals/Signal")]
    public class Signal : ScriptableObject
    {
        private const bool LogCalls = true;
        
        public event SignalCallback RaiseEvent;
        public event SignalCallbackArgs RaiseEventArgs;

        public void Raise(GameObject caller, object args = null)
        {
            RaiseEvent?.Invoke(caller);
            RaiseEventArgs?.Invoke(caller, args);

            if (LogCalls) Debug.Log($"Signal: \"{name}\" was called by \"{caller}\"", this);
        }

        public delegate void SignalCallback(GameObject caller);
        public delegate void SignalCallbackArgs(GameObject caller, object args);
    }
}
