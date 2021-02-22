using Game.Ships.Player;
using System;
using System.Collections;
using UnityEngine;


namespace Game.Ships.Ufo
{
    public class UfoManager : MonoBehaviour
    {

        [SerializeField]
        GameObject largeUfo; //should be kinda SINGLETON... maybe have UfoManager INSTANTIATE 'em ?

        [SerializeField]
        GameObject smallUfo;

        [SerializeField]
        GameLogic gameLogic;

        [SerializeField]
        PlayerShipController playerShip;

        void Start()
        {
            largeUfo.SetActive(false);
            smallUfo.SetActive(false);

            currentlyUsedUfo = GetRandomUfo();

            gameLogic.OnNewRoundStarted += OnNewRoundStarted;
            UfoProjectile.OnProjectileDestroyed += OnUfoProjectileDestroyed;
            largeUfo.GetComponent<Ufo>().OnShipDestroyed += gameLogic.OnUfoDestroyedOrDisabled; //NOT really great to Subscribe a METHOD in anotehr Class from outside of it, no ??? 
            smallUfo.GetComponent<Ufo>().OnShipDestroyed += gameLogic.OnUfoDestroyedOrDisabled;
        }

        private GameObject GetRandomUfo()
        {
            bool returnSmallUfo = UnityEngine.Random.value > 0.5f;
            if (returnSmallUfo)
            {
                return smallUfo;
            }
            return largeUfo;
        }

        private void Update()
        {
            if (ShouldSpawnUfo()) //checking every frame, which isn't nice. Perhaps a coroutine or some other way of calling it periodically, rather than every frame... 
            {
                EnableUfoBasedOnCurrentScore(gameLogic.Score); //problem - if GameLogic is changed, this will break... not STAND ALONE enougugh.. 
            }

        }

        GameObject currentlyUsedUfo;

        float lastTimeUfoSpawned = 0;

        float roundStartedAtTime;
        private void OnNewRoundStarted(float startedAt)
        {
            roundStartedAtTime = startedAt;
        }

        bool ShouldSpawnUfo()
        {
            return gameLogic.CurrentGameState == GameState.Playing && //public from GameLogic

                roundStartedAtTime + GameConfig.SpawnEnemyPeriod < Time.time && // GameLogic ONLY... 

                lastTimeUfoSpawned + GameConfig.SpawnEnemyPeriod < Time.time && //LOCAL to UFO Manager
                !IsUfoCurrentlyActive(); // LOCAL 2 UFO Manager
        }

        void EnableUfoBasedOnCurrentScore(int score)
        {
            if (score > GameConfig.ScoreForSmalUFOsOnly)
            {
                currentlyUsedUfo = smallUfo;

            }
            else
            {
                currentlyUsedUfo = GetRandomUfo();
            }
            lastTimeUfoSpawned = Time.time;
            currentlyUsedUfo.gameObject.SetActive(true);
        }

        IEnumerator DisableEnemyShipAfterTimer() //isn't this the same as "DisableAfter... CS" class ??? 
        {
            yield return new WaitForSeconds(1.5f);
            DisableCurrentlyUsedUfo();
        }

        public event Action OnUfoDisabled;
        private void DisableCurrentlyUsedUfo()
        {
            currentlyUsedUfo.SetActive(false);
            OnUfoDisabled?.Invoke();
        }

        public bool IsUfoCurrentlyActive()
        {
            return currentlyUsedUfo.activeSelf;
        }

        private void OnDestroy()
        {
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnUfoDisabled);
        }

        private void OnUfoProjectileDestroyed(object sender, OnProjectileDestroyedArgs args)
        {
            if (args.tagOfObjectHit.Equals(playerShip.gameObject.tag))
            {
                StartCoroutine(DisableEnemyShipAfterTimer());
            }
        }
    }

}