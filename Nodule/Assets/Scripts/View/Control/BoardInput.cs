using System.Collections.Generic;
using Assets.Scripts.Core.Data;
using Assets.Scripts.View.Game;
using Assets.Scripts.View.Items;
using UnityEngine;

namespace Assets.Scripts.View.Control
{
    /// <summary>
    /// Handles all initial inputs to the game board (i.e., screen swipes and taps)
    /// and converts them to an action.
    /// </summary>
    public class BoardInput : MonoBehaviour
    {
        public float MinSwipeDistanceCm = 1.5f;
        public float MedSwipeDistanceCm = 3f;
        public float MaxSwipeDistanceCm = 4.5f;

        private BoardAction _boardAction;
        private PuzzleScale _puzzleScale;

        private IDictionary<Point, NodeView> _nodeMap;

        void Awake()
        {
            _puzzleScale = GetComponent<PuzzleScale>();
            _boardAction = GetComponent<BoardAction>();
        }

        void Start()
        {
            // Add an event handler for swiping the screen
            var minSwipeRecognizer = new TKSwipeRecognizer(MinSwipeDistanceCm);
            var medSwipeRecognizer = new TKSwipeRecognizer(MedSwipeDistanceCm);
            var maxSwipeRecognizer = new TKSwipeRecognizer(MaxSwipeDistanceCm);

            minSwipeRecognizer.gestureRecognizedEvent += OnSwipe;
            medSwipeRecognizer.gestureRecognizedEvent += OnSwipe;
            maxSwipeRecognizer.gestureRecognizedEvent += OnSwipe;

            TouchKit.addGestureRecognizer(minSwipeRecognizer);
            TouchKit.addGestureRecognizer(medSwipeRecognizer);
            TouchKit.addGestureRecognizer(maxSwipeRecognizer);

            // Add an event handler for tapping the screen
            var tapRecognizer = new TKTapRecognizer();
            tapRecognizer.gestureRecognizedEvent += OnTap;
            TouchKit.addGestureRecognizer(tapRecognizer);
        }

        public void Init(IDictionary<Point, NodeView> nodeMap)
        {
            _nodeMap = nodeMap;
        }

        /// <summary>
        /// Called every time the screen is tapped
        /// </summary>
        private void OnTap(TKTapRecognizer recognizer) {}

        /// <summary>
        /// Called every time the screen is swiped
        /// </summary>
        private void OnSwipe(TKSwipeRecognizer recognizer)
        {
            // Find the nearest node to the swipe (within 1 grid unit), and the swipe direction
            var node = GetNearestNode(recognizer);
            var swipeDirection = recognizer.completedSwipeDirection.ToDirection();

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
        private NodeView GetNearestNode(TKSwipeRecognizer recognizer)
        {
            // Obtain the gesture positions
            var startTouch = Camera.main.ScreenToWorldPoint(recognizer.startPoint);
            var endTouch = Camera.main.ScreenToWorldPoint(recognizer.endPoint);

            // Find the midpoint
            var mid = startTouch + (endTouch - startTouch)/2f;
            var scaledPos = (Vector2) (mid - transform.position);

            // Remove any scaling, and round the position to the nearest integer
            var pos = (scaledPos)/_puzzleScale.Scaling;
            var point = Point.Round(pos);

            // Retrieve the node, if it exists
            NodeView node;
            _nodeMap.TryGetValue(point, out node);
            return node;
        }
    }
}
