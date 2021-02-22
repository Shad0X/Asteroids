using UnityEngine;

namespace Game.Asteroids {
    public class AsteroidFactory : MonoBehaviour 
        //Is it a factory if it's not really creating anything, but just a "fasade" for a Object Pool while adjusting settings on Asteroids before returning them ?? 
    {

        //could use a Factor pattern here? Asteroid Factory = build Asteroid based on input parameter ? 
        //  ... QUESTION - is it a FACTORY if it DOESNT actually create the Objects.. only pools for them and does a bit of SETUP... ?? 

        [SerializeField]
        GameObjectPool asteroidObjectPool;

        public GameObject GetLargeAsteroid()
        {
            GameObject asteroid = asteroidObjectPool.GetObject();
            asteroid.tag = GameConfig.LargeAsteroidTag;
            asteroid.transform.localScale = GameConfig.LargeAsteroidSize;
            return asteroid;
        }

        public GameObject GetMediumAsteroid()
        {
            GameObject asteroid = asteroidObjectPool.GetObject();
            asteroid.tag = GameConfig.MediumAsteroidTag;
            asteroid.transform.localScale = GameConfig.MediumAsteroidSize;
            return asteroid;
        }

        public GameObject GetSmallAsteroid()
        {
            GameObject asteroid = asteroidObjectPool.GetObject();
            asteroid.tag = GameConfig.SmallAsteroidTag;
            asteroid.transform.localScale = GameConfig.SmallAsteroidSize;
            return asteroid;
        }

    }
}