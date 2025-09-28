using System.Collections;
using UnityEngine;
using TMPro;
using System.Text;

// 사용법 : 효과를 주고 싶은 TextMeshProUGUI가 있는 게임 오브젝트에 부착합니다. (예, TextBoxCanvas > TextBox > Text)
// DialogueManager의 AfterClick 함수를 수정하여 타자기 효과를 시작하는 라인을 추가해야 합니다. (case별로 다름)
// DialogueManager의 Dialogue 코루틴을 수정하여 한 번 더 터치 시 타이핑 효과를 종료하는 기능을 추가해야 합니다. (완료)

public class TextEffect : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string currentText;
    public bool isTyping = false;

    private StringBuilder stringBuilder;
    private StringBuilder tempStringBuilder;
    private Coroutine _currentCoroutine = null;

    private const float typingInterval = 0.04f;
    void Awake()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        stringBuilder = new StringBuilder();
        tempStringBuilder = new StringBuilder();
    }

    public void SetText(string _text)
    {
        text.text = _text;
    }

    public void OnTextChange()
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);
        currentText = text.text;
        _currentCoroutine = StartCoroutine(TypeWriterEffect(currentText));
        text.text = "";
    }

    protected virtual IEnumerator TypeWriterEffect(string fullText)
    {
        isTyping = true;
        stringBuilder.Clear();
        for (int i = 0; i < fullText.Length; i++)
        {
            tempStringBuilder.Clear();
            if (i < fullText.Length)
                stringBuilder.Append(fullText[i]);
            text.text = stringBuilder.ToString();
            yield return new WaitForSecondsRealtime(typingInterval);
        }
        isTyping = false;
    }

    public void ShowFullText()
    {
        StopCoroutine(_currentCoroutine);
        text.text = currentText;
        isTyping = false;
    }
}