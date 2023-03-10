using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Health
{
    [System.Serializable]
    public class HealthManager : MonoBehaviour, IDamageable
    {
        [SerializeField] private int currentHealth = 10, maxHealth = 10;
        [SerializeField] private DeathAction deathAction = DeathAction.Destroy;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        private List<DamageListener> damageListeners = new();
        private List<DamageListener> healListeners = new();

        public event Action<DamageArgs> DamageEvent;
        public event Action<DamageArgs> DeathEvent;
        public event Action<DamageArgs> HealEvent;

        public void Damage(DamageArgs args)
        {
            DamageListener.ExecuteAll(damageListeners, ref args);
            
            currentHealth -= (int)args.damage;

            DamageEvent?.Invoke(args);

            if (currentHealth <= 0)
            {
                Die(args);
            }
        }

        public void Heal(DamageArgs args)
        {
            DamageListener.ExecuteAll(healListeners, ref args);
            
            currentHealth += (int)args.damage;
            if (currentHealth > maxHealth) currentHealth = maxHealth;

            HealEvent?.Invoke(args);
        }

        public void RegisterDamageListener(DamageListener listener) => DamageListener.InsertSorted(damageListeners, listener);
        public void DeregisterDamageListener(DamageListener listener) => damageListeners.Remove(listener);

        public void RegisterHealListener(DamageListener listener) => DamageListener.InsertSorted(healListeners, listener);
        public void DeregisterHealListener(DamageListener listener) => healListeners.Remove(listener);

        private void Die(DamageArgs args)
        {
            DeathEvent?.Invoke(args);

            switch (deathAction)
            {
                case DeathAction.None:
                    break;
                case DeathAction.Disable:
                    gameObject.SetActive(false);
                    break;
                case DeathAction.Destroy:
                    UnityEngine.Object.Destroy(gameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum DeathAction
        {
            None,
            Disable,
            Destroy,
        }
    }
}