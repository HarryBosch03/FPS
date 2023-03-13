using Code.Scripts.UI;
using UnityEngine;

namespace Code.Scripts.Player
{
    [RequireComponent(typeof(PlayerBipedController))]
    public class PlayerUIDriver : MonoBehaviour
    {
        private PlayerBipedController bipedController;
        [SerializeField] private RadialSliderUI dashRadial;
        private RadialSliderUI dashRadialBackground;

        private void Awake()
        {
            bipedController = GetComponent<PlayerBipedController>();
            dashRadialBackground = dashRadial.transform.parent.GetComponent<RadialSliderUI>();
        }

        private void Update()
        {
            dashRadial.Fill = bipedController.DashCharge / bipedController.MaxDashCharges;
            dashRadial.Segments = bipedController.MaxDashCharges;

            dashRadialBackground.Fill = 1.0f;
            dashRadialBackground.Segments = bipedController.MaxDashCharges;
        }
    }
}