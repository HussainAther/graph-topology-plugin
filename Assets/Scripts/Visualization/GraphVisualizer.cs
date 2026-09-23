using System.Collections.Generic;
using UnityEngine;
using PerturbationRestoration.Core;
using PerturbationRestoration.Dynamics;
using PerturbationRestoration.Experiments;

namespace PerturbationRestoration.Visualization
{
    public class GraphVisualizer : MonoBehaviour
    {
        [Header("Graph")]
        [SerializeField] private int nodeCount = 8;
        [SerializeField] private float radius = 4f;

        [Header("Simulation")]
        [SerializeField] private float stepInterval = 0.75f;

        [Header("Rendering")]
        [SerializeField] private float nodeScale = 0.6f;
        [SerializeField] private float edgeWidth = 0.06f;

        private PerturbationExperiment _experiment;

        private readonly List<GameObject> _nodeObjects = new();
        private readonly List<LineRenderer> _edgeObjects = new();

        private Material _stateZeroMaterial;
        private Material _stateOneMaterial;
        private Material _mismatchMaterial;
        private Material _selectedMaterial;
        private Material _edgeMaterial;

        private float _timer;
        private bool _running;
        private MetricsHUD _hud;
        private int _selectedNode = -1;

        private void Start()
        {
            CreateMaterials();
            CreateExperiment();
            BuildGraphView();
            RefreshView();
            _hud = gameObject.AddComponent<MetricsHUD>();
            _hud.Experiment = _experiment;
            _hud.SelectedNode = _selectedNode;
            _hud.IsRunning = _running;

            Debug.Log(
                "Controls: click node = select, P = perturb selected, " +
                "RightArrow = step, Space = play/pause"
            );
        }

        private void Update()
        {
            HandleMouseSelection();
            HandleInput();

            if (!_running)
                return;

            _timer += Time.deltaTime;

            if (_timer >= stepInterval)
            {
                _timer -= stepInterval;
                StepSimulation();
            }
        }

        private void CreateExperiment()
        {
            Graph graph = RingGraphGenerator.Generate(nodeCount);

            int[] values = new int[nodeCount];

            for (int i = 0; i < nodeCount; i++)
                values[i] = i % 2;

            SimulationState initialState = new SimulationState(values);

            _experiment = new PerturbationExperiment(
                graph,
                initialState,
                new MajorityRule()
            );
        }

        private void BuildGraphView()
        {
            Graph graph = _experiment.Control.Graph;

            for (int i = 0; i < graph.NodeCount; i++)
            {
                float angle = (Mathf.PI * 2f * i) / graph.NodeCount;

                Vector3 position = new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0f
                );

                GameObject node =
                    GameObject.CreatePrimitive(PrimitiveType.Sphere);

                node.name = $"Node_{i}";
                node.transform.SetParent(transform);
                node.transform.localPosition = position;
                node.transform.localScale = Vector3.one * nodeScale;

                NodeView nodeView = node.AddComponent<NodeView>();
                nodeView.Initialize(i);

                _nodeObjects.Add(node);
            }

            for (int i = 0; i < graph.NodeCount; i++)
            {
                int next = (i + 1) % graph.NodeCount;

                GameObject edgeObject =
                    new GameObject($"Edge_{i}_{next}");

                edgeObject.transform.SetParent(transform);

                LineRenderer line =
                    edgeObject.AddComponent<LineRenderer>();

                line.positionCount = 2;
                line.startWidth = edgeWidth;
                line.endWidth = edgeWidth;
                line.material = _edgeMaterial;
                line.useWorldSpace = true;

                line.SetPosition(
                    0,
                    _nodeObjects[i].transform.position
                );

                line.SetPosition(
                    1,
                    _nodeObjects[next].transform.position
                );

                _edgeObjects.Add(line);
            }
        }

        private void CreateMaterials()
        {
            Shader shader = FindShader();

            _stateZeroMaterial = new Material(shader);
            _stateZeroMaterial.color =
                new Color(0.18f, 0.32f, 0.75f);

            _stateOneMaterial = new Material(shader);
            _stateOneMaterial.color =
                new Color(0.15f, 0.75f, 0.45f);

            _mismatchMaterial = new Material(shader);
            _mismatchMaterial.color =
                new Color(0.95f, 0.25f, 0.18f);

            _selectedMaterial = new Material(shader);
            _selectedMaterial.color =
                new Color(1.0f, 0.75f, 0.15f);

            _edgeMaterial = new Material(shader);
            _edgeMaterial.color =
                new Color(0.45f, 0.45f, 0.45f);
        }

        private static Shader FindShader()
        {
            Shader shader = Shader.Find("Standard");

            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            if (shader == null)
                shader = Shader.Find("Universal Render Pipeline/Lit");

            return shader;
        }

        private void HandleMouseSelection()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            Camera camera = Camera.main;

            if (camera == null)
                return;

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit))
                return;

            NodeView nodeView = hit.collider.GetComponent<NodeView>();

            if (nodeView == null)
                return;

            _selectedNode = nodeView.NodeId;
            _hud.SelectedNode = _selectedNode;

            Debug.Log($"Selected node {_selectedNode}");

            RefreshView();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                 _running = !_running;
                _hud.IsRunning = _running;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
                StepSimulation();

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (_selectedNode < 0)
                {
                    Debug.LogWarning(
                        "Select a node before applying a perturbation."
                    );

                    return;
                }

                _experiment.ApplyNodeFlip(_selectedNode);

                RefreshView();

                Debug.Log(
                    $"Perturbed node {_selectedNode} at " +
                    $"t={_experiment.Timestep}, " +
                    $"damage={_experiment.Metrics.CurrentDamage}"
                );
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                _experiment.Reset();
                _running = false;
                _timer = 0f;
                _selectedNode = -1;

                _hud.Experiment = _experiment;
                _hud.SelectedNode = -1;
                _hud.IsRunning = false;

                 RefreshView();

                 Debug.Log("Experiment reset.");
            }
        }

        private void StepSimulation()
        {
            _experiment.Step();

            RefreshView();

            Debug.Log(
                $"t={_experiment.Timestep}, " +
                $"damage={_experiment.Metrics.CurrentDamage}, " +
                $"peak={_experiment.Metrics.PeakDamage}, " +
                $"integrated={_experiment.Metrics.IntegratedDamage}, " +
                $"coalesced={_experiment.Metrics.HasCoalesced}"
            );
        }

        private void RefreshView()
        {
            HashSet<int> mismatchNodes =
                new HashSet<int>(
                    _experiment.GetMismatchNodes()
                );

            for (
                int nodeId = 0;
                nodeId < _nodeObjects.Count;
                nodeId++
            )
            {
                Renderer renderer =
                    _nodeObjects[nodeId]
                        .GetComponent<Renderer>();

                if (nodeId == _selectedNode)
                {
                    renderer.material = _selectedMaterial;
                    continue;
                }

                if (mismatchNodes.Contains(nodeId))
                {
                    renderer.material = _mismatchMaterial;
                    continue;
                }

                int state =
                    _experiment.Control.State
                        .GetState(nodeId);

                renderer.material =
                    state == 0
                        ? _stateZeroMaterial
                        : _stateOneMaterial;
            }
        }
    }
}
