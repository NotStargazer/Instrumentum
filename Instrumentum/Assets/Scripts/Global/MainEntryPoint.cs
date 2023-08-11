using System;
using UnityEngine;

namespace Instrumentum.UI
{
    public class MainEntryPoint : SingletonBehaviour<MainEntryPoint>
    {
        [SerializeField] private UIRoot _uiRootPrefab;
        [SerializeField] private InputController _inputControllerPrefab;
        [SerializeField] private GlobalResources _globalResources;
        
        private UIRoot _uiRootInstance;
        private InputController _inputControllerInstance;
        
        public override void Instantiate()
        {
            DontDestroyOnLoad(gameObject);

            if (!_uiRootPrefab)
            {
                throw new ArgumentNullException(nameof(_uiRootPrefab));
            }

            if (!_inputControllerPrefab)
            {
                throw new ArgumentNullException(nameof(_inputControllerPrefab));
            }
            
            if (!_globalResources)
            {
                throw new ArgumentNullException(nameof(_globalResources));
            }
            
            _inputControllerInstance = Instantiate(_inputControllerPrefab);
            var inputGameObject = _inputControllerInstance.gameObject;
            inputGameObject.name = "[Input Controller]";
            DontDestroyOnLoad(inputGameObject);
            
            var globalResources = Instantiate(_globalResources);
            globalResources.name = "[Global Resources]";
            DontDestroyOnLoad(globalResources);

            _uiRootInstance = Instantiate(_uiRootPrefab);
            var uiRoot = _uiRootInstance.gameObject;
            uiRoot.name = "[UI Root]";
            DontDestroyOnLoad(uiRoot);
            
            _uiRootInstance.OnStart(_inputControllerInstance);
        }
    }
}
