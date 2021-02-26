using Game;
using Game.Ships.Player;
using System;
using UnityEngine;

public class PlayerLives : MonoBehaviour
{
    //ToDo - update ORDER of stuff.. method / variable order so it makes sense and is near the place where they're used.. 
    [SerializeField]
    GameLogic gameLogic;

    [SerializeField]
    PlayerShipController playerShip;

    [SerializeField]
    ScoreKeeper scoreKeeper;

    private int livesLeft;
    public event Action<int> OnPlayerLivesChanged;
    public event Action OnRespawnPlayer;

    [SerializeField]
    AudioClip playerGainedLive;

    void Start()
    {
        gameLogic.OnNewGameStarted += OnNewGame;
        playerShip.OnShipDestroyed += OnPlayerLostLive;
        scoreKeeper.OnScoreChanged += AddExtraLiveDependingOnScore;

        ResetPlayerStats();
    }

    private void ResetPlayerLiveCount()
    {
        livesLeft = GameConfig.PlayerLiveCountAtStart;
        OnPlayerLivesChanged?.Invoke(livesLeft);
    }

    private void OnPlayerLostLive() //mainly or ONLY stuff for PlayerManager... not rly relevent to GameLogic BESIDES Player LIFE count... 
    {
        livesLeft -= 1;
        OnPlayerLivesChanged?.Invoke(livesLeft);
        if (livesLeft > 0)
        {
            OnRespawnPlayer?.Invoke();
        }
    }

    private void ResetPlayerStats()
    {
        ResetPlayerLiveCount();
        ResetScoreToGainExtraLife();
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

    private void OnNewGame(float timeStartedAt)
    {
        ResetPlayerStats();
    }

    void AddExtraLiveDependingOnScore(int currentScore)
    {
        if (currentScore > scoreToGainExtraLife)
        {
            scoreToGainExtraLife += GameConfig.ScoreForGettingAnotherLife;
            AddLive();
        }
    }

    private int scoreToGainExtraLife;
    void ResetScoreToGainExtraLife() //PLAYER SPECIFIC
    {
        scoreToGainExtraLife = GameConfig.ScoreForGettingAnotherLife;
    }

    private void OnDestroy()
    {
        ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnPlayerLivesChanged);
    }
}
