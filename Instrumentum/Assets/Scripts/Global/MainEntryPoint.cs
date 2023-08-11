using System;
using UnityEngine;

namespace Instrumentum
{
    [Serializable]
    public struct PreloadedObjects
    {
        public string Name;
        public GameObject Prefab;
    }
    
    public class MainEntryPoint : SingletonBehaviour<MainEntryPoint>
    {
        [SerializeField] private PreloadedObjects[] Preloaders;
        
        public override void Instantiate()
        {
            DontDestroyOnLoad(gameObject);

            foreach (var preloader in Preloaders)
            {
                var go = Instantiate(preloader.Prefab);
                go.name = $"[{preloader.Name}]";
                DontDestroyOnLoad(go);
            }
        }
    }
}
