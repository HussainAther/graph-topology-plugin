namespace PerturbationRestoration.Experiments
{
    public class PerturbationMetrics
    {
        public int CurrentDamage { get; set; }
        public int PeakDamage { get; set; }
        public int IntegratedDamage { get; set; }
        public bool HasCoalesced { get; set; }
        public int? RestorationTime { get; set; }
    }
}
