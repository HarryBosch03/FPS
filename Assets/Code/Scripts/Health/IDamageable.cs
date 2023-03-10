using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Scripts.Health
{
    public interface IDamageable
    {
        int CurrentHealth { get; }
        int MaxHealth { get; }

        event System.Action<DamageArgs> DamageEvent; // Called between damage applied and death check.
        event System.Action<DamageArgs> DeathEvent; // Called before death.
        event System.Action<DamageArgs> HealEvent; // Called after heal.
        
        void Damage(DamageArgs args);
        void Heal(DamageArgs args);

        void RegisterDamageListener(DamageListener listener);
        void DeregisterDamageListener(DamageListener listener);
        
        void RegisterHealListener(DamageListener listener);
        void DeregisterHealListener(DamageListener listener);
    }
    
    public struct DamageArgs
    {
        public GameObject dealer;
        public float damage;

        public DamageArgs(GameObject dealer, float damage)
        {
            this.dealer = dealer;
            this.damage = damage;
        }
    }

    public class DamageListener
    {
        public DamageModification callback;
        public int priority;

        public DamageListener(DamageModification callback, int priority = 0)
        {
            this.callback = callback;
            this.priority = priority;
        }

        public static void ExecuteAll(IEnumerable<DamageListener> listeners, ref DamageArgs args)
        {
            foreach (var listener in listeners) listener?.callback(ref args);
        }

        public static int InsertSorted(IList<DamageListener> listeners, DamageListener newListener)
        {
            var h = 0;
            foreach (var listener in listeners)
            {
                if (listener.priority > newListener.priority) break;
                h++;
            }
            listeners.Insert(h, newListener);
            return h;
        }
    }

    public delegate void DamageModification(ref DamageArgs args);
}