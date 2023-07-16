using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

namespace Builder
{
    public class Line : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        private List<Vector2> _points;

        public void UpdateLine(Vector2 position)
        {
            if (_points == null)
            {
                _points = new List<Vector2>();
                SetPoint(position);
                return;
            }

            if (Vector3.Distance(_points.Last(), position) > 0.1f)
            {
                SetPoint(position);
            }
        }

        private void SetPoint(Vector2 point)
        {
            _points.Add(point);

            lineRenderer.positionCount = _points.Count;
            lineRenderer.SetPosition(_points.Count - 1, point);
        }
    }
}