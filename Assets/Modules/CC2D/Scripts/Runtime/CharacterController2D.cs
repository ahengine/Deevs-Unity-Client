using UnityEngine;
using System;
using CC2D.Modules;
using Unity.Mathematics;

namespace CC2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterController2D : MonoBehaviour
    {
        [SerializeField] private float horizontalSpeed = 1;

        [SerializeField] private Vector2 velocity = Vector2.zero;
        public Vector2 Velocity => velocity;
        public float VelocityX { set => velocity.x = value; get => velocity.x; }
        public float VelocityY { set => velocity.y = value; get => velocity.y; }

        public Transform Tr { private set; get; }
        private Rigidbody2D rb;
        internal int FaceDirection { private set; get; } = 1;
        public bool AllowAction { private set; get; } = true;

        [field: SerializeField] public CC2DModule[] Modules { private set; get; }

        public T GetModule<T>() where T : CC2DModule
        {
            for (int i = 0; i < Modules.Length; i++)
                if (Modules[i] is T)
                    return Modules[i] as T;

            return null;
        }

        private void Awake()
        {
            Tr = transform;
            rb = GetComponent<Rigidbody2D>();

            for (int i = 0; i < Modules.Length; i++)
                Modules[i].SetOwner(this);
        }

        private void Update()
        {
            for (int i = 0; i < Modules.Length; i++)
                if (Modules[i].IsActive)
                    Modules[i].Process();
        }

        private void FixedUpdate() =>
            rb.velocity = velocity;

        public void SetHorizontal(float value)
        {
            for (int i = 0; i < Modules.Length; i++)
                if (Modules[i].IsActive && !Modules[i].AllowHorizontalMove)
                    return;

            if (!AllowAction) return;

            VelocityX = value * horizontalSpeed;
            if (value != 0)
                FaceDirection = value > 0 ? 1 : -1;
        }

        public void SetAllowAction(bool value)
        {
            AllowAction = value;
            if (!AllowAction)
                VelocityX = 0;
        }
    }
}