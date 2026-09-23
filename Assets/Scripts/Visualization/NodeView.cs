using UnityEngine;

namespace PerturbationRestoration.Visualization
{
    public class NodeView : MonoBehaviour
    {
        public int NodeId { get; private set; }

        public void Initialize(int nodeId)
        {
            NodeId = nodeId;
        }
    }
}
