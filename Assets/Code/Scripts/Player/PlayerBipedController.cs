using System;
using UnityEngine;

public class PlayerBipedController : BipedController
{
    [Space]
    [SerializeField] float dashDistance;
    [SerializeField] float dashDuration;
    [SerializeField][Range(0.0f, 1.0f)] float velocityCancelation;

    [Space]
    [SerializeField] int maxDashCharges;
    [SerializeField] float dashRechargeTime;
    [SerializeField] float dashRechargeDelay;

    bool dashLast;
    bool dashing;
    float dashTime = float.NegativeInfinity;
    Vector3 dashDirection;
    float dashCharge;
    float lastDashTime;

    public bool Dash { get; set; }
    public float DashCharge => dashCharge;
    public int MaxDashCharges => maxDashCharges;

    protected override void Actions()
    {
        base.Actions();

        DashAction();
    }

    private void DashAction()
    {
        if (Grounded && Time.time - lastDashTime > dashRechargeDelay)
        {
            dashCharge += Time.deltaTime / dashRechargeTime;
        }
        dashCharge = Mathf.Clamp(dashCharge, 0.0f, maxDashCharges);

        if (dashing)
        {
            if (dashTime <= dashDuration)
            {
                velocity = dashDirection * dashDistance / dashTime;
                acceleration = Vector3.zero;
            }
            else
            {
                velocity = dashDirection * (dashDistance / dashTime) * velocityCancelation;
                dashing = false;
            }
        }

        if (!Dash) return;
        if (dashLast) return;
        if (dashCharge <= 0.95f) return;

        if (MoveDirection.sqrMagnitude < 0.1f * 0.1f) return;

        dashDirection = transform.TransformDirection(MoveDirection).normalized;
        dashing = true;
        dashTime = 0.0f;
        dashCharge--;
        lastDashTime = Time.time;
    }

    protected override void SetNextFrameFlags()
    {
        base.SetNextFrameFlags();

        dashLast = Dash;
        dashTime += Time.deltaTime;
    }
}
