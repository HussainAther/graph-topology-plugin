using System.Collections.Generic;

namespace PerturbationRestoration.Core
{
    public class GraphNode
    {
        public int Id { get; }

        private readonly List<int> _neighbors = new();

        public IReadOnlyList<int> Neighbors => _neighbors;

        public GraphNode(int id)
        {
            Id = id;
        }

        public void AddNeighbor(int neighborId)
        {
            if (neighborId == Id)
                return;

            if (!_neighbors.Contains(neighborId))
                _neighbors.Add(neighborId);
        }
    }
}
