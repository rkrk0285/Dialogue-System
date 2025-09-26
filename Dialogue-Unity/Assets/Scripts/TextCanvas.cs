using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextCanvas : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private TextEffect mainText;
    [SerializeField] private Image characterImage;    

    public TextEffect GetEffectText()
    { 
        return mainText; 
    }

    public virtual void SetCharacterImage(Sprite sprite)
    {
        characterImage.sprite = sprite;
    }    
}
