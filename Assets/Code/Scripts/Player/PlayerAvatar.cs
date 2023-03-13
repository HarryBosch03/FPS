using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Player
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerBipedController))]
    [RequireComponent(typeof(PlayerWeaponManager))]
    public class PlayerAvatar : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputAsset;
        [SerializeField] private float mouseSensitivity = 0.3f;
        
        private PlayerBipedController bipedController;
        private InputAction dashAction;
        private InputAction jumpAction;

        private InputActionMap map;
        private InputAction moveAction;
        private InputAction shootAction;
        private InputAction switchWeaponAction;
        private InputAction scrollAction;
        private InputAction holsterAction;
        
        private PlayerWeaponManager weaponManager;

        private void Awake()
        {
            bipedController = GetComponent<PlayerBipedController>();
            weaponManager = GetComponent<PlayerWeaponManager>();

            map = inputAsset.FindActionMap("Player");
            map.Enable();

            moveAction = map.FindAction("Move");
            jumpAction = map.FindAction("Jump");
            dashAction = map.FindAction("Dash");
            shootAction = map.FindAction("Shoot");
            switchWeaponAction = map.FindAction("SwitchWeapons");
            holsterAction = map.FindAction("Holster");

            scrollAction = new InputAction(binding: "<Mouse>/scroll/y", type: InputActionType.Value, expectedControlType: "Axis");
            scrollAction.Enable();
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;

            switchWeaponAction.performed += SwitchWeapons;
            holsterAction.performed += HolsterWeapon;
            scrollAction.performed += ScrollWeapons;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            
            switchWeaponAction.performed -= SwitchWeapons;
            holsterAction.performed -= HolsterWeapon;
            scrollAction.performed -= ScrollWeapons;
            
            scrollAction.Disable();
            scrollAction.Dispose();
        }

        private void Update()
        {
            bipedController.MoveDirection = ReposeMoveInput(moveAction.ReadValue<Vector2>());
            bipedController.Jump = Switch(jumpAction);
            bipedController.Dash = Switch(dashAction);
            weaponManager.Shoot = Switch(shootAction);

            if (Mouse.current != null)
                bipedController.LookRotation += Mouse.current.delta.ReadValue() * mouseSensitivity;
        }

        private void SwitchWeapons(InputAction.CallbackContext ctx)
        {
            var i = (int)ctx.ReadValue<float>() - 1;
            weaponManager.EquipWeapon(i);
        }
        
        private void ScrollWeapons(InputAction.CallbackContext ctx)
        {
            var i = ctx.ReadValue<float>() > 0.0f ? 0 : 1;
            weaponManager.EquipWeapon(i);
        }
        
        private void HolsterWeapon(InputAction.CallbackContext ctx) => weaponManager.HolsterWeapon();
        
        public static Vector3 ReposeMoveInput(Vector2 vec) => new Vector3(vec.x, 0.0f, vec.y);
        public static bool Switch(InputAction action) => action.ReadValue<float>() > 0.5f;
    }
}