using System;

public enum Condition
{
    LOBBY,
    READY,
    BATTLE,
    FINISH
}
    
public static class State
{
    private static Action ready;
    private static Action battle;
    private static Action finish;

    public static void Subscribe(Condition condition, Action action)
    {
        switch (condition)
        {
            case Condition.READY: ready += action;
                break;
            case Condition.FINISH: finish += action;
                break;
            case Condition.BATTLE: battle += action;
                break;
        }
    }

    public static void Unsubscribe(Condition condition, Action action)
    {
        switch (condition)
        {
            case Condition.READY: ready -= action;
                break;
            case Condition.FINISH: finish -= action;
                break;
            case Condition.BATTLE: battle -= action;
                break;
        }
    }

    public static void Publish(Condition condition)
    {
        switch (condition)
        {
            case Condition.READY: ready?.Invoke();
                break;
            case Condition.FINISH: finish?.Invoke();
                break;
            case Condition.BATTLE: battle?.Invoke();
                break;
        }
    }
}
