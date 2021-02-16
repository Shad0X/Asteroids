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

        [SerializeField]
        PlayerManager playerManager;

        void Start()
        {
            SetDefaultStartValues();
            CurrentGameState = GameState.Idle;


            PlayerProjectile.OnProjectileDestroyed += UpdateScoreAndCheckForExtraLives;
            Asteroid.OnAsteroidDestroyed += OnAsteroidDestroyed;
            playerManager.OnPlayerLivesChanged += OnPlayerLivesChanged;

            ufoManager.OnUfoDisabled += UfoDisabled;
        }

        void SetDefaultStartValues()
        {
            SetScore(0);
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

            //other
            CurrentGameState = GameState.Playing;
            OnNewGameStarted?.Invoke();
            roundStartedAtTime = Time.time;
        }

        bool canPlayAgain = false;

        private void Update()
        {
            if ((CurrentGameState == GameState.Idle || CurrentGameState == GameState.GameOver && canPlayAgain) && Input.GetKeyUp(KeyCode.Space))
            {
                StartNewGame();
            }
        }

        public event Action OnGameOver;

        private void OnPlayerLivesChanged(int currentLifeCount) //mainly or ONLY stuff for PlayerManager... not rly relevent to GameLogic BESIDES Player LIFE count... 
        {
            if (currentLifeCount <= 0)
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

        void ContinueGame()
        {
            CurrentGameState = GameState.Playing;
        }

        private void OnAsteroidDestroyed(object sender, OnAsteroidDestroyedArgs args)
        {
            if (args.size == GameConfig.SmallAsteroidSize)
            {
                CheckIfToStartNewRound();
            }
        }

        //same stuff in BOTH methods... why not just call CheckIfToStart..... directly, rather than a method to call it... ? 
        public void OnUfoDestroyedOrDisabled() //not great if PUBLIC, no ... ? 
        {
            CheckIfToStartNewRound();
        }

        void UfoDisabled()
        {
            //send out ACTION to inform other scripts.. ? 
            CheckIfToStartNewRound(); //GameLogic.CS method.. 

        }

        private void CheckIfToStartNewRound()
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

        public event Action OnShowTitleScreen;

        private void OnDestroy()
        {
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnNewGameStarted);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnGameOver);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnCanPlayAgain);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnShowTitleScreen);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnScoreChanged);
        }

    }
}