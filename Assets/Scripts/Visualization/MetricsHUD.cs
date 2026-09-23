using UnityEngine;
using PerturbationRestoration.Experiments;

namespace PerturbationRestoration.Visualization
{
    public class MetricsHUD : MonoBehaviour
    {
        public PerturbationExperiment Experiment { get; set; }

        public int SelectedNode { get; set; } = -1;

        public bool IsRunning { get; set; }

        private GUIStyle _style;

        private void OnGUI()
        {
            if (Experiment == null)
                return;

            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label);
                _style.fontSize = 20;
                _style.normal.textColor = Color.white;
            }

            PerturbationMetrics metrics = Experiment.Metrics;

            string selected =
                SelectedNode >= 0
                    ? SelectedNode.ToString()
                    : "none";

            string restoration =
                metrics.RestorationTime.HasValue
                    ? metrics.RestorationTime.Value.ToString()
                    : "-";

            string text =
                $"Perturbation / Restoration Network Lab\n\n" +
                $"timestep: {Experiment.Timestep}\n" +
                $"damage: {metrics.CurrentDamage}\n" +
                $"peak damage: {metrics.PeakDamage}\n" +
                $"integrated damage: {metrics.IntegratedDamage}\n" +
                $"coalesced: {metrics.HasCoalesced}\n" +
                $"restoration time: {restoration}\n" +
                $"selected node: {selected}\n" +
                $"running: {IsRunning}\n\n" +
                $"Click node = select\n" +
                $"P = perturb\n" +
                $"Right Arrow = step\n" +
                $"Space = play/pause\n" +
                $"R = reset";

            GUI.Label(
                new Rect(20, 20, 420, 400),
                text,
                _style
            );
        }
    }
}
