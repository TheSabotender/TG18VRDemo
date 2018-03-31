using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Wait : MonoBehaviour
{
    private static Wait instance;

    void Start()
    {
        instance = this;
    }

    public static void Then(float time, UnityAction action)
    {
        instance.StartCoroutine(instance.DoWait(time, action));
    }

    public static void Then<T>(float time, UnityAction<T> action, T arg)
    {
        instance.StartCoroutine(instance.DoWait(time, action, arg));
    }

    public IEnumerator DoWait(float time, UnityAction action)
    {
        yield return new WaitForSeconds(time);

        action.Invoke();
    }

    public IEnumerator DoWait<T>(float time, UnityAction<T> action, T arg)
    {
        yield return new WaitForSeconds(time);

        action.Invoke(arg);
    }
}
