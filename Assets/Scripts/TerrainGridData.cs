using UnityEngine;
using System.Collections.Generic;

namespace TerrainGenerator
{
    public class TerrainGridData : MonoBehaviour
    {
        public List<Terrain> terrain;

        public int gridSideCount = 1;

        public bool Validate()
        {
            if (gridSideCount < 1)
                return false;
            else
                return true;
        }
    }
}
