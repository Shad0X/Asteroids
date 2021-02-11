using UnityEngine;

namespace Game //since it applies to Asteroids and Bullets.. or should it be Game.GameObjects or something ? 
{
    public class GameObjectPool : MonoBehaviour
    {
        [SerializeField]
        int size;

        [SerializeField]
        GameObject objectToPool;

        void Start()
        {
            for (int i = 0; i < size; i++)
            {
                AddNewObjectToPool();
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
            return AddNewObjectToPool();
        }

        private GameObject AddNewObjectToPool()
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