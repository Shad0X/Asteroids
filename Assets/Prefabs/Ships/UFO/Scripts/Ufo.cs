using Game.ScreenSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ships.Ufo
{
    public class Ufo : Ship
    {
        Rigidbody2D rigidBody;

        [SerializeField]
        Transform target;

        enum UfoSizeType
        {
            Large, Small
        }

        [SerializeField]
        UfoSizeType sizeType;
        float halfWidth;

        [SerializeField]
        GameLogic gameLogic;

        void Start()
        {
            UpdateGameObjectSizeBasedOnSizeType(sizeType);
            halfWidth = transform.localScale.x / 2;
            canon = gameObject.GetComponentInChildren<Canon>();
            rigidBody = gameObject.GetComponent<Rigidbody2D>();
        }

        void SetRandomStartingLocation()
        {
            float randomVerticalLocation = Random.Range(halfWidth, Screen.height - halfWidth);

            bool leftside = (Random.value > 0.5f);
            if (leftside)
            {
                Vector3 leftScreenLocation = ScreenSpaceUtility.ConvertScreenSpaceToWorldSpace(halfWidth, randomVerticalLocation);
                transform.position = leftScreenLocation;
            }
            else
            {

                Vector3 rightScreenLodation = ScreenSpaceUtility.ConvertScreenSpaceToWorldSpace(Screen.width - halfWidth, randomVerticalLocation);
                transform.position = rightScreenLodation;
            }
        }

        void UpdateGameObjectSizeBasedOnSizeType(UfoSizeType type)
        {
            switch (type)
            {
                case UfoSizeType.Large:
                    gameObject.transform.localScale = Vector3.one * 2;
                    break;
                case UfoSizeType.Small:
                    gameObject.transform.localScale = Vector3.one;
                    break;
            }
        }


        private void OnEnable()
        {
            SetRandomStartingLocation();
            SetMovementDirectionTowardsWorldCenter();

            StartCoroutine(ChangeMovementDirection());
            StartCoroutine(FireCanonAfterDelay());
        }

        Vector3 movementDirection;

        static readonly Vector3 LEFT = new Vector3(1, 0, 0);
        static readonly Vector3 RIGHT = new Vector3(-1, 0, 0);
        static readonly Vector3 LEFTUP = new Vector3(1, 1, 0);
        static readonly Vector3 LEFTDOWN = new Vector3(1, -1, 0);
        static readonly Vector3 RIGHTUP = new Vector3(-1, 1, 0);
        static readonly Vector3 RIGHTDOWN = new Vector3(-1, -1, 0);

        private readonly List<Vector3> availableDirections = new List<Vector3>()
    {
        LEFT, RIGHT, LEFTUP, LEFTDOWN, RIGHTUP, RIGHTDOWN
    };

        void SetMovementDirectionTowardsWorldCenter()
        {
            if (transform.position.x > 0)
            {
                movementDirection = RIGHT;
            }
            else
            {
                movementDirection = LEFT;
            }
        }

        void FixedUpdate()
        {
            MoveInDirection(movementDirection);
        }

        [SerializeField]
        float maximumSpeed = 2f;
        private void MoveInDirection(Vector3 direction)
        {
            if (rigidBody.velocity.magnitude < maximumSpeed) // limiting movement speed
            {
                rigidBody.AddForce(direction);
            }
        }

        [SerializeField]
        float timeBetweenChangingFlightDirection = 3f;

        IEnumerator ChangeMovementDirection()
        {
            yield return new WaitForSeconds(timeBetweenChangingFlightDirection);
            int randomIndex = UnityEngine.Random.Range(0, availableDirections.Count);
            movementDirection = availableDirections[randomIndex];
        }

        [SerializeField]
        float timeBetweenFiringCanon = 1f;

        IEnumerator FireCanonAfterDelay()
        {
            yield return new WaitForSeconds(timeBetweenFiringCanon);
            FireCanonBasedOnUfoSizeType();
            StartCoroutine(FireCanonAfterDelay());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        void FireCanonBasedOnUfoSizeType()
        {
            if (sizeType == UfoSizeType.Large)
            {
                FireCanonInRandomDirection();
            }
            if (sizeType == UfoSizeType.Small && target.gameObject.activeSelf)
            {
                FireCanonAtLocation(target.transform.position);
            }
        }

        Canon canon;

        void FireCanonAtLocation(Vector3 location)
        {
            canon.AimAt(location);
            FireCanon();
        }

        void FireCanonInRandomDirection()
        {
            Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
            canon.AimAt(randomDirection);
            FireCanon();
        }

        void FireCanon()
        {
            canon.FireCanon();
        }

    }
}