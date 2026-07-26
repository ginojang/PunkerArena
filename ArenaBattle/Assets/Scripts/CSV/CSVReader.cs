using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };
	
	public static List<Dictionary<string, string>> Read(string file)
	{
        var list = new List<Dictionary<string, string>>();

        string filePath = $"CSV/{file}";
        TextAsset data = Resources.Load(filePath) as TextAsset;

        if (data == null)
        {
            Debug.Log($"{file} is null");
            return null;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, string>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                value = value.Replace("<br>", "\n"); // 추가된 부분. 개행문자를 \n대신 <br>로 사용한다.
                value = value.Replace("<c>", ",");
                value = value.Replace("NONE", string.Empty);

                entry[header[j]] = value; //finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }

    public static List<Dictionary<string, string>> ReadData(string data)
    {
        var list = new List<Dictionary<string, string>>();

        var lines = Regex.Split(data, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, string>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                header[j] = header[j].Replace("\uFEFF", "");
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                value = value.Replace("\uFEFF", "");
                value = value.Replace("<br>", "\n"); // 추가된 부분. 개행문자를 \n대신 <br>로 사용한다.
                value = value.Replace("<c>", ",");
                value = value.Replace("NONE", string.Empty);

                entry.Add(header[j], value);
                //entry[header[j]] = value; //finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }

    public static List<string> ReadStringFilter(string file)
	{
		var list = new List<string>();
		string filePath = string.Format("Filter/{0}", file);
		TextAsset data = Resources.Load(filePath) as TextAsset;

		var lines = Regex.Split(data.text, LINE_SPLIT_RE);
		if (lines.Length < 1) return list;

		for (int i = 0; i < lines.Length; ++i)
		{
			var values = Regex.Split(lines[i], SPLIT_RE);
			if (values.Length == 0) continue;

			for (int j = 0; j < values.Length; ++j)
			{
				string value = values[j];
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

				list.Add(value);
			}
		}

		return list;
	}
}
