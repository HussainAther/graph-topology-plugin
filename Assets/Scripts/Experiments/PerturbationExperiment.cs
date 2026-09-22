using System;
using System.Collections.Generic;
using PerturbationRestoration.Core;
using PerturbationRestoration.Dynamics;

namespace PerturbationRestoration.Experiments
{
    public class PerturbationExperiment
    {
        public SimulationEngine Control { get; }
        public SimulationEngine Perturbed { get; }

        public PerturbationMetrics Metrics { get; } = new();

        public int Timestep => Control.Timestep;

        private bool _perturbationApplied;

        public PerturbationExperiment(
            Graph graph,
            SimulationState initialState,
            INodeUpdateRule updateRule
        )
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            if (initialState == null)
                throw new ArgumentNullException(nameof(initialState));

            if (updateRule == null)
                throw new ArgumentNullException(nameof(updateRule));

            Control = new SimulationEngine(
                graph,
                initialState,
                updateRule
            );

            Perturbed = new SimulationEngine(
                graph,
                initialState,
                updateRule
            );

            UpdateMetrics();
        }

        public void ApplyNodeFlip(int nodeId)
        {
            Perturbed.FlipNode(nodeId);
            _perturbationApplied = true;

            UpdateMetrics();
        }

        public void Step()
        {
            Control.Step();
            Perturbed.Step();

            UpdateMetrics();
        }

        public IReadOnlyList<int> GetMismatchNodes()
        {
            List<int> mismatches = new();

            for (int nodeId = 0; nodeId < Control.Graph.NodeCount; nodeId++)
            {
                if (
                    Control.State.GetState(nodeId) !=
                    Perturbed.State.GetState(nodeId)
                )
                {
                    mismatches.Add(nodeId);
                }
            }

            return mismatches;
        }

        public bool StatesMatchExactly()
        {
            for (int nodeId = 0; nodeId < Control.Graph.NodeCount; nodeId++)
            {
                if (
                    Control.State.GetState(nodeId) !=
                    Perturbed.State.GetState(nodeId)
                )
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateMetrics()
        {
            int damage = GetMismatchNodes().Count;

            Metrics.CurrentDamage = damage;

            if (damage > Metrics.PeakDamage)
                Metrics.PeakDamage = damage;

            if (_perturbationApplied)
                Metrics.IntegratedDamage += damage;

            if (
                _perturbationApplied &&
                !Metrics.HasCoalesced &&
                damage == 0
            )
            {
                Metrics.HasCoalesced = true;
                Metrics.RestorationTime = Timestep;
            }
        }
    }
}
