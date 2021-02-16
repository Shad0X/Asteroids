using System;
using UnityEngine;

namespace Game.UI
{
    public class PlayerLivesDisplayHandler : MonoBehaviour
    {

        [SerializeField]
        GameObject imageObject;

        float horizontalDistanceBetweenImages;

        Vector2 rectTransformSize;
        private void Start()
        {
            horizontalDistanceBetweenImages = GetDistanceBetweenImages();
            rectTransformSize = transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        }

        public void SetImageCount(int ammount)
        {
            int difference = ammount - GetActiveImageCount();
            if (difference > 0) //gained live
            {
                for (int i = 0; i < difference; i++)
                {
                    ShowOneMoreImage();
                }
            }
            else if (difference < 0) //lost live
            {
                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    DisableLastActiveImage();
                }
            }
        }

        void ShowOneMoreImage()
        {
            foreach (Transform child in transform)
            {
                if (!child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true);
                    return;
                }
            }
            AddAndShowOneMoreImage();
        }

        void AddAndShowOneMoreImage()
        {
            Transform lastImage = transform.GetChild(transform.childCount - 1);
            GameObject image = Instantiate(imageObject, transform);
            image.transform.GetComponent<RectTransform>().sizeDelta = rectTransformSize;
            image.transform.position = lastImage.position + new Vector3(horizontalDistanceBetweenImages, 0, 0);
        }

        float GetDistanceBetweenImages()
        {
            Transform first = transform.GetChild(0);
            Transform second = transform.GetChild(1);
            return second.position.x - first.position.x;
        }

        void DisableLastActiveImage()
        {
            int activeChildCount = GetActiveImageCount();
            int index = activeChildCount > 0 ? activeChildCount - 1 : 0;
            transform.GetChild(index).gameObject.SetActive(false);
        }

        int GetActiveImageCount()
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                count += child.gameObject.activeSelf ? 1 : 0;
            }
            return count;
        }

    }
}