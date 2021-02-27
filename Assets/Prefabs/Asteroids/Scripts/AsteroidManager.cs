using Game.ScreenSpace;
using UnityEngine;

namespace Game.Asteroids
{
    public class AsteroidManager : MonoBehaviour
    {
        [SerializeField]
        GameLogic gameLogic;

        [SerializeField]
        AsteroidFactory asteroidFactory;

        private void Start()
        {
            ResetAsteroidCount();

            gameLogic.OnNewGameStarted += OnNewGameStarted;
            gameLogic.OnNewRoundStarted += OnNewRoundStarted;
            Asteroid.OnAsteroidDestroyed += OnAsteroidDestroyed;
        }

        int asteroidCountAtStart;

        private void OnNewGameStarted(float timeStartedAt)
        {
            ResetAsteroidCount();
            EnableLargeAsteroids(asteroidCountAtStart);
        }

        private void OnNewRoundStarted(float timeStartedAt)
        {
            asteroidCountAtStart += GameConfig.asteroidCountToAddPerNewRound;
            EnableLargeAsteroids(asteroidCountAtStart);
        }

        void ResetAsteroidCount()
        {
            asteroidCountAtStart = GameConfig.asteroidCountAtNewGame;
        }


        //Object Pool specific... 
        [SerializeField]
        GameObjectPool asteroidObjectPool; //Shouldn't be here... since it's already used in the AsteroidFactory... 
        // maybe have a separate class responsible for the pool ?


        private void EnableLargeAsteroids(int ammount)
        {
            DisableAllAsteroids();
            for (int i = 0; i < ammount; i++)
            {
                GameObject asteroid = asteroidFactory.GetLargeAsteroid();
                asteroid.transform.position = ScreenSpaceUtility.GetRandomLocationInWorldSpace();
                asteroid.SetActive(true);
                AddForceInRandomDirectionRelativeToSize(asteroid.GetComponent<Rigidbody2D>());
            }
        }

        
        void DisableAllAsteroids()
        {
            asteroidObjectPool.DisableAllObjects();
        }

        ////Object Pool specific... 

        //Should this even be here? seems VERY Asteroid SPECIFIC and not MANAGER.... no ? 
        // looks a lot like something that SHOULD be on an Asteroid itself ?
        private void OnAsteroidDestroyed(object sender, OnAsteroidDestroyedArgs args)
        {
            if (args.size == GameConfig.LargeAsteroidSize)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject mediumAsteroid = asteroidFactory.GetMediumAsteroid();
                    EnableGameobjectAtLocation(mediumAsteroid, args.location);
                    AddForceInRandomDirectionRelativeToSize(mediumAsteroid.GetComponent<Rigidbody2D>());
                }
            }
            if (args.size == GameConfig.MediumAsteroidSize)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject smallAsteroid = asteroidFactory.GetSmallAsteroid();
                    EnableGameobjectAtLocation(smallAsteroid, args.location);
                    AddForceInRandomDirectionRelativeToSize(smallAsteroid.GetComponent<Rigidbody2D>());
                }
            }
        }

        //Also NOT Asteroid specific... can be / basically is used for Player / UFO / Asteroids ... 

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

        //technically NOT Asteroid specific, but there's also NOTHING else that would use this.. 
        public void AddForceInRandomDirectionRelativeToSize(Rigidbody2D rigidbody)
        {
            float randomForceX = Random.Range(-1f, 1f);
            float randomForceY = Random.Range(-1f, 1f);
            rigidbody.AddForce(new Vector2(randomForceX, randomForceY) / rigidbody.gameObject.transform.localScale, ForceMode2D.Impulse);
        }

        void OnDestroy()
        {
            Asteroid.OnAsteroidDestroyed -= OnAsteroidDestroyed;
        }

    }
}