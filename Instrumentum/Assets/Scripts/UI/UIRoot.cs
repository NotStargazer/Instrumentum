using System;
using UnityEngine;

namespace Instrumentum.UI
{
    public interface IUIRoot
    {
        public Camera RootCamera { get; }
        public Camera Camera { get; }
    }

    public class UIRoot : MonoBehaviour, IUIRoot
    {
        public Camera RootCamera => _gameCamera;
        public Camera Camera => _uiCamera;

        [SerializeField] private Camera _gameCamera;
        [SerializeField] private Camera _uiCamera;
        
        private GameDriver _gameDriver;

        private void Awake()
        {
            if (!_gameCamera)
            {
                throw new NullReferenceException(nameof(_gameCamera));
            }
            
            if (!_uiCamera)
            {
                throw new NullReferenceException(nameof(_uiCamera));
            }
        }

        public void OnStart(InputController inputController)
        {
            _gameDriver = new GameDriver(this, inputController);
        }
    }
}

