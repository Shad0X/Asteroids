using System;
using System.Collections;
using UnityEngine;

namespace Game.Ships.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public event Action<int> OnPlayerLivesChanged;

        int livesLeft; //PLAYER SPECIFIC

        [SerializeField]
        PlayerShipController playerShip; //PLAYER SPECIFIC

        [SerializeField]
        AudioClip playerGainedLive; //PLAYER SPECIFIC

        [SerializeField]
        GameLogic gameLogic;

        private void ResetPlayerLiveCount() //PLAYER SPECIFIC
        {
            livesLeft = GameConfig.PlayerLiveCountAtStart;
            OnPlayerLivesChanged?.Invoke(livesLeft);
        }

        void Start()
        {
            ResetPlayerLiveCount();
            playerShip.OnShipDestroyed += OnPlayerLostLive;
            DisablePlayerShip();

            ResetScoreToGainExtraLife();

            gameLogic.OnScoreChanged += AddExtraLiveDependingOnScore;
            gameLogic.OnNewGameStarted += OnNewGame;
        }

        void DisablePlayerShip() //PLAYER SPECIFIC
        {
            playerShip.gameObject.SetActive(false);
        }

        private void OnPlayerLostLive() //mainly or ONLY stuff for PlayerManager... not rly relevent to GameLogic BESIDES Player LIFE count... 
        {
            livesLeft -= 1;
            OnPlayerLivesChanged?.Invoke(livesLeft);
            if (livesLeft > 0)
            {
                StartCoroutine(RespawnPlayerAfterTimer());
            }
        }

        public event Action OnPlayerRespawned; //??? 
        IEnumerator RespawnPlayerAfterTimer()
        {
            yield return new WaitForSeconds(GameConfig.PlayerRespawnTimer);
            EnablePlayerAtWorldCenter();
        }

        private int scoreToGainExtraLife;
        void ResetScoreToGainExtraLife() //PLAYER SPECIFIC
        {
            scoreToGainExtraLife = GameConfig.ScoreForGettingAnotherLife;
        }
        void AddExtraLiveDependingOnScore(int currentScore)
        {
            if (currentScore > scoreToGainExtraLife)
            {
                scoreToGainExtraLife += GameConfig.ScoreForGettingAnotherLife;
                AddLive();
            }
        }

        void AddLive()
        {
            livesLeft += 1;

            if (livesLeft > 250) // https://en.wikipedia.org/wiki/Asteroids_(video_game)#:~:text=Asteroids%20contains%20several%20bugs.,than%20250%20lives%20are%20collected.
            {
                livesLeft = 0;
            }

            OnPlayerLivesChanged?.Invoke(livesLeft);
            AudioSource.PlayClipAtPoint(playerGainedLive, gameObject.transform.position);
        }

        private void OnNewGame()
        {
            ResetScoreToGainExtraLife();
            EnablePlayerAtWorldCenter();
            ResetPlayerLiveCount();
        }

        void EnablePlayerAtWorldCenter()
        {
            playerShip.transform.position = Vector3.zero;
            playerShip.transform.rotation = Quaternion.Euler(Vector3.zero);
            playerShip.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnPlayerLivesChanged);
        }
    }
}