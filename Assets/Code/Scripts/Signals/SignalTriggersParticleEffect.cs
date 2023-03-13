using UnityEngine;

namespace Code.Scripts.Signals
{
    public class SignalTriggersParticleEffect : MonoBehaviour
    {
        [SerializeField] private string signalName;
        [SerializeField] private bool global = false;
        
        private ParticleSystem[] systems;
        private SignalListener listener;

        private void Awake()
        {
            systems = GetComponentsInChildren<ParticleSystem>();
        }
        
        private void OnEnable()
        {
            listener = new SignalListener(signalName, OnRaise, gameObject, global);
        }

        private void OnDisable()
        {
            listener.Deregister();
        }

        private void OnRaise()
        {
            foreach (var system in systems) system.Play();
        }
    }
}
