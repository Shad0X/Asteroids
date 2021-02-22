using UnityEngine;

namespace Game //probably NOT the best place, but gotta place it somewhere..  ScreenSPACE maybe, since it's only used for updating stuff WITHIN ScreenSpace... ?
{
    public class ObjectLocationChanger : MonoBehaviour
    {
        Camera mainCamera;

        int leftScreenEdgeLocation;
        int rightScreenEdgeLocation;
        int topScreenEdgeLocation;
        int bottomScreenEdgeLocation;

        private void Start()
        {
            mainCamera = Camera.main;

            leftScreenEdgeLocation = 0;
            rightScreenEdgeLocation = Screen.width;
            bottomScreenEdgeLocation = 0;
            topScreenEdgeLocation = Screen.height;
        }

        [SerializeField]
        float extraDistanceFromScreenEdge = 1f;

        Vector2 locationInScreenSpace;
        private void OnTriggerStay2D(Collider2D collision)
        {
            locationInScreenSpace = mainCamera.WorldToScreenPoint(collision.transform.position);

            if (locationInScreenSpace.x < leftScreenEdgeLocation)
            {
                collision.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width - extraDistanceFromScreenEdge, locationInScreenSpace.y, 0));
            }

            if (locationInScreenSpace.x > rightScreenEdgeLocation)
            {
                collision.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(extraDistanceFromScreenEdge, locationInScreenSpace.y, 0));
            }

            if (locationInScreenSpace.y < bottomScreenEdgeLocation)
            {
                collision.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(locationInScreenSpace.x, Screen.height - extraDistanceFromScreenEdge, 0));
            }

            if (locationInScreenSpace.y > topScreenEdgeLocation)
            {
                collision.transform.position = mainCamera.ScreenToWorldPoint(new Vector3(locationInScreenSpace.x, extraDistanceFromScreenEdge, 0));
            }
        }

    }
}