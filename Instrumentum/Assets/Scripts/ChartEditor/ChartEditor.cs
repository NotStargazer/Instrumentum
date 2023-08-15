using System;
using Instrumentum.UI;
using UnityEngine;

namespace Instrumentum.ChartEditor
{
    public class ChartEditor : SingletonBehaviour<ChartEditor>
    {
        private Chart _currentChart;

        public override void Instantiate()
        {
            _currentChart = ChartLoader.Load();
        }
    }
}