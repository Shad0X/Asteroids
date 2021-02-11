using UnityEngine;

namespace Game.Ships
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        GameObjectPool projectilePool;

        [SerializeField]
        AudioClip firingSound;

        [SerializeField]
        float projectileSpeed = 10f;

        public void FireWeapon()
        {
            GameObject projectile = projectilePool.GetObject();
            projectile.transform.position = gameObject.transform.position;
            projectile.SetActive(true);
            projectile.GetComponent<Rigidbody2D>().AddForce(gameObject.transform.up * projectileSpeed, ForceMode2D.Impulse);
            AudioSource.PlayClipAtPoint(firingSound, gameObject.transform.position);
        }
    }
}