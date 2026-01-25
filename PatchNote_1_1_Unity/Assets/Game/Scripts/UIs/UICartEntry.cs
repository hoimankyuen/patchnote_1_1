using MoonlightTools.GeneralTools;
using UnityEngine;
using UnityEngine.UI;

public class UICartEntry : ManagedPooledObject<UICartDisplay>
{
    [Header("Components")]
    [SerializeField] private Image image;

    public void Setup(Sprite icon)
    {
        image.sprite = icon;
    }
}
