using Game.Ships.Player;
using Game.Ships.Ufo;
using Game.Asteroids;
using System;
using System.Collections;
using UnityEngine;
using Game.Ships;

//Tooo large... 
// split up into smaller chunks depending on what the Logic is for... 

namespace Game
{
    public class GameLogic : MonoBehaviour
    {
        enum GameState
        {
            Playing, WaitingForNextRound, GameOver, Idle
        }

        GameState gameState;

        //Asteroid Logic
        int asteroidCountAtStart;
        [SerializeField]
        AsteroidManager asteroidManager;

        void Start()
        {
            SetDefaultStartValues();
            gameState = GameState.Idle;

            largeUfo.SetActive(false);
            smallUfo.SetActive(false);
            currentlyUsedUfo = GetRandomUfo();

            DisablePlayerShip();

            largeUfo.GetComponent<Ufo>().OnShipDestroyed += OnUfoDestroyedOrDisabled;
            smallUfo.GetComponent<Ufo>().OnShipDestroyed += OnUfoDestroyedOrDisabled;

            playerShip.OnShipDestroyed += OnPlayerLostLive;
            PlayerProjectile.OnProjectileDestroyed += UpdateScoreAndCheckForExtraLives;
            UfoProjectile.OnProjectileDestroyed += OnUfoProjectileDestroyed;
            Asteroid.OnAsteroidDestroyed += OnAsteroidDestroyed;
            OnScoreChanged += CheckForExtraLivesBasedOnScore;
        }

        void SetDefaultStartValues()
        {
            SetScore(0);
            ResetPlayerLiveCount();
            ResetAsteroidCount();
        }


        float roundStartedAt;
        public event Action OnNewGameStarted;
        public event Action OnCanPlayAgain;

        void StopTitleScreenCoroutineIfAnyExists()
        {
            if (showTitleScreenCoroutine != null)
            {
                StopCoroutine(showTitleScreenCoroutine);
            }
        }

        void StartNewGame()
        {
            StopTitleScreenCoroutineIfAnyExists();

            //General
            SetDefaultStartValues();

            //Player
            EnablePlayerAtWorldCenter();
            ResetScoreToGainExtraLife();

            //Asteroids
            asteroidManager.EnableLargeAsteroids(asteroidCountAtStart);

            //other
            gameState = GameState.Playing;
            DisableCurrentlyUsedUfo();
            OnNewGameStarted?.Invoke();
            roundStartedAt = Time.time;
        }

        void ResetScoreToGainExtraLife()
        {
            currentScoreToGainExtraLife = GameConfig.ScoreForGettingAnotherLife;
        }


        void DisableCurrentlyUsedUfo()
        {
            currentlyUsedUfo.SetActive(false);
        }


        void DisablePlayerShip()
        {
            playerShip.gameObject.SetActive(false);
        }

        void ResetAsteroidCount()
        {
            asteroidCountAtStart = GameConfig.asteroidCountAtNewGame;
        }


        void ContinueGame()
        {
            EnablePlayerAtWorldCenter();
            gameState = GameState.Playing;
        }

        void EnablePlayerAtWorldCenter()
        {
            playerShip.transform.position = Vector3.zero;
            playerShip.transform.rotation = Quaternion.Euler(Vector3.zero);
            playerShip.gameObject.SetActive(true);
        }

        bool canPlayAgain = false;

        private void Update()
        {
            if ((gameState == GameState.Idle || gameState == GameState.GameOver && canPlayAgain) && Input.GetKeyUp(KeyCode.Space))
            {
                StartNewGame();
            }

            if (ShouldSpawnUfo()) //checking every frame, which isn't nice. Perhaps a coroutine or some other way of calling it periodically, rather than every frame... 
            {
                EnableUfoBasedOnCurrentScore();
            }

        }


        bool ShouldSpawnUfo()
        {
            return gameState == GameState.Playing &&
                roundStartedAt + GameConfig.SpawnEnemyPeriod < Time.time &&
                lastTimeUfoSpawned + GameConfig.SpawnEnemyPeriod < Time.time &&
                !IsUfoCurrentlyActive();
        }


        float lastTimeUfoSpawned = 0;

        [SerializeField]
        GameObject largeUfo;

        [SerializeField]
        GameObject smallUfo;

        GameObject currentlyUsedUfo;

        void EnableUfoBasedOnCurrentScore()
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

        private GameObject GetRandomUfo()
        {
            bool returnSmallUfo = UnityEngine.Random.value > 0.5f;
            if (returnSmallUfo)
            {
                return smallUfo;
            }
            return largeUfo;
        }


        int playerLives;

        [SerializeField]
        PlayerShipController playerShip;

        [SerializeField]
        AudioClip playerGainedLive;

        public event Action OnGameOver;
        public event Action<int> OnPlayerLivesChanged;

        private void ResetPlayerLiveCount()
        {
            playerLives = GameConfig.PlayerLiveCountAtStart;
            OnPlayerLivesChanged?.Invoke(playerLives);
        }

        private void OnPlayerLostLive()
        {
            playerLives -= 1;
            OnPlayerLivesChanged?.Invoke(playerLives);
            if (playerLives > 0)
            {
                StartCoroutine(RespawnPlayerAfterTimer());
            }
            else
            {
                HandleGameOver();
            }
        }

        IEnumerator showTitleScreenCoroutine;
        void HandleGameOver()
        {
            canPlayAgain = false;
            gameState = GameState.GameOver;
            StartCoroutine(AllowToPlayAgainAfterSomeTime());
            showTitleScreenCoroutine = ShowTitleScreenAfterSomeTime();
            StartCoroutine(showTitleScreenCoroutine);
            OnGameOver?.Invoke();
        }

        IEnumerator AllowToPlayAgainAfterSomeTime()
        {
            yield return new WaitForSeconds(5f);
            OnCanPlayAgain?.Invoke();
            canPlayAgain = true;
        }

        IEnumerator RespawnPlayerAfterTimer()
        {
            yield return new WaitForSeconds(GameConfig.PlayerRespawnTimer);
            ContinueGame();
        }



        private void OnUfoProjectileDestroyed(object sender, OnProjectileDestroyedArgs args)
        {
            if (args.tagOfObjectHit.Equals(playerShip.gameObject.tag))
            {
                StartCoroutine(DisableEnemyShipAfterTimer());
            }
        }

        IEnumerator DisableEnemyShipAfterTimer()
        {
            yield return new WaitForSeconds(1.5f);
            DisableCurrentlyUsedUfo();
            CheckIfToStartNewRound();
        }

        private void OnAsteroidDestroyed(object sender, OnAsteroidDestroyedArgs args)
        {
            if (args.size == GameConfig.SmallAsteroidSize)
            {
                CheckIfToStartNewRound();
            }
        }

        private void OnUfoDestroyedOrDisabled()
        {
            CheckIfToStartNewRound();
        }

        void CheckIfToStartNewRound()
        {
            if (IsRoundOver())
            {
                StartCoroutine(StartMewRoundAfterDelay());
            }
        }


        IEnumerator StartMewRoundAfterDelay()
        {
            gameState = GameState.WaitingForNextRound;
            yield return new WaitForSeconds(GameConfig.TimeBeforeNewRoundStarts);
            StartNewRound();
        }



        void StartNewRound()
        {
            asteroidCountAtStart += GameConfig.asteroidCountToAddPerNewRound;
            asteroidManager.EnableLargeAsteroids(asteroidCountAtStart);
            gameState = GameState.Playing;
            roundStartedAt = Time.time;
        }

        private bool IsRoundOver()
        {
            if (!asteroidManager.AnyAsteroidsActive() && !IsUfoCurrentlyActive())
            {
                return true;
            }
            return false;
        }


        bool IsUfoCurrentlyActive()
        {
            return currentlyUsedUfo.activeSelf;
        }


        public int GetCurrentPlayerScore()
        {
            return score;
        }

        private int score;
        private int currentScoreToGainExtraLife;
        public event Action<int> OnScoreChanged;

        private void SetScore(int value)
        {
            score = value;
            OnScoreChanged?.Invoke(score);
        }

        void AddPointsToScore(int points)
        {
            SetScore(score + points);
        }

        IEnumerator ShowTitleScreenAfterSomeTime()
        {
            yield return new WaitForSeconds(20f);
            OnShowTitleScreen?.Invoke();
        }

        private void UpdateScoreAndCheckForExtraLives(object sender, OnProjectileDestroyedArgs args)
        {
            switch (args.tagOfObjectHit)
            {
                case GameConfig.LargeAsteroidTag:
                    AddPointsToScore(GameConfig.LargeAsteroidPointsValue);
                    break;
                case GameConfig.MediumAsteroidTag:
                    AddPointsToScore(GameConfig.MediumAsteroidPointsValue);
                    break;
                case GameConfig.SmallAsteroidTag:
                    AddPointsToScore(GameConfig.SmallAsteroidPointsValue);
                    break;
                case GameConfig.UfoTag:
                    AddPointsToScore(GameConfig.UfoPointsValue);
                    break;
            }
        }


        void CheckForExtraLivesBasedOnScore(int newScore)
        {
            if (newScore - currentScoreToGainExtraLife > 0)
            {
                currentScoreToGainExtraLife += GameConfig.ScoreForGettingAnotherLife;
                AddLive();
            }
        }

        void AddLive()
        {
            playerLives += 1;

            if (playerLives > 250) // https://en.wikipedia.org/wiki/Asteroids_(video_game)#:~:text=Asteroids%20contains%20several%20bugs.,than%20250%20lives%20are%20collected.
            {
                playerLives = 0;
            }

            OnPlayerLivesChanged?.Invoke(playerLives);
            AudioSource.PlayClipAtPoint(playerGainedLive, gameObject.transform.position);
        }

        public event Action OnShowTitleScreen;

        private void OnDestroy()
        {
            UnsubscribeActionListenersFrom(ref OnNewGameStarted);
            UnsubscribeActionListenersFrom(ref OnGameOver);
            UnsubscribeActionListenersFrom(ref OnCanPlayAgain);
            UnsubscribeActionListenersFrom(ref OnShowTitleScreen);
            UnsubscribeActionListenersFrom(ref OnPlayerLivesChanged);
            UnsubscribeActionListenersFrom(ref OnScoreChanged);
        }

        void UnsubscribeActionListenersFrom(ref Action<int> action)
        {
            if (action != null)
            {
                Delegate[] subscribers = action.GetInvocationList();
                foreach (Delegate subscriber in subscribers)
                {
                    action -= (subscriber as Action<int>);
                }
                action = null;
            }
        }

        void UnsubscribeActionListenersFrom(ref Action action)
        {
            if (action != null)
            {
                Delegate[] subscribers = action.GetInvocationList();
                foreach (Delegate subscriber in subscribers)
                {
                    action -= (subscriber as Action);
                }
                action = null;
            }
        }

    }
}