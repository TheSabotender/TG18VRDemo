using System;

[AttributeUsage(AttributeTargets.Method)]
public class ExposeInEditorAttribute :  Attribute
{
    public bool showWarning = false;
    public bool allowInEditor = true;
    public bool allowWhilePlaying = true;
    public string buttonNameOverride = "";
    public ConsoleColor buttonColorOverride = ConsoleColor.White;

    /* Old
    public ExposeInEditorAttribute(bool warning = false, string name = "", bool allowInEditor = true, bool allowInPlay = true, ConsoleColor color = ConsoleColor.White)
    {
        warn = warning;
        allowEdit = allowInEditor;
        allowPlay = allowInPlay;
        nameOverride = name;
        colorOver = color;
    }
    */
}

