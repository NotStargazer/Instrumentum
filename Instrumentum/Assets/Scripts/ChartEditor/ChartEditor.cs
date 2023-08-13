using System;
using UnityEngine;

namespace Instrumentum.ChartEditor
{
    public class ChartEditor : MonoBehaviour
    {
        private Chart _currentChart;
        
        private void Awake()
        {
            ChartLoader.Load();
        }
    }
}