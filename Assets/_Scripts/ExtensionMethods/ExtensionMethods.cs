using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Game.Extensions
{
    public static class ExtensionMethods
    {
        public static void Off(this GameObject val)
        {
            val.SetActive(false);
        }
        public static void On(this GameObject val)
        {
            val.SetActive(true);
        }

        public static void DelayedAction(this MonoBehaviour mono, UnityAction action, float delay)
        {
            mono.StartCoroutine(DoAction(delay, action));
        }
        private static IEnumerator DoAction(float delay, UnityAction action)
        {
            yield return new WaitForSecondsRealtime(delay);
            action.Invoke();
        }
    }
}