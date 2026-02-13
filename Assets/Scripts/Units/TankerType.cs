using UnityEngine;

public class TankerType : Unit
{
    void Awake()
    {
        base.Awake();

        atk = 12;
        hp = 200;
        speed = 7;
    }
}
