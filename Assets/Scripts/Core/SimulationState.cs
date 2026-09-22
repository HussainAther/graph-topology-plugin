using System;

namespace PerturbationRestoration.Core
{
    public class SimulationState
    {
        private readonly int[] _states;

        public int Count => _states.Length;

        public SimulationState(int nodeCount)
        {
            if (nodeCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(nodeCount));

            _states = new int[nodeCount];
        }

        public SimulationState(int[] states)
        {
            if (states == null)
                throw new ArgumentNullException(nameof(states));

            _states = (int[])states.Clone();
            ValidateBinaryStates();
        }

        public int GetState(int nodeId)
        {
            return _states[nodeId];
        }

        public void SetState(int nodeId, int value)
        {
            ValidateBinaryValue(value);
            _states[nodeId] = value;
        }

        public void Flip(int nodeId)
        {
            _states[nodeId] = _states[nodeId] == 0 ? 1 : 0;
        }

        public SimulationState Clone()
        {
            return new SimulationState(_states);
        }

        public int[] ToArray()
        {
            return (int[])_states.Clone();
        }

        private void ValidateBinaryStates()
        {
            foreach (int state in _states)
                ValidateBinaryValue(state);
        }

        private static void ValidateBinaryValue(int value)
        {
            if (value != 0 && value != 1)
            {
                throw new ArgumentException(
                    $"Binary node state must be 0 or 1, received {value}."
                );
            }
        }
    }
}
