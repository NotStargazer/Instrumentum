using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Instrumentum.UI
{
    public interface IUIRoot
    {
        public Camera Camera { get; }
    }

    public class UIRoot : MonoBehaviour, IUIRoot
    {
        public Camera Camera => _uiCamera;

        [SerializeField] private Camera _uiCamera;
        
        private Driver _driver;

        private void Awake()
        {
            if (!_uiCamera)
            {
                throw new NullReferenceException(nameof(_uiCamera));
            }
        }

        public void OnStart(InputController inputController)
        {
            _driver = new Driver(this, inputController);
        }
    }
}

