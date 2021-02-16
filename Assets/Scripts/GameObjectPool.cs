using UnityEngine;

namespace Game //since it applies to Asteroids and Bullets.. or should it be Game.GameObjects or something ? 
{
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField]
        int startingSize;

        [SerializeField]
        GameObject objectToPool;

        void Start()
        {
            for (int i = 0; i < startingSize; i++)
            {
                AddNewObject();
            }
        }

        public GameObject GetObject()
        {
            foreach (Transform obj in gameObject.transform)
            {
                if (!obj.gameObject.activeInHierarchy)
                {
                    return obj.gameObject;
                }
            }
            return AddNewObject();
        }

        private GameObject AddNewObject()
        {
            GameObject obj = Instantiate(objectToPool, gameObject.transform);
            obj.SetActive(false);
            return obj;
        }

        public void DisableAllObjects()
        {
            foreach (Transform obj in transform)
            {
                obj.gameObject.SetActive(false);
            }
        }

    }
}