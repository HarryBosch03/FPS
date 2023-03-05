using UnityEngine;

[RequireComponent(typeof(PlayerBipedController))]
public class PlayerUIDriver : MonoBehaviour
{
    [SerializeField] RadialSliderUI dashRadial;

    PlayerBipedController bipedController;
    RadialSliderUI dashRadialBackground;

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
