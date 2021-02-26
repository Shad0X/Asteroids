using System.Collections;
using UnityEngine;

namespace Game.Ships.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField]
        GameLogic gameLogic;

        [SerializeField]
        PlayerShipController playerShip;

        [SerializeField]
        PlayerLives playerLives;

        void Start()
        {
            DisablePlayerShip();
            gameLogic.OnNewGameStarted += OnNewGame;
            playerLives.OnRespawnPlayer += OnPlayerLivesChanged;
        }

        void DisablePlayerShip()
        {
            playerShip.gameObject.SetActive(false);
        }

        //public event Action OnPlayerRespawned; //??? 
        IEnumerator RespawnPlayerAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            EnablePlayerAtWorldCenter();
        }

        private void OnPlayerLivesChanged() 
        {
            StartCoroutine(RespawnPlayerAfterSeconds(GameConfig.PlayerRespawnTimer));
        }

        private void OnNewGame(float timeStartedAt)
        {
            EnablePlayerAtWorldCenter(); //spaning..
        }

        void EnablePlayerAtWorldCenter() //only PARTIALLY Player specific.. generally very similar to other Methods in Asteroid / Ufo that Place them on Screen @ X, Z.. 
        {
            playerShip.transform.position = Vector3.zero;
            playerShip.transform.rotation = Quaternion.Euler(Vector3.zero);
            playerShip.gameObject.SetActive(true);
        }

    }
}