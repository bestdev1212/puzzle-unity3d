using System;
using System.Collections.Generic;
using Core.Data;
using Core.Items;
using UnityEngine;
using View.Control;

namespace View.Items
{
    /// <summary>
    /// An ArcView represents the view for an arc in the gameboard. It is responsible
    /// for visualizing arcs rotating and sliding across the gameboard.
    /// </summary>
    public class ArcView : MonoBehaviour
    {
        public Color ArcColor => GameDef.Get.ArcColor;
        public float ArcMoveTime => GameDef.Get.ArcMoveTime;
        public LeanTweenType ArcMoveEase => GameDef.Get.ArcMoveEase;

        private ScaleScript _arcScale;
        private Colorizer _colorizer;
        private GameAudio _gameAudio;

        public GameObject MarkerPrefab;

        public Transform Parent { private get; set; }

        public Arc Arc { get; private set; }
        
        // TODO: make configurable
        private static float MoveVolume => 0.3f;

        private void Awake()
        {
            _arcScale = GetComponent<ScaleScript>();
            _colorizer = GetComponent<Colorizer>();
            _gameAudio = GameObject.FindGameObjectWithTag("GameAudio").GetComponent<GameAudio>();
        }

        public void Init(Arc arc, Transform parent, bool inStartIsland)
        {
            Arc = arc;
            Parent = parent;

            _arcScale.SetArc(arc);

            _colorizer.PrimaryColor = ArcColor;
            
//            CreateMarkers();

            if (!inStartIsland) {
                _colorizer.Darken(0f);
            }
        }

        public void ResetParent()
        {
            transform.parent = Parent;
        }

        public void Highlight(bool enable)
        {
            if (enable) {
                _colorizer.Highlight();
            } else {
                _colorizer.Darken();
            }
        }

        public void PushSound()
        {
            switch (Arc.Length) {
                case 1:
                    _gameAudio.Play(GameClip.MovePushHigh, volume: MoveVolume);
                    break;
                case 2:
                    _gameAudio.Play(GameClip.MovePushMid, volume: MoveVolume);
                    break;
                default:
                    _gameAudio.Play(GameClip.MovePushLow, volume: MoveVolume);
                    break;
            }
        }

        public void PullSound()
        {
            switch (Arc.Length) {
                case 1:
                    _gameAudio.Play(GameClip.MovePullHigh, volume: MoveVolume);
                    break;
                case 2:
                    _gameAudio.Play(GameClip.MovePullMid, volume: MoveVolume);
                    break;
                default:
                    _gameAudio.Play(GameClip.MovePullLow, volume: MoveVolume);
                    break;
            }
        }
        
        public void MoveTo(NodeView nodeView, Action onComplete)
        {
            // Move to the same node
            if (nodeView.transform.Equals(transform.parent.parent)) {
                onComplete();
                return;
            }

            transform.parent = nodeView.transform;

            _gameAudio.Play(GameClip.ArcMove);
            
            LeanTween.move(gameObject, nodeView.transform, ArcMoveTime)
                .setEase(ArcMoveEase)
                .setOnComplete(onComplete);
            LeanTween.moveLocalZ(gameObject, -_arcScale.Length, ArcMoveTime)
                .setEase(ArcMoveEase);
        }

        /// <summary>
        /// Moves this arc along a path of nodes specified in the given node list
        /// </summary>
        public void MoveTo(List<NodeView> nodeViews, Action onComplete)
        {
            var list = new LinkedList<NodeView>(nodeViews);
            MoveNext(list.First, onComplete);
        }

        /// <summary>
        /// Recursively applies arc movement to a node based on a list of nodes
        /// </summary>
        private void MoveNext(LinkedListNode<NodeView> nodeViews, Action onComplete)
        {
            if (nodeViews == null) {
                return;
            }

            var head = nodeViews.Value;
            var tail = nodeViews.Next;

            if (tail == null) {
                MoveTo(head, onComplete);
                return;
            }

            MoveTo(head, () => MoveNext(tail, onComplete));
        }

        private void CreateMarkers()
        {
            for (var i = 1; i < Arc.Length; i++) {
                var marker = Instantiate(MarkerPrefab);
                marker.name = "Marker " + i;

                marker.transform.parent = transform.parent;
                marker.transform.localRotation = Arc.Direction.Rotation();
                
                marker.transform.parent = transform;

                var fractionPos = ((float) i / Arc.Length * 2f - 1f) / 10f;
                marker.transform.localPosition = Vector3.right * fractionPos;
            }
            
        }
    }
}
