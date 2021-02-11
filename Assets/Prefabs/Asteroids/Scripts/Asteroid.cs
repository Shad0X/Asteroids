using System;
using UnityEngine;

namespace Game.Asteroids
{
    public class OnAsteroidDestroyedArgs
    {
        public Vector3 location;
        public Vector3 size;
    }

    public class Asteroid : MonoBehaviour
    {
        public static event EventHandler<OnAsteroidDestroyedArgs> OnAsteroidDestroyed; //static, since it shouldn't be attached to a single GameObject due to the amount of Asteroids inGame... 

        [SerializeField]
        AudioClip explosionSound;

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer(GameConfig.AsteroidLayerName);
            Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            AudioSource.PlayClipAtPoint(explosionSound, gameObject.transform.position);
            gameObject.SetActive(false);
            OnAsteroidDestroyed?.Invoke(this, new OnAsteroidDestroyedArgs { size = gameObject.transform.localScale, location = transform.position });
        }

        private void OnDestroy()
        {
            UnsubscribeAllEventListeners();
        }

        void UnsubscribeAllEventListeners()
        {
            if (OnAsteroidDestroyed != null)
            {
                Delegate[] subscribers = OnAsteroidDestroyed.GetInvocationList();
                foreach (Delegate subscriber in subscribers)
                {
                    OnAsteroidDestroyed -= (subscriber as EventHandler<OnAsteroidDestroyedArgs>);
                }
                OnAsteroidDestroyed = null;
            }
        }

    }
}