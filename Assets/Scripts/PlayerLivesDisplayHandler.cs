using System;
using UnityEngine;

namespace Game.UI
{
    public class PlayerLivesDisplayHandler : MonoBehaviour
    {

        [SerializeField]
        GameObject imageObject;

        float xDistanceBetweenImages;

        Vector2 rectTransformSize;
        private void Start()
        {
            xDistanceBetweenImages = GetDistanceBetweenImages();
            rectTransformSize = transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        }

        public void SetImageCount(int liveCount)
        {
            int difference = liveCount - GetActiveChildCount();
            if (difference > 0) //gained live
            {
                for (int i = 0; i < difference; i++)
                {
                    ShowOneMore();
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

        void ShowOneMore()
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
            image.transform.position = lastImage.position + new Vector3(xDistanceBetweenImages, 0, 0);
        }

        float GetDistanceBetweenImages()
        {
            Transform first = transform.GetChild(0);
            Transform second = transform.GetChild(1);
            return second.position.x - first.position.x;
        }

        void DisableLastActiveImage()
        {
            int activeChildCount = GetActiveChildCount();
            int index = activeChildCount > 0 ? activeChildCount - 1 : 0;
            transform.GetChild(index).gameObject.SetActive(false);
        }

        int GetActiveChildCount()
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