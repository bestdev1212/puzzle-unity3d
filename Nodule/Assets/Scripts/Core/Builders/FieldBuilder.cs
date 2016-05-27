﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Game;
using Assets.Scripts.Core.Items;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Core.Builders
{
    /// <summary>
    /// A FieldBuilder takes nodes as input, and produces corresponding 
    /// Fields between nodes. A field will be created whenever two nodes have a direct
    /// route to each other on the grid. A field will be removed whenever a node 
    /// connected to it is removed.
    /// </summary>
    public class FieldBuilder {

        // Keep a simple collection of fields
        private readonly ICollection<Field> _fields = new HashSet<Field>();
        
        // Maps points to occupying fields
        private IDictionary<Point, Field> _occupiedFields = new Dictionary<Point, Field>();

        public void BuildFields(Node node, IDictionary<Point, Node> nodeMap)
        {
            // Find and add fields in all directions
            Directions.All.ForEach(direction => AddField(node, direction, nodeMap));
        }

        public void DestroyFields(Node node)
        {
            node.Fields.Values.ToList()
                .ForEach(field => RemoveField(field));
        }

        private void AddField(Node node, Direction direction, IDictionary<Point, Node> nodeMap)
        {
            // Find the nearest node in the given direction
            var nearest = NearestNode(node, direction, nodeMap);
            if (nearest == null) return;

            var length = node.GetDistance(nearest);

            // If an existing field is found, remove it
            Field existingField;
            if (node.Fields.TryGetValue(direction, out existingField))
                RemoveField(existingField);

            // Ensure that fields always point either up or right
            Pair.Swap(ref node, ref nearest, direction.IsDownLeft());

            // Create a new field, and add it to the list
            var field = new Field(length, node, nearest);
            _fields.Add(field);
            AddOccupied(field);
        }

        /// <summary>
        /// Finds the nearest node to another in a given direction
        /// </summary>
        private static Node NearestNode(Node start, Direction direction, IDictionary<Point, Node> nodeMap)
        {
            var dirPoint = direction.ToPoint();

            for (var i = 1; i < Field.MaxLength; i++)
            {
                var next = Grid.NodeAt(start.Position + i * dirPoint, nodeMap);
                if (next != null) { return next; }
            }

            return null;
        }

        private void RemoveField(Field field)
        {
            if (field == null) return;

            field.DisconnectNodes();
            _fields.Remove(field);
            RemoveOccupied(field);
            RemoveOverlap(field);
        }

        private void AddOccupied(Field field)
        {
            var dirPoint = field.Direction.ToPoint();

            for (var i = 1; i < field.Length; i++)
            {
                var next = field.Position + i * dirPoint;

                // Keep track of overlap
                Field overlap;
                if (_occupiedFields.TryGetValue(next, out overlap))
                {
                    AddOverlap(field, overlap);
                    continue;
                }

                // Add all points occupied by the field to the map
                _occupiedFields.Add(next, field);
            }
        }

        private void RemoveOccupied(Field field)
        {
            // Create a new dictionary with the field positions removed
            _occupiedFields = _occupiedFields
                .Where(occ => !occ.Value.Position.Equals(field.Position))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private static void AddOverlap(Field f1, Field f2)
        {
            f1.Overlap.Add(f2);
            f2.Overlap.Add(f1);
        }

        private static void RemoveOverlap(Field field)
        {
            foreach (var overlap in field.Overlap)
                overlap.Overlap.Remove(field);
        }
    }
}
