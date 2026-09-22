using PerturbationRestoration.Core;

namespace PerturbationRestoration.Dynamics
{
    public class MajorityRule : INodeUpdateRule
    {
        public int ComputeNextState(
            int nodeId,
            Graph graph,
            SimulationState currentState
        )
        {
            GraphNode node = graph.GetNode(nodeId);

            int ones = currentState.GetState(nodeId);
            int zeros = ones == 1 ? 0 : 1;

            foreach (int neighborId in node.Neighbors)
            {
                if (currentState.GetState(neighborId) == 1)
                    ones++;
                else
                    zeros++;
            }

            if (ones > zeros)
                return 1;

            if (zeros > ones)
                return 0;

            // Deterministic tie behavior:
            // preserve current state.
            return currentState.GetState(nodeId);
        }
    }
}
