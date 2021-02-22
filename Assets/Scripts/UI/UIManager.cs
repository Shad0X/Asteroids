using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{ 
    public class UIManager : MonoBehaviour //Keep as Manager, or divide into smaller classes?
        // 1 Generic class that uses Unity Events instead ? 
        // OnEvent - Show TXT
        // OnEvent 2 - Hide TXT
        // .. can configure it in the Editor, but is more confusing when it comes to code... 
    {
        [SerializeField]
        GameLogic gameLogic;

        [SerializeField]
        Text scoreDisplay;

        [SerializeField]
        Text gameOverText;

        [SerializeField]
        Text pressSpaceToPlayAgainText;

        [SerializeField]
        Text controlsText;

        [SerializeField]
        PlayerLivesDisplayHandler livesDisplay;

        [SerializeField]
        ScoreKeeper scoreKeeper;

        void Start()
        {
            gameLogic.OnNewGameStarted += OnNewGameStarted;
            scoreKeeper.OnScoreChanged += OnScoreChanged;
            gameLogic.OnGameOver += OnGameOver;
            gameLogic.OnCanPlayAgain += OnCanPlayAgain;
            gameLogic.OnShowTitleScreen += OnTitleScreenShown;
        }

        void OnGameOver()
        {
            gameOverText.gameObject.SetActive(true);
        }

        void OnCanPlayAgain()
        {
            pressSpaceToPlayAgainText.gameObject.SetActive(true);
        }

        void OnNewGameStarted()
        {
            gameOverText.gameObject.SetActive(false);
            controlsText.gameObject.SetActive(false);
            pressSpaceToPlayAgainText.gameObject.SetActive(false);
        }

        void OnTitleScreenShown()
        {
            controlsText.gameObject.SetActive(true);
            gameOverText.gameObject.SetActive(false);
            pressSpaceToPlayAgainText.gameObject.SetActive(false);
        }

        void OnScoreChanged(int score)
        {
            scoreDisplay.text = score.ToString();
        }

    }
}