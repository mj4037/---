using UnityEngine;

public class NormalType : Unit
{
    void Awake()
    {
        base.Awake();

        atk = 7;
        hp = 80;
        speed = 4.0f;
    }
}
