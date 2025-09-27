using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextCanvas : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private TextEffect mainText;
    [SerializeField] private Image characterImage;    

    public void SetEffectedText(string _text)
    {
        mainText.SetText(_text);
        mainText.OnTextChange();        
    }

    public TextEffect GetEffectedText()
    { 
        return mainText; 
    }

    public virtual void SetCharacterImage(Sprite sprite)
    {
        characterImage.sprite = sprite;
    }
}
