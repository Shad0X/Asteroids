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

    public enum GameState
    {
        Playing, WaitingForNextRound, GameOver, Idle
    }

    public partial class GameLogic : MonoBehaviour
    {

        public GameState CurrentGameState {
            get; private set;
        }



        [SerializeField]
        AsteroidManager asteroidManager;

        [SerializeField]
        UfoManager ufoManager;

        void Start()
        {
            SetDefaultStartValues();
            CurrentGameState = GameState.Idle;

            DisablePlayerShip();

            playerShip.OnShipDestroyed += OnPlayerLostLive;
            PlayerProjectile.OnProjectileDestroyed += UpdateScoreAndCheckForExtraLives;
            Asteroid.OnAsteroidDestroyed += OnAsteroidDestroyed;
            OnScoreChanged += AddExtraLiveDependingOnScore;

            ufoManager.OnUfoDisabled += UfoDisabled;
        }

        void SetDefaultStartValues()
        {
            SetScore(0);
            ResetPlayerLiveCount();

        }


        float roundStartedAtTime;
        public event Action OnNewGameStarted;
        public event Action<float> OnNewRoundStarted;
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

            //other
            CurrentGameState = GameState.Playing;
            //DisableCurrentlyUsedUfo(); //realistically not needed since there shouldnt be any Active UFOs ??
            OnNewGameStarted?.Invoke();
            roundStartedAtTime = Time.time;
        }

        void ResetScoreToGainExtraLife()
        {
            scoreToGainExtraLife = GameConfig.ScoreForGettingAnotherLife;
        }





        void DisablePlayerShip()
        {
            playerShip.gameObject.SetActive(false);
        }




        void ContinueGame()
        {
            EnablePlayerAtWorldCenter();
            CurrentGameState = GameState.Playing;
        }

        void EnablePlayerAtWorldCenter() //PLAYER SPECIFIC
        {
            playerShip.transform.position = Vector3.zero;
            playerShip.transform.rotation = Quaternion.Euler(Vector3.zero);
            playerShip.gameObject.SetActive(true);
        }

        bool canPlayAgain = false;

        private void Update()
        {
            if ((CurrentGameState == GameState.Idle || CurrentGameState == GameState.GameOver && canPlayAgain) && Input.GetKeyUp(KeyCode.Space))
            {
                StartNewGame();
            }

     
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
            CurrentGameState = GameState.GameOver;
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





   

        private void OnAsteroidDestroyed(object sender, OnAsteroidDestroyedArgs args)
        {
            if (args.size == GameConfig.SmallAsteroidSize)
            {
                CheckIfToStartNewRound();
            }
        }

        public void OnUfoDestroyedOrDisabled() //not great if PUBLIC, no ... ? 
        {
            CheckIfToStartNewRound();
        }

        void UfoDisabled()
        {
            //send out ACTION to inform other scripts.. ? 
            CheckIfToStartNewRound(); //GameLogic.CS method.. 

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
            CurrentGameState = GameState.WaitingForNextRound;
            yield return new WaitForSeconds(GameConfig.TimeBeforeNewRoundStarts);
            StartNewRound();
        }



        void StartNewRound()
        {


            //GameLogic stuff
            CurrentGameState = GameState.Playing;
            roundStartedAtTime = Time.time;
            OnNewRoundStarted.Invoke(roundStartedAtTime);
        }

        private bool IsRoundOver()
        {
            if (!asteroidManager.AnyAsteroidsActive() && !ufoManager.IsUfoCurrentlyActive())
            {
                return true;
            }
            return false;
        }

        public int Score {
            get; private set;
        }

        private int scoreToGainExtraLife;
        public event Action<int> OnScoreChanged;

        private void SetScore(int value)
        {
            Score = value;
            OnScoreChanged?.Invoke(Score);
        }

        void AddPointsToScore(int points)
        {
            SetScore(Score + points);
        }

        IEnumerator ShowTitleScreenAfterSomeTime()
        {
            //kinda a UI thing, but can't have logic inside the UI itself really.. it should just show n hide UI n update it.. 
            yield return new WaitForSeconds(20f);
            OnShowTitleScreen?.Invoke();
        }

        private void UpdateScoreAndCheckForExtraLives(object sender, OnProjectileDestroyedArgs args)
        {
            //alternative would be to get an Object reference and get POINTs directly from it ??
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


        void AddExtraLiveDependingOnScore(int currentScore)
        {
            if (currentScore - scoreToGainExtraLife > 0)
            {
                scoreToGainExtraLife += GameConfig.ScoreForGettingAnotherLife;
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
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnNewGameStarted);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnGameOver);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnCanPlayAgain);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnShowTitleScreen);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnPlayerLivesChanged);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnScoreChanged);
        }

    }
}