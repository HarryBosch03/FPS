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
        [SerializeField] InputActionAsset inputAsset;
        [SerializeField] float mouseSensitivity = 0.3f;

        PlayerBipedController bipedController;
        PlayerWeaponManager weaponManager;

        InputActionMap map;
        InputAction moveAction;
        InputAction jumpAction;
        InputAction dashAction;
        InputAction shootAction;

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
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            bipedController.MoveDirection = ReposeMoveInput(moveAction.ReadValue<Vector2>());
            bipedController.Jump = Switch(jumpAction);
            bipedController.Dash = Switch(dashAction);
            weaponManager.Shoot = Switch(shootAction);

            if (Mouse.current != null)
            {
                bipedController.LookRotation += Mouse.current.delta.ReadValue() * mouseSensitivity;
            }
        }

        public Vector3 ReposeMoveInput(Vector2 vec) => new Vector3(vec.x, 0.0f, vec.y);

        public bool Switch(InputAction action) => action.ReadValue<float>() > 0.5f;
    }
}
