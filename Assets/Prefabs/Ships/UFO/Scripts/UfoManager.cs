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

            gameLogic.OnNewGameStarted += OnNewGameStarted;
            gameLogic.OnNewRoundStarted += OnNewRoundStarted;
            UfoProjectile.OnProjectileDestroyed += OnUfoProjectileDestroyed;
            largeUfo.GetComponent<Ufo>().OnShipDestroyed += gameLogic.OnUfoDestroyedOrDisabled; //NOT really great to Subscribe a METHOD in anotehr Class from outside of it, no ??? 
            smallUfo.GetComponent<Ufo>().OnShipDestroyed += gameLogic.OnUfoDestroyedOrDisabled; // GameLogic SHOULD BE responsible for handling WHAT it Subscribes to.. NOT let other Classes do it without GameLogic agreeing to it... 
            //make Method in GameLogic PRIVATE + [SerializedFields] for Ufos in it so it can Subscribe to their DEATH States ? 
            // still not ideal... 
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

        [SerializeField]
        ScoreKeeper scoreKeeper;

        private void Update()
        {
            if (ShouldSpawnUfo()) //checking every frame, which isn't nice. Perhaps a coroutine or some other way of calling it periodically, rather than every frame... 
            {
                EnableUfoBasedOnScore(scoreKeeper.Score); //problem - if GameLogic is changed, this will break... not STAND ALONE enougugh.. 
            }

        }

        GameObject currentlyUsedUfo;

        float lastTimeUfoSpawned = 0;

        float roundStartedAtTime;
        private void OnNewRoundStarted(float startedAt)
        {
            roundStartedAtTime = startedAt;
        }

        private void OnNewGameStarted(float startedAt)
        {
            roundStartedAtTime = startedAt;
        }

        bool ShouldSpawnUfo() // or SHOULD the GameState keep track of this and just TRIGGER The Spawning ?? via Action Event or something.. ?? 
        {
            return gameLogic.CurrentGameState == GameState.Playing && //public from GameLogic

                roundStartedAtTime + GameConfig.SpawnEnemyPeriod < Time.time && // GameLogic ONLY... 

                lastTimeUfoSpawned + GameConfig.SpawnEnemyPeriod < Time.time && //LOCAL to UFO Manager
                !IsAnyUfoActive(); // LOCAL 2 UFO Manager
        }

        void EnableUfoBasedOnScore(int currentScore)
        {
            if (currentScore > GameConfig.ScoreForSmalUFOsOnly)
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

        IEnumerator DisableShipAfterTimer() //isn't this the same as "DisableAfter... CS" class ??? 
        {
            yield return new WaitForSeconds(1.5f);
            DisableCurrentlyUsedShip(); //currently USED or ACTIVE ship ? 
        }

        public event Action OnShipDisabled;
        private void DisableCurrentlyUsedShip()
        {
            currentlyUsedUfo.SetActive(false);
            OnShipDisabled?.Invoke();
        }

        public bool IsAnyUfoActive()
        {
            return currentlyUsedUfo.activeSelf;
        }

        private void OnDestroy()
        {
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnShipDisabled);
        }

        private void OnUfoProjectileDestroyed(object sender, OnProjectileDestroyedArgs args)
        {
            if (args.tagOfObjectHit.Equals(playerShip.gameObject.tag))
            {
                StartCoroutine(DisableShipAfterTimer());
            }
        }
    }

}