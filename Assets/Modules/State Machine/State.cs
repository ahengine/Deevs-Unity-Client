using UnityEngine;

namespace Patterns.FSMMonoBase
{
    public class State<T> : MonoBehaviour
    {
        [field:SerializeField] public T ID { get; private set; }


        public virtual void Enter()
        {

        }

        public virtual void Updates()
        {

        }

        public virtual void LateUpdates()
        {

        }

        public virtual void FixedUpdates()
        {

        }

        public virtual void Exit()
        {

        }
    }
}

