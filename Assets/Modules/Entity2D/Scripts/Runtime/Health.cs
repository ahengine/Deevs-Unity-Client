using UnityEngine;
using System;

namespace Entities
{
    [Serializable]
    public class Health
    {
        [field: SerializeField] public int Default { private set; get; } = 100;
        [field: SerializeField] public int Max { private set; get; } = 100;
        public int Current { private set; get; }
        public float CurrentFillAmount => (float)Current / (float)Max;

        public event Action<int, int> OnAdd;
        public event Action<int> OnDamage;
        public event Action OnDeath;

        public void Init()
        {
            Current = 0;
            Add(Default);
        }

        public void Add(int value)
        {
            Current += value;
            if(Current > Max) Current = Max;
            OnAdd?.Invoke(Current, value);
        }

        public void ApplyDamage(int damage)
        {
            if (Current == 0) return;

            if (Current <= damage)
            {
                ApplyDeath();
                return;
            }

            Current -= damage;
            OnDamage?.Invoke(Current);
        }

        public void ApplyDeath()
        {
            if (Current == 0) return;

            Current = 0;
            OnDeath?.Invoke();
        }
    }
}