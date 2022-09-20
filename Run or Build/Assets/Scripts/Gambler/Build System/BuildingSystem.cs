using Mirror;
using UnityEngine;

namespace Gambler.Build_System
{
    public class BuildingSystem : NetworkBehaviour
    {
        [SerializeField] private int _maxEnergy;
        
        private EnergyGambler _energy;
        private Building _building;
        private Camera _camera;
        private BoxCollider2D _colliderBuilding;
        
        private BuildingFabric _buildingFabric;
        private TileFabric _tileFabric;
        
        private string _buildingName;
    
        private readonly Collider2D[] _resultOverlap = new Collider2D[5];

        public void Init(BuildingFabric buildingFabric, TileFabric tileFabric)
        {
            _tileFabric = tileFabric;
            _buildingFabric = buildingFabric;
        }
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            _energy = new EnergyGambler(_maxEnergy);
        }

        public void StartPlacingBuilding(BuildingConfig config)
        {
            if (!hasAuthority) return;
            if(_building != null) Destroy(_building.gameObject);
            
            if (!_energy.CanBuild(config.Cost)) return;
            _energy.StealEnergy();

            _buildingName = config.Name;
            _building = Instantiate(config.Building);
            _building.TryGetComponent(out _colliderBuilding);
        }

        private void Update()
        {
            if (!hasAuthority) return;
            if (_building == null) return;
        
            var groundPlane = new Plane(Vector3.back, Vector3.zero);
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
        
            if (!groundPlane.Raycast(ray, out var position)) return;
        
            var worldPosition = ray.GetPoint(position);
        
            var x = worldPosition.x;
            var y = worldPosition.y;
        
            var available = !IsPlaceTaken();
        
            _building.transform.position = new Vector3(x, y, 0);
            _building.SetTransparent(available);
        
            if (available && Input.GetMouseButtonDown(0))
            {
                PlaceFlyingBuilding(_building.transform.position, _buildingName);
                Destroy(_building.gameObject);
                _building = null;
            }
        }

        private bool IsPlaceTaken()
        {
            var overlap = Physics2D.OverlapBoxNonAlloc(_colliderBuilding.transform.position, _colliderBuilding.size, 0, _resultOverlap);
            return overlap > 1;
        }
    
        [Command]
        private void PlaceFlyingBuilding(Vector3 position, string buildingName)
        {
            var buildingGo = _buildingFabric.GetBuilding(buildingName).Building;
        
            var go = Instantiate(buildingGo, position, Quaternion.identity).gameObject;
        
            NetworkServer.Spawn(go);
            var building = go.GetComponent<Building>();
            building.SetNormalServer();
            ReportToClient(building);
        }

        [ClientRpc]
        private void ReportToClient(Building building)
        {
            building.SetNormalServer();
            if(hasAuthority) SetActiveOnClient(building);
        }

        [ClientCallback]
        private void SetActiveOnClient(Building building) =>
            building.SetNormal();
    }
}