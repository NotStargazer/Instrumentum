using UnityEngine;

namespace Instrumentum.UI
{
    public class InputController : MonoBehaviour
    {
        private EditorInputs _inputs;
        public EditorInputs.EditorActions EditorActions => _inputs.Editor;

        private void Awake()
        {
            _inputs = new EditorInputs();
        }
    }
}