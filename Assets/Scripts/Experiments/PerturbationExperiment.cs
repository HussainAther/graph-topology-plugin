using System;
using System.Collections.Generic;
using PerturbationRestoration.Core;
using PerturbationRestoration.Dynamics;

namespace PerturbationRestoration.Experiments
{
    public class PerturbationExperiment
    {
        public SimulationEngine Control { get; private set; }
        public SimulationEngine Perturbed { get; private set; }

        public PerturbationMetrics Metrics { get; private set; }

        public int Timestep => Control.Timestep;

        private readonly Graph _graph;
        private readonly SimulationState _initialState;
        private readonly INodeUpdateRule _updateRule;

        private bool _perturbationApplied;

        public PerturbationExperiment(
            Graph graph,
            SimulationState initialState,
            INodeUpdateRule updateRule
        )
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));

            _initialState = initialState?.Clone()
                ?? throw new ArgumentNullException(nameof(initialState));

            _updateRule = updateRule
                ?? throw new ArgumentNullException(nameof(updateRule));

            Reset();
        }

        public void Reset()
        {
            Control = new SimulationEngine(
                _graph,
                _initialState,
                _updateRule
            );

            Perturbed = new SimulationEngine(
                _graph,
                _initialState,
                _updateRule
            );

            Metrics = new PerturbationMetrics();

            _perturbationApplied = false;

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
