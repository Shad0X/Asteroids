﻿using Game.Asteroids;
using Game.ScreenSpace;
using UnityEngine;

namespace Game
{
    public class AsteroidManager : MonoBehaviour
    {
        private void Start()
        {
            Asteroid.OnAsteroidDestroyed += OnAsteroidDestroyed;
        }

        public void EnableLargeAsteroids(int ammount)
        {
            DisableAllAsteroids();
            for (int i = 0; i < ammount; i++)
            {
                GameObject asteroid = GetLargeAsteroid();
                asteroid.transform.position = ScreenSpaceUtility.GetRandomLocationInWorldSpace();
                asteroid.SetActive(true);
                AddForceInRandomDirectionRelativeToSize(asteroid.GetComponent<Rigidbody2D>());
            }
        }

        void DisableAllAsteroids()
        {
            asteroidObjectPool.DisableAllObjects();
        }


        private void OnAsteroidDestroyed(object sender, OnAsteroidDestroyedArgs args)
        {
            if (args.size == GameConfig.LargeAsteroidSize)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject mediumAsteroid = GetMediumAsteroid();
                    EnableGameobjectAtLocation(mediumAsteroid, args.location);
                    AddForceInRandomDirectionRelativeToSize(mediumAsteroid.GetComponent<Rigidbody2D>());
                }
            }
            if (args.size == GameConfig.MediumAsteroidSize)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject smallAsteroid = GetSmallAsteroid();
                    EnableGameobjectAtLocation(smallAsteroid, args.location);
                    AddForceInRandomDirectionRelativeToSize(smallAsteroid.GetComponent<Rigidbody2D>());
                }
            }
        }

        private void EnableGameobjectAtLocation(GameObject obj, Vector3 location)
        {
            obj.transform.position = location;
            obj.SetActive(true);
        }

        public bool AnyAsteroidsActive()
        {
            foreach (Transform child in asteroidObjectPool.transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddForceInRandomDirectionRelativeToSize(Rigidbody2D rigidbody)
        {
            float randomForceX = Random.Range(-1f, 1f);
            float randomForceY = Random.Range(-1f, 1f);
            rigidbody.AddForce(new Vector2(randomForceX, randomForceY) / rigidbody.gameObject.transform.localScale, ForceMode2D.Impulse);
        }

        [SerializeField]
        GameObjectPool asteroidObjectPool;

        GameObject GetLargeAsteroid()
        {
            GameObject asteroid = asteroidObjectPool.GetObject();
            asteroid.tag = GameConfig.LargeAsteroidTag;
            asteroid.transform.localScale = GameConfig.LargeAsteroidSize;
            return asteroid;
        }

        GameObject GetMediumAsteroid()
        {
            GameObject asteroid = asteroidObjectPool.GetObject();
            asteroid.tag = GameConfig.MediumAsteroidTag;
            asteroid.transform.localScale = GameConfig.MediumAsteroidSize;
            return asteroid;
        }

        GameObject GetSmallAsteroid()
        {
            GameObject asteroid = asteroidObjectPool.GetObject();
            asteroid.tag = GameConfig.SmallAsteroidTag;
            asteroid.transform.localScale = GameConfig.SmallAsteroidSize;
            return asteroid;
        }

        void OnDestroy()
        {
            Asteroid.OnAsteroidDestroyed -= OnAsteroidDestroyed;
        }

    }
}