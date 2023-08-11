using System.Collections.Generic;

namespace Instrumentum.UI
{
    public enum DriverState
    {
        NotePlacement
    }

    public interface IDriverState : EditorInputs.IEditorActions
    {
        public void OnStateEnter(IUIRoot ui, DriverState? from);
        public void OnStateExit(DriverState to);
    }

    public class GameDriver
    {
        private UIRoot _uiRoot;
        private InputController _input;

        private Dictionary<DriverState, IDriverState> _states;

        private DriverState _currentDriverState;

        internal GameDriver(UIRoot ui, InputController input)
        {
            _uiRoot = ui;
            _input = input;

            _states = new Dictionary<DriverState, IDriverState>
            {
                
            };
            
            _currentDriverState = GlobalResources.Instance.InitialState;
            var state = _states[_currentDriverState];
            state.OnStateEnter(_uiRoot, null);
            _input.EditorActions.SetCallbacks(state);
        }

        internal void OnChangeState(DriverState nextState)
        {
            var nextDriverState = _states[nextState];
            var currentDriverState = _states[_currentDriverState];

            currentDriverState.OnStateExit(nextState);
            _input.EditorActions.SetCallbacks(nextDriverState);
            nextDriverState.OnStateEnter(_uiRoot, _currentDriverState);

            _currentDriverState = nextState;
        }
    }
}