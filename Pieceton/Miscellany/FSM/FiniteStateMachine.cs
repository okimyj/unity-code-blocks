namespace Pieceton.Misc
{
    public abstract class PFSMState<T>
    {
        public abstract void Enter(T _entity);
        public abstract void Execute(T _entity, float _delta_time);
        public abstract void Exit(T _entity);
    }

    public class PFiniteStateMachine<T>
    {
        private T owner;
        public PFSMState<T> currState { get; private set; }
        public PFSMState<T> prevState { get; private set; }

        public void Awake()
        {
            currState = null;
            prevState = null;
        }

        public void Configure(T _owner, PFSMState<T> InitialState)
        {
            owner = _owner;
            ChangeState(InitialState, true);
        }

        public void Update(float _delta_time)
        {
            if (null == owner)
                return;

            if (null == currState)
                return;

            currState.Execute(owner, _delta_time);
        }

        public bool ChangeState(PFSMState<T> _new_state, bool _force)
        {
            if (null == owner)
                return false;

            if (!_force && (null != currState))
            {
                if (currState.Equals(_new_state))
                    return false;
            }

            prevState = currState;

            if (null != prevState)
            {
                currState.Exit(owner);
            }

            currState = _new_state;

            if (null != currState)
            {
                currState.Enter(owner);
            }

            return true;
        }
    };
}