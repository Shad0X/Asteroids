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
        PlayerLives playerLives;
        //Sphagetti code ...END... 

        void Start()
        {
            CurrentGameState = GameState.Idle;

            Asteroid.OnAsteroidDestroyed += OnAsteroidDestroyed;
            
            ufoManager.OnShipDisabled += OnUfoDisabled; //UFO disabled

            playerLives.OnPlayerLivesChanged += OnPlayerLivesChanged;
        }

        public event Action<float> OnNewGameStarted;
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
            OnNewGameStarted?.Invoke(Time.time);
        }

        IEnumerator showTitleScreenCoroutine;
        public event Action OnGameOver;

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






//All 3 pretty much DO THE SAME thing... check if the Round is over.... 

        //KINDA Asteroid specific, BUT Affects Game LOGIC... 
        private void OnAsteroidDestroyed(object sender, OnAsteroidDestroyedArgs args)
        {
            if (args.size == GameConfig.SmallAsteroidSize)
            {
                StartNewRoundIfPossible();
            }
        }

        //same stuff in BOTH methods... why not just call CheckIfToStart..... directly, rather than a method to call it... ? 
        public void OnUfoDestroyedOrDisabled() //not great if PUBLIC, no ... ?  SHOULDN'T allow OTHER Classes to use this as a SUBSCRIBER... 
        {
            StartNewRoundIfPossible();
        }

        void OnUfoDisabled()
        {
            //send out ACTION to inform other scripts.. ? 
            StartNewRoundIfPossible(); //GameLogic.CS method.. 
        }

        private void StartNewRoundIfPossible() 
            //name isn't ideal, but better than OLD 1...  
        {
            if (IsRoundOver())
            {
                StartCoroutine(StartMewRoundAfterDelay());
                //could be an issue if the Player is blown up AFTER starting CoRoutine ? 
                    // ex - UFO shoots @ Player, Player blows up UFO > starts counter > UFO Projectile blows up Player... 
            }
        }












        //Could just be a separate Event ? No real need for GameLogic to know the LIFE Count.. just that it's 0... 
        // we're just kinda reUsing it since we already have it for the LIFE Display.. sort of.. 
        private void OnPlayerLivesChanged(int currentLifeCount) //mainly or ONLY stuff for PlayerManager... not rly relevent to GameLogic BESIDES Player LIFE count... 
        {
            if (currentLifeCount <= 0)
            {
                HandleGameOver();
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
            OnNewRoundStarted.Invoke(Time.time);
        }

        private bool IsRoundOver()
        {
            if (!asteroidManager.AnyAsteroidsActive() && !ufoManager.IsAnyUfoActive())
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
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnNewRoundStarted);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnNewGameStarted);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnGameOver);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnCanPlayAgain);
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnShowTitleScreen);
        }

    }
}