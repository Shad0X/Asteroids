using Game;
using Game.Ships;
using Game.Ships.Player;
using System;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField]
    GameLogic gameLogic;

    void Start()
    {
        SetScore(0);
        PlayerProjectile.OnProjectileDestroyed += UpdateScoreAndCheckForExtraLives;
        gameLogic.OnNewGameStarted += OnNewGameStarted;
    }

    public int Score
    {
        get; private set;
    }

    private void OnNewGameStarted(float timeStartedAt)
    {
        SetScore(0);
    }

    public event Action<int> OnScoreChanged; //does it need the "event" part ??? 

    private void SetScore(int value) //kinda pointless, since the Score VARIABLE already has a private set, no ?
    {
        Score = value;
        OnScoreChanged?.Invoke(Score);
    }

    void AddPointsToScore(int points)
    {
        SetScore(Score + points);
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

    private void OnDestroy()
    {
        ActionListenerUtility.UnsubscribeActionListenersFrom(ref OnScoreChanged);
    }
}
