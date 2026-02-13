using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] GameObject startPanel;

    private void OnEnable()
    {
        State.Subscribe(Condition.READY, ActiveTrue);
        State.Subscribe(Condition.BATTLE, ActiveFalse);
    }

    private void OnDisable()
    {
        State.Unsubscribe(Condition.READY, ActiveTrue);
        State.Unsubscribe(Condition.BATTLE, ActiveFalse);
    }

    public void StartGame()
    {
        State.Publish(Condition.BATTLE);
    }

    void ActiveTrue()
    {
        if (startPanel.activeSelf == false)
        {
            startPanel.SetActive(true);
        }
    }

    void ActiveFalse()
    {
        if(startPanel.activeSelf == true)
        {
            startPanel.SetActive(false);
        }
    }
}
