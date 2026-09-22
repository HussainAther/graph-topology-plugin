using UnityEngine;
using PerturbationRestoration.Core;
using PerturbationRestoration.Dynamics;
using PerturbationRestoration.Experiments;

namespace PerturbationRestoration.Debugging
{
    public class PerturbationDebugRunner : MonoBehaviour
    {
        private void Start()
        {
            Graph graph = RingGraphGenerator.Generate(8);

            SimulationState initialState = new SimulationState(
                new[] { 0, 1, 1, 0, 1, 0, 0, 1 }
            );

            MajorityRule rule = new MajorityRule();

            PerturbationExperiment experiment =
                new PerturbationExperiment(
                    graph,
                    initialState,
                    rule
                );

            Debug.Log($"t={experiment.Timestep} damage={experiment.Metrics.CurrentDamage}");

            experiment.ApplyNodeFlip(3);

            Debug.Log(
                $"After perturbation: t={experiment.Timestep} " +
                $"damage={experiment.Metrics.CurrentDamage}"
            );

            for (int i = 0; i < 10; i++)
            {
                experiment.Step();

                string mismatches =
                    string.Join(",", experiment.GetMismatchNodes());

                Debug.Log(
                    $"t={experiment.Timestep} " +
                    $"damage={experiment.Metrics.CurrentDamage} " +
                    $"peak={experiment.Metrics.PeakDamage} " +
                    $"integrated={experiment.Metrics.IntegratedDamage} " +
                    $"coalesced={experiment.Metrics.HasCoalesced} " +
                    $"mismatch=[{mismatches}]"
                );
            }
        }
    }
}
