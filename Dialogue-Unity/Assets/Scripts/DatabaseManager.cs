using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance = null;

    List<Dictionary<string, object>> Texts;
    public Dictionary<string, List<Struct_Dialogue>> Dialogues;
    string CSV_FILE_NAME = "TextCSV";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        Texts = ReadData(CSV_FILE_NAME);
        Dialogues = RefineData(Texts);
    }

    protected List<Dictionary<string, object>> ReadData(string CSVName)
    {
        return CSV_Reader.Read(CSVName);
    }

    protected Dictionary<string, List<Struct_Dialogue>> RefineData(List<Dictionary<string, object>> texts)
    {
        Dictionary<string, List<Struct_Dialogue>> dialogueList = new Dictionary<string, List<Struct_Dialogue>>();

        // 쉼표 기준 슬라이싱
        for (int i = 0; i < texts.Count; i++)
        {
            string category = texts[i]["category"].ToString();

            Struct_Dialogue tempDialogue = new Struct_Dialogue(
                category,
                texts[i]["dialogue_type"].ToString(),
                RefineText(texts[i]["character"].ToString()),
                RefineText(texts[i]["text"].ToString()),
                texts[i]["command"].ToString());

            if (!dialogueList.ContainsKey(category))
            {
                List<Struct_Dialogue> tempDialogues = new List<Struct_Dialogue>();
                tempDialogues.Add(tempDialogue);
                dialogueList.Add(category, tempDialogues);
            }
            else
            {
                dialogueList[category].Add(tempDialogue);
            }
        }
        return dialogueList;
    }

    protected string RefineText(string tempString)
    {
        return tempString.Replace("@", ",").Replace("\\n", "\n")
                         .Replace("\"", "");
    }
}