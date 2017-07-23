using System.Collections.Generic;
using Core.Data;
using Core.Game;
using UnityEngine;
using View.Data;
using View.Items;

namespace View.Game
{
    public class PuzzleSpawner : MonoBehaviour
    {
        public NodeView NodeScript;
        public ArcView ArcScript;
        public FieldView FieldScript;

        private GameBoard _gameBoard;

        private LatticeView _lattice;
        private PuzzleScale _puzzleScale;

        public IDictionary<Point, NodeView> NodeMap { get; } = new Dictionary<Point, NodeView>();

        public FieldViewMap FieldMap { get; } = new FieldViewMap();

        public ArcViewMap ArcMap { get; } = new ArcViewMap();

        public bool FinishedSpawn { get; private set; }

        public int LevelCount => Levels.LevelCount;

        private void Awake()
        {
            _lattice = GetComponentInChildren<LatticeView>();
            _puzzleScale = GetComponent<PuzzleScale>();
        }

        public Puzzle SpawnBoard(int level)
        {
            // Create the game board model
            var newGameBoard = Levels.BuildLevel(level);

            if (newGameBoard != null) {
                _gameBoard = newGameBoard;
            } else {
                Debug.LogError($"The game board for level {level} is in an invalid format");
            }

            // Instantiate all the necessary components to view the board
            FinishedSpawn = false;
            InstantiateNodes();
            InstantiateFields();
            InstantiateArcs();

            StartAnimations();

            // Wrap a puzzle around the gameboard and return it
            return new Puzzle(_gameBoard);
        }

        public void DestroyBoard()
        {
            if (_gameBoard == null) return;

            // Destroy all objects in the game board
            var i = 0;
            foreach (var node in NodeMap.Values) {
                node.WaveOut(i++);
            }

            foreach (Transform child in transform) {
                // Only destroy board pieces
                if (child.gameObject.GetComponent<NodeView>() ??
                    child.gameObject.GetComponent<ArcView>() != null) {
                        Destroy(child.gameObject, 1.5f);
                }
            }

            NodeMap.Clear();
            ArcMap.Clear();
            FieldMap.Clear();

            _gameBoard = null;
        }

        private void InstantiateNodes()
        {
            var i = 0;
            foreach (var node in _gameBoard.Nodes) {
                var nodeView = Instantiate(NodeScript);

                // Set the node's parent as this puzzle
                nodeView.transform.SetParent(transform);
                nodeView.Init(node, _gameBoard.StartIsland.Contains(node), i);
                nodeView.name = "Node " + i++;
                NodeMap.Add(node.Position, nodeView);
            }
        }

        private void InstantiateFields()
        {
            var i = 0;
            foreach (var field in _gameBoard.Fields) {
                var fieldView = Instantiate(FieldScript);

                // Find the node at the field's position and set it as a parent of this field
                fieldView.transform.SetParent(NodeMap[field.Position].transform);
                fieldView.Init(field, NodeMap[field.Position], NodeMap[field.ConnectedPosition]);
                fieldView.name = "Field " + i++;

                // Keep track of the field in grid space
                // Since fields are undirected, we should add the opposite direction as well
                FieldMap.Add(field.Position, field.Direction, fieldView);
                FieldMap.Add(field.ConnectedPosition, field.Direction.Opposite(), fieldView);
            }
        }

        private void InstantiateArcs()
        {
            var i = 0;
            foreach (var arc in _gameBoard.Arcs) {
                var arcView = Instantiate(ArcScript);

                // Find the node at the arc's position and set it as a perent of this arc
                var parent = NodeMap[arc.Position].transform;
                arcView.transform.SetParent(parent);
                arcView.Init(arc, parent, _gameBoard.StartIsland.Contains(arc.ParentNode));
                arcView.name = "Arc " + i++;

                // Keep track of the arc in grid space
                // Since arcs are undirected, we should add the opposite direction as well
                ArcMap.Add(arc.Position, arc.Direction, arcView);
                ArcMap.Add(arc.ConnectedPosition, arc.Direction.Opposite(), arcView);
            }
        }

        private void StartAnimations()
        {
            _lattice.Init(_gameBoard.Size.Y + 1, _gameBoard.Size.X + 1, _puzzleScale.Scaling);
            
            var i = 0;
            foreach (var nodeView in NodeMap.Values) {
                if (i < NodeMap.Values.Count - 1) {
                    nodeView.WaveIn(i++);
                } else {
                    // On completion of the last node, the puzzle has finished spawning
                    nodeView.WaveIn(i++, () => {
                        FinishedSpawn = true;
                    });
                }
            }
        }
    }
}
