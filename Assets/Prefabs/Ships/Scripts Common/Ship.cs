using System;
using UnityEngine;

namespace Game.Ships
{
    public class Ship : MonoBehaviour
    {
        public event Action OnShipDestroyed;

        [SerializeField]
        AudioClip explosionSound;
        private void OnCollisionEnter2D(Collision2D collision)
        {
            AudioSource.PlayClipAtPoint(explosionSound, gameObject.transform.position);
            gameObject.SetActive(false);
            OnShipDestroyed?.Invoke();
        }
        private void OnDestroy()
        {
            UnsubscribeAllEventListeners();
        }

        void UnsubscribeAllEventListeners()
        {
            if (OnShipDestroyed != null)
            {
                Delegate[] subscribers = OnShipDestroyed.GetInvocationList();
                foreach (Delegate subscriber in subscribers)
                {
                    OnShipDestroyed -= (subscriber as Action);
                }
                OnShipDestroyed = null;
            }
        }
    }
}