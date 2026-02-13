using UnityEngine;
using UnityEngine.UIElements;

public class WaveManager : MonoBehaviour
{
    [SerializeField] public int wave = 1;

    public void WaveUp()
    {
        wave++;
    }

    public bool BossWave()
    {
        if (wave % 10 == 0)
        {
            return true;
        }

        return false;
    }
}
