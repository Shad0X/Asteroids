using System.Collections;
using UnityEngine;

namespace Game //Game.GameObjects .. or Game.Prefabs ?? 
{
    public class DisableObjectAfterTime : MonoBehaviour
    {
        [SerializeField]
        float disableAfterSeconds;

        protected void OnEnable()
        {
            StartCoroutine(DisableAfterTime());
        }

        IEnumerator DisableAfterTime()
        {
            yield return new WaitForSeconds(disableAfterSeconds);
            gameObject.SetActive(false);
        }
    }
}