using UnityEngine;

namespace Game.ScreenSpace
{ 
    public class ScreenSpaceUtility
    {
        private const float distanceToScreenEdge = 10f;
        public static Vector3 GetRandomLocationInWorldSpace()
        {
            float randomXlocation = Random.Range(distanceToScreenEdge, Screen.width - distanceToScreenEdge);
            float randomYlocation = Random.Range(distanceToScreenEdge, Screen.height - distanceToScreenEdge);

            return ConvertScreenSpaceToWorldSpace(randomXlocation, randomYlocation);
        }

        public static Vector3 ConvertScreenSpaceToWorldSpace(float x, float y)
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(x, y, 0));
        }

    }
}