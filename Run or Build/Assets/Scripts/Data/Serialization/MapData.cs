using System.Collections.Generic;
using UnityEngine;

namespace Data.Serialization
{
    [System.Serializable]
    public struct MapData
    {
        public List<Vector3IntS> map;
    }
    
    [System.Serializable]
    public struct Vector3IntS
    {
        private int _x, _y, _z;
        
        public int x { get => _x; set => _x = value; }
        public int y { get => _y; set => _y = value; }
        public int z { get => _z; set => _z = value; }

        public Vector3Int AsVector3Int => new Vector3Int(_x, _y, _z);

        public Vector3IntS(Vector3Int vector3)
        {
            _x = vector3.x;
            _y = vector3.y;
            _z = vector3.z;
        }
       
        public Vector3IntS(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
    }
}


