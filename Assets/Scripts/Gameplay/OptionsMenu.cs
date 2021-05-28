using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public static OptionsMenu instance { get; private set; }

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Creating multiple Instances of OptionsMenu");
            
            return;
        }
        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        instance = this;
    }

    [SerializeField] GameObject Menu;
    [SerializeField] Slider slider;
    
    public void ValueChangeCheck()
    {
        AudioManager.instance.ChangeVolume(slider.value);
  
    }

    public bool Open()
    {
        if (!Menu.activeSelf)
        {
            Menu.SetActive(true);
            return true;
        }
        else
        {
            Menu.SetActive(false);
            return false;
        }

    }

}