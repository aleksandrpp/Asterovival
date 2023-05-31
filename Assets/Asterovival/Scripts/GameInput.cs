using UnityEngine;
using UnityEngine.InputSystem;

namespace AK.Asterovival
{
    public class GameInput : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;

        public InputAction FireAction { get; private set; }
        public InputAction LaserAction { get; private set; }
        public InputAction ThrustAction { get; private set; }
        public InputAction LeftAction { get; private set; }
        public InputAction RightAction { get; private set; }
        public InputAction RestartAction { get; private set; }

        private void Awake()
        {
            FireAction = _input.actions["Fire"];
            LaserAction = _input.actions["Laser"];
            ThrustAction = _input.actions["Thrust"];
            LeftAction = _input.actions["Left"];
            RightAction = _input.actions["Right"];
            RestartAction = _input.actions["Restart"];
        }
    }
}
