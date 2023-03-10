using Code.Scripts.Editor_Tools;
using UnityEngine;

namespace Code.Scripts.Signals
{
    [RequireComponent(typeof(ParticleSystem))]
    public class SignalTriggersParticleEffect : MonoBehaviour
    {
        [SerializeField][CannotBeNull] private Signal signal;
        [SerializeField] private bool global = false;

        private ParticleSystem[] systems;

        private void Awake()
        {
            systems = GetComponentsInChildren<ParticleSystem>();
        }
        
        private void OnEnable()
        {
            signal.RaiseEvent += OnRaise;
        }

        private void OnDisable()
        {
            signal.RaiseEvent -= OnRaise;
        }

        private void OnRaise(GameObject caller)
        {
            if (!transform.IsChildOf(caller.transform) && !global) return;
            
            foreach (var system in systems) system.Play();
        }
    }
}
