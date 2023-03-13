using System;
using UnityEngine;

namespace Code.Scripts.Signals
{
    public class SignalListener
    {
        public string messageName;
        public readonly Action callback;
        public readonly GameObject listenContext;
        public readonly bool global;

        public SignalListener(Action callback, GameObject listenContext, bool global = false)
        {
            this.callback = callback;
            this.listenContext = listenContext;
            this.global = global;
        }

        public SignalListener(string messageName, Action callback, GameObject listenContext, bool global = false) : this(callback, listenContext, global)
        {
            Register(messageName);
        }

        public void Register(string messageName)
        {
            Signal.Register(messageName, this);
        }

        public void Deregister()
        {
            Signal.Deregister(this);
        }

        public void TryCall(GameObject callContext)
        {
            if (!global && !listenContext.transform.IsChildOf(callContext.transform)) return;
            callback();
        }
    }
}