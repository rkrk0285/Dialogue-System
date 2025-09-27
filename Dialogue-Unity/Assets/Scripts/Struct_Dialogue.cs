public struct Struct_Dialogue
{
    public string category;    
    public string dialogue_type;    
    public string character;    
    public string text;    
    public string command;
    public int skipLine;
    public Struct_Dialogue(string _category, string _dialogue_type, string _character, string _text, string _command, int _skipLine) 
    { 
        category = _category;
        dialogue_type = _dialogue_type;
        character = _character;
        text = _text;
        command = _command;
        skipLine = _skipLine;
    }

    public string GetCategory() { return category; }
    public readonly string GetDialogueType() { return dialogue_type; }
    public string GetCharacter() { return character; }
    public string GetText() { return text; }
    public string GetCommand() { return command == "" ? null : command; }
    public int GetSkipLine() { return skipLine; }
}