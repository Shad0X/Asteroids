using UnityEngine;

namespace Game
{
    //Scriptable OBJ instead, so it can be changed in the Editor by anybody, not just in code ?
    // .. could be a bit of OVER ENGINEERING THOUGH... 
    public struct GameConfig
    //not sure if it's good to have something like this... 
    // could make all fields Serializable so people can edit them in the EDITOR, but since they're CONST there's no instance of it
    // meaning that a SCRIPTABLE OBJECT might be a better option ? 
    // problem is that editing values wont affect the game until AFTER RESTART... so Tuning stuff DURING the game wont be an option.. 
    {
        //General
        public const float TimeBeforeNewRoundStarts = 3f;

        //Player
        public const int PlayerLiveCountAtStart = 3;
        public const int ScoreForGettingAnotherLife = 10000;
        public const float PlayerRespawnTimer = 3f;

        //UFO
        public const string UfoTag = "UFO";
        public const float SpawnEnemyPeriod = 20f;
        public const int ScoreForSmalUFOsOnly = 100000;
        public const int UfoPointsValue = 200;

        //Asteroids
        public const int asteroidCountAtNewGame = 4;
        public const int asteroidCountToAddPerNewRound = 1;

        public const string AsteroidLayerName = "Asteroids";

        public const string LargeAsteroidTag = "Asteroid_L";
        public const string MediumAsteroidTag = "Asteroid_M";
        public const string SmallAsteroidTag = "Asteroid_S";

        public const int LargeAsteroidPointsValue = 20;
        public const int MediumAsteroidPointsValue = 50;
        public const int SmallAsteroidPointsValue = 100;

        public static Vector3 LargeAsteroidSize = Vector3.one * 2;
        public static Vector3 MediumAsteroidSize = Vector3.one;
        public static Vector3 SmallAsteroidSize = Vector3.one / 2;

    }
}