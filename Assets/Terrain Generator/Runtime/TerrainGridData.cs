using UnityEngine;
using System.Collections.Generic;

namespace TerrainGenerator
{
    public class TerrainGridData : MonoBehaviour
    {
        public List<Terrain> terrains;

        [Range(1, 64)]
        public int gridSideCount = 1;
    }
}
