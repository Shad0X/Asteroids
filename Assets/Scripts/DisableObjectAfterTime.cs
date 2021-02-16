using System.Collections;
using UnityEngine;

namespace Game //Game.GameObjects .. or Game.Prefabs ?? 
{
    public class DisableObjectAfterTime : MonoBehaviour
    {
        [SerializeField]
        float seconds = 1f;

        protected void OnEnable()
        {
            StartCoroutine(DisableAfterTime(seconds));
        }

        IEnumerator DisableAfterTime(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            gameObject.SetActive(false);
        }
    }
}