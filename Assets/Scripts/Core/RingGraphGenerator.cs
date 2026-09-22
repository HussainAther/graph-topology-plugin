using System;

namespace PerturbationRestoration.Core
{
    public static class RingGraphGenerator
    {
        public static Graph Generate(int nodeCount)
        {
            if (nodeCount < 3)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(nodeCount),
                    "A ring graph requires at least 3 nodes."
                );
            }

            Graph graph = new Graph(nodeCount);

            for (int i = 0; i < nodeCount; i++)
            {
                int next = (i + 1) % nodeCount;
                graph.AddUndirectedEdge(i, next);
            }

            return graph;
        }
    }
}
