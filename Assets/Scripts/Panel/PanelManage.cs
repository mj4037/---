using UnityEngine;
using UnityEngine.UI;

public class PanelManage: MonoBehaviour
{
    [SerializeField] GameObject[] panels;
    [SerializeField] GameObject mask;
    [SerializeField] Button button;

    [SerializeField] bool overrideIntroSkip;
    [SerializeField] int introSkipOverrideValue;

    int i = 0;

    const string IntroSkipKey = "IntroSkip";

    private void Awake()
    {
        if (overrideIntroSkip)
        {
            PlayerPrefs.SetInt(IntroSkipKey, introSkipOverrideValue);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.GetInt(IntroSkipKey, 0) >= 1)
        {
            for (int j = 0; j < panels.Length; j++)
            {
                panels[j].SetActive(false);
            }

            mask.SetActive(false);
            button.gameObject.SetActive(false);
            State.Publish(Condition.READY);
            return;
        }

        for (int j = 0; j < panels.Length; j++)
        {
            panels[j].SetActive(false);
        }

        panels[i].SetActive(true);
    }

    public void Next()
    {
        panels[i++].SetActive(false);

        if (i == panels.Length)
        {
            mask.SetActive(false);

            button.gameObject.SetActive(false);

            PlayerPrefs.SetInt(IntroSkipKey, 1);
            PlayerPrefs.Save();

            State.Publish(Condition.READY);

            return;
        }

        if(panels[i].activeSelf == false)
        {
            panels[i].SetActive(true);
        }
    }
}
