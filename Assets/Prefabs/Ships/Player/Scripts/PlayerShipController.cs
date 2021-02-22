using Game.ScreenSpace;
using UnityEngine;

namespace Game.Ships.Player
{
    public class PlayerShipController : Ship
    {
        Weapon weapon;
        Rigidbody2D rigidBody;

        [SerializeField]
        AudioClip extraLifeGained;

        void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody2D>();
            weapon = transform.GetChild(0).GetComponent<Weapon>();
        }

        void Update()
        {

            if (Input.GetKeyUp(KeyCode.Space))
            {
                weapon.Shoot();
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                TeleportToRandomLocation();
            }
        }

        private void TeleportToRandomLocation()
        {
            rigidBody.position = ScreenSpaceUtility.GetRandomLocationInWorldSpace();
        }

        [SerializeField]
        float maximumMovementSpeed = 2f;

        [SerializeField]
        float movementSpeed = 10f;

        [SerializeField]
        float rotationSpeed = 2f;

        float verticalInput;
        float horizontalInput;

        private void FixedUpdate()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            rigidBody.MoveRotation(rigidBody.rotation + (-horizontalInput * rotationSpeed));

            verticalInput = Input.GetAxis("Vertical");
            if (verticalInput > 0 && rigidBody.velocity.magnitude < maximumMovementSpeed) // limiting movement speed
            {
                rigidBody.AddForce(gameObject.transform.up * movementSpeed * verticalInput);
            }

            rigidBody.AddForce(-rigidBody.velocity * 0.25f); //slowing down over time
        }

    }
}