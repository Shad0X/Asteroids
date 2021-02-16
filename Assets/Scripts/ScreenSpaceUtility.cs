using UnityEngine;

namespace Game.ScreenSpace
{ 
    public class ScreenSpaceUtility //needs better name
    {
        private const float distanceFromScreenEdge = 10f; //not the best name... 
        public static Vector3 GetRandomLocationInWorldSpace()
        {
            float randomXlocation = Random.Range(distanceFromScreenEdge, Screen.width - distanceFromScreenEdge);
            float randomYlocation = Random.Range(distanceFromScreenEdge, Screen.height - distanceFromScreenEdge);

            return ConvertScreenSpaceToWorldSpace(randomXlocation, randomYlocation);
        }

        public static Vector3 ConvertScreenSpaceToWorldSpace(float x, float y)
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0));
        }

    }
}