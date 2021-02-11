using System;
using UnityEngine;

namespace Game.Ships.Ufo
{
    [RequireComponent(typeof(DisableObjectAfterTime))]
    public class UfoProjectile : MonoBehaviour
    {
        public static EventHandler<OnProjectileDestroyedArgs> OnProjectileDestroyed;

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
                Delegate[] listeners = OnProjectileDestroyed.GetInvocationList();
                foreach (Delegate listener in listeners)
                {
                    OnProjectileDestroyed -= (listener as EventHandler<OnProjectileDestroyedArgs>);
                }
            }
        }

    }
}