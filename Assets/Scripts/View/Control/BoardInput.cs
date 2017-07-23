using System.Collections.Generic;
using Core.Data;
using UnityEngine;
using View.Game;
using View.Items;

namespace View.Control
{
    /// <summary>
    /// Handles all initial inputs to the game board (i.e., screen swipes and taps)
    /// and converts them to an action.
    /// </summary>
    public class BoardInput : MonoBehaviour
    {
        private BoardAction _boardAction;
        private PuzzleScale _puzzleScale;

        private IDictionary<Point, NodeView> _nodeMap;

        public float MinSwipeDistanceCm => GameDef.Get.MinSwipeDistanceCm;

        private void Awake()
        {
            _puzzleScale = GetComponent<PuzzleScale>();
            _boardAction = GetComponent<BoardAction>();
        }

        private void Start()
        {
            // Add event handlers for swiping the screen
            var swipeRecognizers = new[] {
                new TKSwipeRecognizer(MinSwipeDistanceCm) {
                    minimumNumberOfTouches = 1,
                    maximumNumberOfTouches = 10,
                    timeToSwipe = 1f
                }
            };

            foreach (var swipe in swipeRecognizers) {
                swipe.gestureRecognizedEvent += OnSwipe;
                TouchKit.addGestureRecognizer(swipe);
            }

            // Add an event handler for tapping the screen
            var tapRecognizer = new TKTapRecognizer();
            tapRecognizer.gestureRecognizedEvent += OnTap;
            TouchKit.addGestureRecognizer(tapRecognizer);

            var touch = new TKAnyTouchRecognizer(new TKRect());
            TouchKit.addGestureRecognizer(touch);
            touch.onEnteredEvent += r => { Debug.Log("Touch begin"); };
        }

        public void Init(IDictionary<Point, NodeView> nodeMap)
        {
            _nodeMap = nodeMap;
        }

        /// <summary>
        /// Called every time the screen is tapped
        /// </summary>
        private void OnTap(TKTapRecognizer recognizer)
        {
//            // Find the nearest node to the tap
//            var field = GetNearestField(recognizer);
//
//            if (field == null) {
//                return;
//            }

//            _boardAction.Play(field);
        }

        /// <summary>
        /// Called every time the screen is swiped
        /// </summary>
        private void OnSwipe(TKSwipeRecognizer recognizer)
        {
            // Find the nearest node to the swipe (within 1 grid unit), and the swipe direction
            var swipeDirection = recognizer.completedSwipeDirection.ToDirection();
            var node = GetNearestNode(recognizer.startPoint) ?? GetNearestNode(recognizer.endPoint);

            // If the swipe is invalid, don't do anything
            if (node == null || swipeDirection == Direction.None) {
                return;
            }

            // Otherwise, play the move
            _boardAction.Play(node, swipeDirection);
        }

        /// <summary>
        /// Finds the nearest node to the gesture
        /// </summary>
        private NodeView GetNearestNode(Vector2 screenPoint)
        {
            // TODO: make this more robust

            // Obtain the gesture positions
            var startTouch = Camera.main.ScreenToWorldPoint(screenPoint);
            var scaledPos = (Vector2) (startTouch - transform.position);

            // Remove any scaling, and round the position to the nearest integer
            var pos = scaledPos/_puzzleScale.Scaling;
            var point = Point.Round(pos);

            // Retrieve the node, if it exists
            NodeView node;
            _nodeMap.TryGetValue(point, out node);
            return node;
        }

//        private FieldView GetNearestField(TKTapRecognizer recognizer)
//        {
//            var pos = Camera.main.ScreenToWorldPoint(recognizer.startTouchLocation()) / _puzzleScale.Scaling;
//            return null;
//        }
    }
}
