using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDefTable
{
    virtual public void SetData(List<Dictionary<string, string>> csvTable) { }
    virtual public void SetData(List<Dictionary<string, string>> csvTable, CharacterClass type = CharacterClass.None) { }
    virtual public void SetData(List<Dictionary<string, string>> csvTable, CharacterTalent type = CharacterTalent.None) { }
}

public abstract class DefData
{
    public abstract int Index { get; }

    public abstract void Load(Dictionary<string, string> data);
}
