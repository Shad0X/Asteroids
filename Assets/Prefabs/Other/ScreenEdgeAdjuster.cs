using UnityEngine;

namespace Game.ScreenSpace //idk if it's the best place, but it's a start.. 
{
    public class ScreenEdgeAdjuster : MonoBehaviour
    {
        Camera mainCamera;
        const float edgeSize = 1f;

        void Start()
        {
            mainCamera = Camera.main;

            float screenWidthInWorldUnits = GetScreenWidthInWorldUnits();
            float screenHeightInWorldUnits = GetScreenHeightInWorldUnits();

            Vector2 horizontalEdgeSize = new Vector2(edgeSize, screenHeightInWorldUnits);
            Vector2 verticalEdgeSize = new Vector2(screenWidthInWorldUnits, edgeSize);

            float halfWidth = screenWidthInWorldUnits / 2;
            float halfHeight = screenHeightInWorldUnits / 2;

            float xOffSet = horizontalEdgeSize.x / 2;
            float yOffSet = verticalEdgeSize.y / 2;

            float xDistanceToScreenCenter = halfWidth + xOffSet;
            float yDistanceToScreenCenter = halfHeight + yOffSet;


            int counter = 0;
            foreach (Transform screenEdge in transform)
            {
                switch (counter)
                {
                    case 0:
                        screenEdge.gameObject.GetComponent<BoxCollider2D>().size = horizontalEdgeSize;
                        screenEdge.position = new Vector3(-xDistanceToScreenCenter, 0, 0);
                        break;
                    case 1:
                        screenEdge.gameObject.GetComponent<BoxCollider2D>().size = horizontalEdgeSize;
                        screenEdge.position = new Vector3(xDistanceToScreenCenter, 0, 0);
                        break;
                    case 2:
                        screenEdge.gameObject.GetComponent<BoxCollider2D>().size = verticalEdgeSize;
                        screenEdge.transform.position = new Vector3(0, yDistanceToScreenCenter, 0);
                        break;
                    case 3:
                        screenEdge.gameObject.GetComponent<BoxCollider2D>().size = verticalEdgeSize;
                        screenEdge.transform.position = new Vector3(0, -yDistanceToScreenCenter, 0);
                        break;
                }
                counter += 1;
            }
        }

        float GetScreenWidthInWorldUnits()
        {
            Vector3 min = mainCamera.ScreenToWorldPoint(Vector3.zero);
            Vector3 max = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
            return max.x - min.x;
        }

        float GetScreenHeightInWorldUnits()
        {
            Vector3 min = mainCamera.ScreenToWorldPoint(Vector3.zero);
            Vector3 max = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
            return max.y - min.y;
        }
    }
}