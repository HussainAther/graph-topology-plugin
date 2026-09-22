using System;
using System.Collections.Generic;

namespace PerturbationRestoration.Core
{
    public class Graph
    {
        private readonly List<GraphNode> _nodes = new();

        public IReadOnlyList<GraphNode> Nodes => _nodes;

        public int NodeCount => _nodes.Count;

        public Graph(int nodeCount)
        {
            if (nodeCount <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(nodeCount),
                    "Graph must contain at least one node."
                );

            for (int i = 0; i < nodeCount; i++)
                _nodes.Add(new GraphNode(i));
        }

        public GraphNode GetNode(int id)
        {
            ValidateNodeId(id);
            return _nodes[id];
        }

        public void AddUndirectedEdge(int a, int b)
        {
            ValidateNodeId(a);
            ValidateNodeId(b);

            if (a == b)
                return;

            _nodes[a].AddNeighbor(b);
            _nodes[b].AddNeighbor(a);
        }

        public bool AreConnected(int a, int b)
        {
            ValidateNodeId(a);
            ValidateNodeId(b);

            foreach (int neighbor in _nodes[a].Neighbors)
            {
                if (neighbor == b)
                    return true;
            }

            return false;
        }

        private void ValidateNodeId(int id)
        {
            if (id < 0 || id >= NodeCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    $"Node ID {id} is outside graph bounds."
                );
            }
        }
    }
}
