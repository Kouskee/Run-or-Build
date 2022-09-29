using System.Collections;
using System.Collections.Generic;
using Data.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Mirror.Managers
{
    public class MapNetManager : NetworkBehaviour
    {
        [SerializeField] private DataPathSetting _path;
        [SerializeField] private Tilemap _tileMap;
        [SerializeField] private TileBase _tile;
        [SerializeField] private MapData _data;
        [SerializeField] private List<Vector3IntS> availablePlaces;
        
        [ServerCallback]
        public void SerializeData()
        {
            availablePlaces = new List<Vector3IntS>();
            
            for (int x = _tileMap.cellBounds.xMin; x < _tileMap.cellBounds.xMax; x++)
            {
                for (int y = _tileMap.cellBounds.yMin; y < _tileMap.cellBounds.yMax; y++)
                {
                    var localPlace = (new Vector3Int(x, y, 0));
                    var place = _tileMap.CellToWorld(localPlace);
                    if (_tileMap.HasTile(localPlace))
                        availablePlaces.Add(new Vector3IntS(localPlace));
                }
            }
            _data = new MapData()
            {
                map = availablePlaces
            };
            BinarySerializer.Serializer(_path.DataPath, _data);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if(!isServer)
                StartCoroutine(GenerateMapFromData());
        }

        private IEnumerator GenerateMapFromData()
        {
            var data = BinarySerializer.Deserializer<MapData>(_path.DataPath);
            _data = data;
            for (var i = 0; i < data.map.Count; i++)
            {
                _tileMap.SetTile(data.map[i].AsVector3Int, _tile);
                yield return null;
            }
        }
    }
}