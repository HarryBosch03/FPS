using UnityEngine;

namespace Code.Scripts.Player
{
    [RequireComponent(typeof(PlayerBipedController))]
    public class PlayerCameraDriver : MonoBehaviour
    {
        private PlayerBipedController bipedController;
        [SerializeField] private float cameraSway;
        [SerializeField] private float maxCamSway;

        private Transform target;

        private void Awake()
        {
            bipedController = GetComponent<PlayerBipedController>();
        }

        private void Start()
        {
            target = bipedController.Head.GetChild(0);
        }

        private void Update()
        {
            var sway = Vector3.Dot(transform.right, bipedController.RelativeVelocity) * cameraSway;
            sway = Mathf.Clamp(sway, -maxCamSway, maxCamSway);
            target.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -sway);
        }
    }
}