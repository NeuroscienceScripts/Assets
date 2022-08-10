using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Classes
{
    public static class NodeExtension
    {
        private static readonly float distanceThreshold = 1 - 0.45f; // To change threshold, change the second number (0.5 max)

        public static GridLocation CurrentNode(Vector3 position)
        {
            int xPos = Mathf.Abs(position.x - (int)position.x) >= distanceThreshold ? ( (int) position.x + Mathf.RoundToInt((position.x - (int)position.x)) ) : (int) position.x;
            int zPos = Mathf.Abs(position.z - (int)position.z) >= distanceThreshold ? ((int)position.z + Mathf.RoundToInt((position.z - (int)position.z))) : (int)position.z;

            // if (obstacles.Contains(GridLocation(xPos, yPos){ return 2nd closest}

            return new GridLocation(xPos, zPos);
        }
    }
}
