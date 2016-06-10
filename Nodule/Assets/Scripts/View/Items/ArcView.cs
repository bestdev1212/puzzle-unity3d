﻿using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Game;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    public class ArcView : MonoBehaviour
    {
        private ScaleScript _arcScale;
        private Colorizer _colorizer;

        private Transform _parent;

        public Arc Arc { get; private set; }

        public void Init(Arc arc, Transform parent, bool inStartIsland)
        {
            _arcScale = GetComponent<ScaleScript>();
            _colorizer = GetComponent<Colorizer>();

            Arc = arc;
            _parent = parent;

            _arcScale.SetArc(arc);

            if (!inStartIsland) { _colorizer.Darken(); }
        }

        public void ResetParent()
        {
            transform.parent = _parent;
        }

    }
}
