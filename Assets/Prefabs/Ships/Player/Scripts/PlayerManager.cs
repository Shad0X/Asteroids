﻿using System;
using System.Collections;
using UnityEngine;

namespace Game.Ships.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField]
        PlayerShipController playerShip;

        [SerializeField]
        GameLogic gameLogic;

        [SerializeField]
        ScoreKeeper scoreKeeper;

        private void ResetPlayerLiveCount()
        {
            livesLeft = GameConfig.PlayerLiveCountAtStart;
            OnPlayerLivesChanged?.Invoke(livesLeft);
        }

        private int livesLeft;
        public event Action<int> OnPlayerLivesChanged;

        void Start()
        {
            DisablePlayerShip();
            ResetPlayerStats();

            gameLogic.OnNewGameStarted += OnNewGame;
            playerShip.OnShipDestroyed += OnPlayerLostLive;
            scoreKeeper.OnScoreChanged += AddExtraLiveDependingOnScore;
        }

        void DisablePlayerShip()
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

        [SerializeField]
        AudioClip playerGainedLive;

        private void OnNewGame(float timeStartedAt)
        {
            ResetPlayerStats();
            EnablePlayerAtWorldCenter();
        }

        private void ResetPlayerStats()
        {
            ResetPlayerLiveCount();
            ResetScoreToGainExtraLife();
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