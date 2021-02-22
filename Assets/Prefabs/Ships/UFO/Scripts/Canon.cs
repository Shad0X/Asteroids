using UnityEngine;

namespace Game.Ships
{
    public class Canon : MonoBehaviour
    {
        Weapon weapon;
        private void Start()
        {
            weapon = gameObject.GetComponentInChildren<Weapon>();
        }

        Vector3 direction;
        float angle;
        public void AimAt(Vector3 location)
        {
            direction = location - transform.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        public void Shoot()
        {
            weapon.Shoot();
        }
    }
}