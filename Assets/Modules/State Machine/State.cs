using Action = System.Action;

namespace Patterns
{
    public class State<T>
    {
        public string Name { get; set; }

        public T ID { get; private set; }

        public FiniteStateMachine<T> StateMachine { get; set; }

        public State(T id) => ID = id;
        public State(T id, string name) : this(id) => Name = name;


        public Action OnEnter;
        public Action OnExit;
        public Action OnUpdate;
        public Action OnLateUpdate;
        public Action OnFixedUpdate;

        public State(T id,
            Action onEnter,
            Action onExit = null,
            Action onUpdate = null,
            Action onLateUpdate = null,
            Action onFixedUpdate = null) : this(id)
        {
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
            OnLateUpdate = onLateUpdate;
            OnFixedUpdate = onFixedUpdate;
        }
        public State(T id, 
            string name,
            Action onEnter,
            Action onExit = null,
            Action onUpdate = null,
            Action onLateUpdate = null,
            Action onFixedUpdate = null) : this(id, name)
        {
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
            OnLateUpdate = onLateUpdate;
            OnFixedUpdate = onFixedUpdate;
        }

        virtual public void Enter() => OnEnter?.Invoke();

        virtual public void Exit() => OnExit?.Invoke();
        virtual public void Update() => OnUpdate?.Invoke();

        virtual public void LateUpdate() => OnLateUpdate?.Invoke();
        virtual public void FixedUpdate() => OnFixedUpdate?.Invoke();
    }
}

