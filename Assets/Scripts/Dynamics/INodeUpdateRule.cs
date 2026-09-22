using PerturbationRestoration.Core;

namespace PerturbationRestoration.Dynamics
{
    public interface INodeUpdateRule
    {
        int ComputeNextState(
            int nodeId,
            Graph graph,
            SimulationState currentState
        );
    }
}
