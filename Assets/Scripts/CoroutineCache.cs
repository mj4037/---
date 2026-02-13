using System.Collections.Generic;
using UnityEngine;

public class CoroutineCache : MonoBehaviour
{
    static Dictionary<float, WaitForSeconds> dictionary = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds WaitForSeconds(float time)
    {
        WaitForSeconds waitForSeconds;

        if(dictionary.TryGetValue(time, out waitForSeconds) == false)
        {
            dictionary.Add(time, new WaitForSeconds(time));

            waitForSeconds = dictionary[time];
        }

        return waitForSeconds;
    }
}
