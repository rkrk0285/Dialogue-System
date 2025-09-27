using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance = null;

    List<Dictionary<string, object>> Texts;
    public Dictionary<string, List<Struct_Dialogue>> Dialogues;
    string CSV_FILE_NAME = "TextCSV";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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
            string dialogue_type = texts[i]["dialogue_type"].ToString();
            string character = RefineText(texts[i]["character"].ToString());
            string text = RefineText(texts[i]["text"].ToString());
            string command = texts[i]["command"].ToString();
            int skipLine = 0;
            int.TryParse(texts[i]["skipLine"].ToString(), out skipLine);

            Struct_Dialogue tempDialogue = new Struct_Dialogue(
                category, dialogue_type, character, text, command, skipLine);

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