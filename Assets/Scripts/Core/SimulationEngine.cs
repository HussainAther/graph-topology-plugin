using System;
using PerturbationRestoration.Dynamics;

namespace PerturbationRestoration.Core
{
    public class SimulationEngine
    {
        public Graph Graph { get; }

        public SimulationState State { get; private set; }

        public int Timestep { get; private set; }

        private readonly INodeUpdateRule _updateRule;

        public SimulationEngine(
            Graph graph,
            SimulationState initialState,
            INodeUpdateRule updateRule
        )
        {
            Graph = graph ?? throw new ArgumentNullException(nameof(graph));

            State = initialState?.Clone()
                ?? throw new ArgumentNullException(nameof(initialState));

            _updateRule = updateRule
                ?? throw new ArgumentNullException(nameof(updateRule));

            if (State.Count != Graph.NodeCount)
            {
                throw new ArgumentException(
                    "Simulation state size must match graph node count."
                );
            }

            Timestep = 0;
        }

        public void Step()
        {
            SimulationState nextState =
                new SimulationState(Graph.NodeCount);

            for (int nodeId = 0; nodeId < Graph.NodeCount; nodeId++)
            {
                int nextValue = _updateRule.ComputeNextState(
                    nodeId,
                    Graph,
                    State
                );

                nextState.SetState(nodeId, nextValue);
            }

            State = nextState;
            Timestep++;
        }

        public void FlipNode(int nodeId)
        {
            State.Flip(nodeId);
        }
    }
}
