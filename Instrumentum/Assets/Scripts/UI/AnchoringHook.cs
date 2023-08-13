using System;
using UnityEngine;

namespace Game.UI
{
    [Serializable]
    public struct AnchorPoint
    {
        public AnchorPosition Position;
        public Transform Point;
    }

    public enum AnchorPosition
    {
        TopLeft, Top, TopRight,
        MiddleLeft, Middle, MiddleRight,
        BottomLeft, Bottom, BottomRight
    }

    public class AnchoringHook : MonoBehaviour
    {
        [SerializeField] private Camera _uiCamera;
        [Range(0.5f, 5f)] [SerializeField] private float _uiScale;
        [SerializeField] private AnchorPoint[] _anchorPoints;
        private float _currentCameraSize;

        private readonly Vector2[] _anchorNormals =
        {
            new(-1, 1),
            new(0, 1),
            new(1, 1),

            new(-1, 0),
            new(0, 0),
            new(1, 0),

            new(-1, -1),
            new(0, -1),
            new(1, -1),
        };


        private void Awake()
        {
            _currentCameraSize = _uiCamera.orthographicSize;
            UpdateAnchorPositions();
        }

        private void Update()
        {
            var cameraRect = _uiCamera.pixelRect;

            if (Math.Abs(_currentCameraSize - cameraRect.size.magnitude) >= 1f)
            {
                UpdateAnchorPositions();
            }
        }

        private void UpdateAnchorPositions()
        {
            var cameraRect = _uiCamera.pixelRect;

            _currentCameraSize = cameraRect.size.magnitude;
            _uiCamera.orthographicSize = cameraRect.size.y;

            foreach (var anchorPoint in _anchorPoints)
            {
                anchorPoint.Point.localPosition = 
                    cameraRect.size * _anchorNormals[(int)anchorPoint.Position] + (Vector2)_uiCamera.transform.position;
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var anchorPoint in _anchorPoints)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(anchorPoint.Point.position, 20f);
            }
        }

        private void OnValidate()
        {
            foreach (var anchorPoint in _anchorPoints)
            {
                if (anchorPoint.Point)
                {
                    anchorPoint.Point.localScale = new Vector2(_uiScale, _uiScale);
                }
            }
        }
    }
}
