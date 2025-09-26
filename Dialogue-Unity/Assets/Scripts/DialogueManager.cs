using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance = null;

    [Header("Canvas")]
    [SerializeField] GameObject touchInputCanvas; // 터치 입력을 받는 캔버스
    [SerializeField] GameObject noticeDialogueCanvas; // 나레이션 출력 캔버스 (나레이션 느낌의 다이얼로그)
    [SerializeField] GameObject conversationDialogueCanvas; // 대화 출력 캔버스
    [SerializeField] GameObject optionalDialogueCanvas; // 선택지 출력 캔버스

    [Header("Ingame Object")]
    [SerializeField] GameObject noticeCharacter; // 나레이션 캐릭터 표기용 오브젝트

    [Header("Prefab")]
    [SerializeField] GameObject selectBoxPrefab; // 선택지박스 프리팹

    [Header("Sprite")]
    [SerializeField] Sprite[] characterSpriteList; // 캐릭터 스프라이트 배열

    private List<Struct_Dialogue> dialogueList; // 현재 카테고리에 속한 다이얼로그를 List로 불러옴.
    private Struct_Dialogue currentDialogue; // 현재 다이얼로그 리스트에서 진행해야할 다이얼로그
    public bool isClicked = false; // 터치 캔버스에서 터치를 받으면 isClicked Toggle되는 형식으로 사용.
    private Coroutine currentCoroutine = null; // 코루틴 실행되는지 확인용.
    private int dialogueIdx = 0; // 현재 다이얼로그의 인덱스

    private TextEffect _textEffect = null; // 현재 이펙트 사용 중인 텍스트

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    /// <summary>
    /// 디버깅용 다이얼로그 로드
    /// </summary>
    private void Start()
    {
        StartDialogue("Intro");
    }

    /// <summary>
    /// 다이얼로그 가공 처리
    /// </summary>
    void ProcessingDialogue()
    {
        CloseAllCanvas();
        string dialogueType = currentDialogue.GetDialogueType();

        // 터치 입력이 필요한 경우
        if (dialogueType == "나레이션" || 
            dialogueType == "대화" || 
            dialogueType == "")
            touchInputCanvas.SetActive(true);
        
        string text = currentDialogue.GetText();
        // 각 케이스에 맞는 다이얼로그 처리
        switch (dialogueType)
        {
            case "나레이션":
                noticeDialogueCanvas.SetActive(true);
                TextCanvas noticeCanvas = noticeDialogueCanvas.GetComponent<TextCanvas>();

                // 텍스트 이펙트
                _textEffect = noticeCanvas.GetEffectText();
                _textEffect.SetText(text);
                break;
            case "대화":
                conversationDialogueCanvas.SetActive(true);
                TextCanvas conversationCanvas = conversationDialogueCanvas.GetComponent<TextCanvas>();

                // 캐릭터 이미지
                Sprite characterSprite = GetCharacterSprite(currentDialogue.GetCharacter());
                if (characterSprite != null)
                {
                    conversationCanvas.SetCharacterImage(characterSprite);
                }

                // 텍스트 이펙트
                _textEffect = conversationCanvas.GetEffectText();
                _textEffect.SetText(text);
                break;
            case "선택지2":
            case "선택지3":
                // 선택지 뒤 숫자 추출
                int optionalCount = (int)char.GetNumericValue(dialogueType[3]);

                // 먼저 선택지 오브젝트들을 초기화
                foreach (Transform prefab in optionalDialogueCanvas.transform.GetChild(0).transform)
                {
                    Destroy(prefab.gameObject);
                }
                optionalDialogueCanvas.SetActive(true);

                // 선택지 개수만큼 생성
                for (int i = 0; i < optionalCount; i++)
                {
                    // 변수 로컬 캡쳐
                    int localIdx = dialogueIdx;
                    GameObject selectBox = Instantiate(selectBoxPrefab, optionalDialogueCanvas.transform.GetChild(0).transform);
                    selectBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = dialogueList[localIdx].GetText();
                    selectBox.transform.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        string nextCommand = dialogueList[localIdx].GetCommand();
                        StartDialogue(nextCommand);
                    });

                    if (i != optionalCount - 1)
                        dialogueIdx++;
                }
                break;
            default:
                break;
        }

        if (dialogueType != "선택지2" && dialogueType != "선택지3" && currentDialogue.command != null)
        {
            CallNextCommand(currentDialogue.command);
        }
    }

    /// <summary>
    /// 모든 캔버스 Close
    /// </summary>
    void CloseAllCanvas()
    {
        touchInputCanvas.SetActive(false);
        optionalDialogueCanvas.SetActive(false);
        noticeDialogueCanvas.SetActive(false);
        conversationDialogueCanvas.SetActive(false);
    }

    /// <summary>
    /// 카테고리 단위로 다이얼로그 출력
    /// </summary>
    /// <param name="category"></param>
    public void StartDialogue(string category)
    {
        // 현재 다이얼로그의 인덱스를 0으로.
        dialogueIdx = 0;

        // 시작할 다이얼로그 불러오기.
        dialogueList = DatabaseManager.instance.Dialogues[category];

        // 대사를 토대로 코루틴 실행.
        isClicked = false;
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ContinueDialogueCoroutine());
    }

    /// <summary>
    /// 다이얼로그 중단
    /// </summary>
    /// <param name="category"></param>
    void EndDialogue()
    {
        // 해당 카테고리의 다이얼로그가 끝나면 코루틴 중단
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        // 모든 캔버스 닫기.
        CloseAllCanvas();
    }

    /// <summary>
    /// 터치 입력 대기 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator ContinueDialogueCoroutine()
    {
        while (dialogueIdx < dialogueList.Count)
        {
            CallNextDialogue();
            if (_textEffect != null)
            {
                while (_textEffect.isTyping && !isClicked)
                {
                    yield return null;
                }
                _textEffect.ShowFullText();
                isClicked = false;
            }

            // Wait Click Input
            while (!isClicked)
            {
                yield return null;
            }
            isClicked = false;
        }
        EndDialogue();
    }

    /// <summary>
    /// 현재 다이얼로그 리스트에서 다음 다이얼로그 출력
    /// </summary>
    void CallNextDialogue()
    {
        // 현재 인덱스에 해당되는 다이얼로그 저장.
        currentDialogue = dialogueList[dialogueIdx];
        ProcessingDialogue();

        // 다이얼로그 카운트 추가.
        dialogueIdx++;
    }

    /// <summary>
    /// Command 컬럼 데이터 처리
    /// </summary>
    /// <param name="str"></param>
    public void CallNextCommand(string str)
    {
        currentCoroutine = null;
        switch (str)
        {            
            case "Intro_1":
                StartDialogue("Intro_1");
                break;
            case "Intro_2":
                StartDialogue("Intro_2");
                break;            
        }
    }

    /// <summary>
    /// character 컬럼 값 받아서 캐릭터 스프라이트 반환
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    Sprite GetCharacterSprite(string character)
    {
        switch (character)
        {
            case "캐릭터1":
                return characterSpriteList[0];
            case "캐릭터2":
                return characterSpriteList[1];
        }
        return null;
    }
}