using System;
using UnityEngine;

namespace Game.Ships.Player
{
    [RequireComponent(typeof(DisableObjectAfterTime))]
    public class PlayerProjectile : MonoBehaviour
    {

        public static event EventHandler<OnProjectileDestroyedArgs> OnProjectileDestroyed;

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            gameObject.SetActive(false);
            OnProjectileDestroyed?.Invoke(this,
            new OnProjectileDestroyedArgs
            {
                tagOfObjectHit = collision.gameObject.tag
            });
        }

        private void OnDestroy()
        {
            UnsubscribeEventListeners();
        }

        void UnsubscribeEventListeners()
        {
            if (OnProjectileDestroyed != null)
            {

                Delegate[] subscribers = OnProjectileDestroyed.GetInvocationList();
                foreach (Delegate subscriber in subscribers)
                {
                    OnProjectileDestroyed -= (subscriber as EventHandler<OnProjectileDestroyedArgs>);
                }
            }
        }
    }
}