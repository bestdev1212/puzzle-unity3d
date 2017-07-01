using System;
using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Items;
using Assets.Scripts.View.Control;
using UnityEngine;

namespace Assets.Scripts.View.Items
{
    /// <summary>
    /// A NodeView represents the view for an node in the gameboard. It is responsible
    /// for visualizing nodes that connect and rotate arcs.
    /// </summary>
    public class NodeView : MonoBehaviour
    {
        public Transform Rotor;

        public Color NodeColor { get { return GameDef.Get.NodeColor; } }
        public Color NodeFinalColor { get { return GameDef.Get.NodeFinalColor; } }

        private ScaleScript _nodeScale;
        private Colorizer _colorizer;
        private Transit _transit;

        public Node Node { get; private set; }

        public Point Position
        {
            get { return Node.Position; }
        }

        public Field GetField(Direction dir)
        {
            return Node.Fields[dir];
        }

        void Awake()
        {
            _nodeScale = GetComponent<ScaleScript>();
            _colorizer = GetComponentInChildren<Colorizer>();
            _transit = GetComponent<Transit>();
        }

        public void Init(Node node, bool inStartIsland, int delay)
        {
            Node = node;

            // TODO: replace nodescale
            _nodeScale.SetNode(node);

            _colorizer.PrimaryColor = node.Final ? NodeFinalColor : NodeColor;

            if (!inStartIsland && !node.Final) {
                _colorizer.Darken(0f);
            }
        }

        public void WaveIn(int delay)
        {
            _transit.WaveIn(delay);
        }

        public void WaveOut(int delay)
        {
            _transit.WaveOut(delay);
        }

        public void Rotate(Direction dir, Action onComplete)
        {
            _transit.Rotate90(dir, onComplete);
        }

        public void Shake(Direction dir, Action onComplete)
        {
            _transit.Shake(dir, onComplete);
        }

        public void WinAnimation()
        {
            _transit.RotateFast();
        }
        
        public void Highlight(bool enable)
        {
            if (Node.Final) {
                return;
            }

            if (enable) {
                _colorizer.Highlight();
            } else {
                _colorizer.Darken();
            }
        }
    }
}
