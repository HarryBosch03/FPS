using UnityEngine;

namespace Code.Scripts.Player
{
    [RequireComponent(typeof(PlayerBipedController))]
    public class PlayerCameraDriver : MonoBehaviour
    {
        [SerializeField] float cameraSway;
        [SerializeField] float maxCamSway;

        Transform target;

        PlayerBipedController bipedController;

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
            float sway = Vector3.Dot(transform.right, bipedController.RelativeVelocity) * cameraSway;
            sway = Mathf.Clamp(sway, -maxCamSway, maxCamSway);
            target.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -sway);
        }
    }
}
