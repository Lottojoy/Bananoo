using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyHintMap", menuName = "Typing/Key Hint Map (Image)")]
public class KeyHintMap : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public string keyChar;       // 1 ตัวอักษร หรือ "space","enter","tab"
        public Texture2D texture;    // รูปคำแนะนำปุ่ม
    }

    [SerializeField] private List<Entry> entries = new List<Entry>();
    private Dictionary<string, Entry> table;

    void OnEnable() => Build();

    void Build()
    {
        table = new Dictionary<string, Entry>();
        foreach (var e in entries)
        {
            if (string.IsNullOrEmpty(e.keyChar) || e.texture == null) continue;
            var k = e.keyChar.Trim().ToLowerInvariant();
            if (!table.ContainsKey(k)) table.Add(k, e);
        }
        AddAlias(" ", "space");
        AddAlias("\n", "enter");
        AddAlias("\r", "enter");
        AddAlias("\t", "tab");
    }

    void AddAlias(string from, string to)
    {
        if (table.ContainsKey(to) && !table.ContainsKey(from)) table.Add(from, table[to]);
    }

    void EnsureBuilt() { if (table == null) Build(); }

    public bool TryGet(char c, out Texture2D tex)
    {
        EnsureBuilt();
        tex = null;
        var k = c.ToString().ToLowerInvariant();
        if (table.TryGetValue(k, out var e)) { tex = e.texture; return true; }
        return false;
    }

    public bool TryGetSpecial(string key, out Texture2D tex)
    {
        EnsureBuilt();
        tex = null;
        key = (key ?? "").Trim().ToLowerInvariant();
        if (table.TryGetValue(key, out var e)) { tex = e.texture; return true; }
        return false;
    }
}
