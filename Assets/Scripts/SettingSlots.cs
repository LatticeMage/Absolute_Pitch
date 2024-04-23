using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSlots
{
    public string name;
    
    public enum NoteType
    {
        silence,
        single,
        multi,
        length
    };

    public NoteType type;

    private static string[] typeText =
    {
        "silence",
        "single",
        "multi",
        "length"
    };

    public List<int> ids;

    public static int GetTypeByName(string name)
    {
        for (int i = 0; i < typeText.Length; ++i)
            if (name == typeText[i])
                return i;
        return -1;
    }

    public static string GetNameByType(NoteType type)
    {
        return typeText[(int)type];
    }

    public SettingSlots()
    {
        ids = new List<int>();
    }
}
