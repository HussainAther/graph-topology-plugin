using NUnit.Framework;
using PerturbationRestoration.Core;
using PerturbationRestoration.Dynamics;
using PerturbationRestoration.Experiments;

namespace PerturbationRestoration.Tests
{
    public class PerturbationExperimentTests
    {
        [Test]
        public void ControlAndPerturbedMatchBeforePerturbation()
        {
            Graph graph = RingGraphGenerator.Generate(6);
            SimulationState initial = new SimulationState(
                new[] { 0, 1, 0, 1, 0, 1 }
            );

            PerturbationExperiment experiment =
                new PerturbationExperiment(
                    graph,
                    initial,
                    new MajorityRule()
                );

            Assert.IsTrue(experiment.StatesMatchExactly());
            Assert.AreEqual(0, experiment.Metrics.CurrentDamage);
        }

        [Test]
        public void FlippingOneNodeCreatesOneMismatch()
        {
            Graph graph = RingGraphGenerator.Generate(6);
            SimulationState initial = new SimulationState(
                new[] { 0, 1, 0, 1, 0, 1 }
            );

            PerturbationExperiment experiment =
                new PerturbationExperiment(
                    graph,
                    initial,
                    new MajorityRule()
                );

            experiment.ApplyNodeFlip(2);

            Assert.IsFalse(experiment.StatesMatchExactly());
            Assert.AreEqual(1, experiment.Metrics.CurrentDamage);
            Assert.AreEqual(1, experiment.Metrics.PeakDamage);
        }

        [Test]
        public void DamageEqualsMismatchCount()
        {
            Graph graph = RingGraphGenerator.Generate(6);
            SimulationState initial = new SimulationState(
                new[] { 0, 1, 0, 1, 0, 1 }
            );

            PerturbationExperiment experiment =
                new PerturbationExperiment(
                    graph,
                    initial,
                    new MajorityRule()
                );

            experiment.ApplyNodeFlip(0);
            experiment.ApplyNodeFlip(3);

            Assert.AreEqual(
                experiment.GetMismatchNodes().Count,
                experiment.Metrics.CurrentDamage
            );

            Assert.AreEqual(2, experiment.Metrics.CurrentDamage);
        }

        [Test]
        public void SimulationUsesSynchronousUpdates()
        {
            Graph graph = RingGraphGenerator.Generate(3);

            SimulationState initial = new SimulationState(
                new[] { 1, 0, 0 }
            );

            SimulationEngine engine =
                new SimulationEngine(
                    graph,
                    initial,
                    new MajorityRule()
                );

            engine.Step();

            int[] result = engine.State.ToArray();

            CollectionAssert.AreEqual(
                new[] { 0, 0, 0 },
                result
            );
        }

        [Test]
        public void SameInitialStateProducesSameTrajectory()
        {
            Graph graph = RingGraphGenerator.Generate(8);

            SimulationState initial = new SimulationState(
                new[] { 0, 1, 1, 0, 1, 0, 0, 1 }
            );

            SimulationEngine a =
                new SimulationEngine(
                    graph,
                    initial,
                    new MajorityRule()
                );

            SimulationEngine b =
                new SimulationEngine(
                    graph,
                    initial,
                    new MajorityRule()
                );

            for (int i = 0; i < 10; i++)
            {
                a.Step();
                b.Step();

                CollectionAssert.AreEqual(
                    a.State.ToArray(),
                    b.State.ToArray()
                );
            }
        }

        [Test]
        public void CoalescenceRequiresExactEquality()
        {
            Graph graph = RingGraphGenerator.Generate(5);

            SimulationState initial = new SimulationState(
                new[] { 0, 0, 0, 0, 0 }
            );

            PerturbationExperiment experiment =
                new PerturbationExperiment(
                    graph,
                    initial,
                    new MajorityRule()
                );

            experiment.ApplyNodeFlip(2);

            Assert.IsFalse(experiment.Metrics.HasCoalesced);

            experiment.Step();

            Assert.IsTrue(experiment.StatesMatchExactly());
            Assert.IsTrue(experiment.Metrics.HasCoalesced);
            Assert.AreEqual(1, experiment.Metrics.RestorationTime);
        }
    }
}
