using UnityEngine;

namespace Code.Scripts.FX
{
    [CreateAssetMenu(menuName = "Scriptable Objects/FX/Bullet Holes")]
    public class BulletHoles : ScriptableObject
    {
        private const float ZOffset = 0.01f;
        
        [SerializeField] private ParticleSystem systemPrefab;

        private ParticleSystem systemInstance;
        
        public void Spawn(Vector3 position, Vector3 direction)
        {
            if (!systemInstance)
            {
                systemInstance = Instantiate(systemPrefab);
            }

            var emmitParams = new ParticleSystem.EmitParams
            {
                position = position + direction * ZOffset
            };
            
            Debug.DrawRay(position, direction, Color.red, 5.0f);
            direction.z = -direction.z;
            emmitParams.rotation3D = Quaternion.LookRotation(direction).eulerAngles;
            
            systemInstance.Emit(emmitParams, 1);
        }

        private void OnDisable()
        {
            if (!systemInstance) return;
            Destroy(systemPrefab.gameObject);
        }
    }
}
