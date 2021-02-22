using Game.Ships.Player;
using Game.Ships.Ufo;
using Game.Asteroids;
using System;
using System.Collections;
using UnityEngine;

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

        //Sphagetti code ... 
        [SerializeField]
        AsteroidManager asteroidManager;

        [SerializeField]
        UfoManager ufoManager;

        [SerializeField]
        PlayerManager playerManager;
        //Sphagetti code ...END... 

        void Start()
        {
            CurrentGameState = GameState.Idle;

            Asteroid.OnAsteroidDestroyed += OnAsteroidDestroyed;
            
            ufoManager.OnUfoDisabled += OnUfoDisabled; //UFO disabled

            playerManager.OnPlayerLivesChanged += OnPlayerLivesChanged;
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

        bool canPlayAgain = false;

        private void Update()
        {
            if ((CurrentGameState == GameState.Idle || CurrentGameState == GameState.GameOver && canPlayAgain) && Input.GetKeyUp(KeyCode.Space))
            {
                StartNewGame();
            }
        }

        void StartNewGame()
        {
            StopTitleScreenCoroutineIfAnyExists();

            //other
            CurrentGameState = GameState.Playing;
            OnNewGameStarted?.Invoke();
            roundStartedAtTime = Time.time;
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

        //KINDA Asteroid specific, BUT Affects Game LOGIC... 
        private void OnAsteroidDestroyed(object sender, OnAsteroidDestroyedArgs args)
        {
            if (args.size == GameConfig.SmallAsteroidSize)
            {
                CheckIfToStartNewRound();
            }
        }

        //same stuff in BOTH methods... why not just call CheckIfToStart..... directly, rather than a method to call it... ? 
        public void OnUfoDestroyedOrDisabled() //not great if PUBLIC, no ... ?  SHOULDN'T allow OTHER Classes to use this as a SUBSCRIBER... 
        {
            CheckIfToStartNewRound();
        }

        void OnUfoDisabled()
        {
            //send out ACTION to inform other scripts.. ? 
            CheckIfToStartNewRound(); //GameLogic.CS method.. 
        }

        public event Action OnGameOver;

        private void OnPlayerLivesChanged(int currentLifeCount) //mainly or ONLY stuff for PlayerManager... not rly relevent to GameLogic BESIDES Player LIFE count... 
        {
            if (currentLifeCount <= 0)
            {
                HandleGameOver();
            }
        }

        private void CheckIfToStartNewRound() //DOES MORE than the name implies... NOT just CHECKING, but also TRIGGERING a NEW ROUND timer... 
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

        IEnumerator ShowTitleScreenAfterSomeTime()
        {
            //kinda a UI thing, but can't have logic inside the UI itself really.. it should just show n hide UI n update it.. 
            yield return new WaitForSeconds(20f);
            OnShowTitleScreen?.Invoke();
        }

        public event Action OnShowTitleScreen;

        private void OnDestroy()
        {
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnNewGameStarted);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnGameOver);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnCanPlayAgain);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnShowTitleScreen);
        }

    }
}