
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


#if UNITY_EDITOR || UNITY_5_0_2 || UNITY_5_0 || UNITY_5 || UNITY_IPHONE || UNITY_ANDROID
//using Framework.UI;
#endif

//using CsvHelper;
//using System.Reflection;



#if UNITY_EDITOR || UNITY_5_0_2 || UNITY_5_0 || UNITY_5 || UNITY_IPHONE || UNITY_ANDROID
    using UnityEngine;
#else
using System.Diagnostics;
#endif



public interface I_CSVMember
{
    int GetmemberCnt();
    //  void SetValue(string str);
    void SetValue(string[] vals, int startCol = 0);
};

public interface I_CSVTable
{
    void SetValue(string[] vals, int startCol = 0);

    void SetValue(string[] vals, Dictionary<string, int> dicCol);
    string GetKeyname();

    int GetIndex();
};


[Serializable]
public class BitFlag<T> : I_CSVMember where T : new()
{
    public int flag;

    public int GetmemberCnt() { return 1; }

    public void SetValue(string str)
    {

        if (!typeof(T).IsEnum && !typeof(T).IsPrimitive)
            return;

        string[] vals = CSVTableHelper.AtomicArrayParsing(str);
        T[] t = new T[vals.Length];
        for (int i = 0; i < t.Length; ++i)
        {
            CSVTableHelper.SetValue(ref t[i], vals[i]);
            object o = t[i];
            flag |= (1 << (int)o);
        }

    }

    public void SetValue(string[] vals, int startCol = 0)
    {
        T[] t = new T[vals.Length];
        for (int i = startCol; i < t.Length; ++i)
        {
            CSVTableHelper.SetValue(ref t[i], vals[i]);
            object o = t[i];
            flag |= (1 << (int)o);
        }

    }

}
/*
public class TableWrapper<T> where T : class, new()
{
    public T[] tables;
    public Dictionary<string, T> tableDic = new Dictionary<string, T>();
    public bool Load(string strPath)
    {
        return CSVTableHelper.LoadCSV(strPath, ref tables, ref tableDic);
    }


    public int GetLength()
    {
        return tables.Length;
    }


    public T this[string key]
    {
        get
        {
            T rec;
            if (!tableDic.TryGetValue(key, out rec))
                return null;

            return rec;
        }
    }


    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= tables.Length)
            {
                return null;
            }
            else
            {

                return tables[index];
            }
        }

    }
}
*/



public class CSVTableHelper
{
    public const string TABLE_DEFINATION = "<table_define>";
    public const string TYPE_DEFINATION = "<type_define>";
    public const string DATA_DEFINATION = "<data_define>";
    public const string KEY_COLUMN = "key_column";

    public static int nCurRow = 0;
    public static StringBuilder sbErr = new StringBuilder(100, 512);

    public static void ErrorMsg(string str)
    {





#if UNITY_EDITOR || UNITY_5_0_2 || UNITY_5_0 || UNITY_5 || UNITY_IPHONE || UNITY_ANDROID

        sbErr.Length = 0;
        sbErr.Append("CSVTable Value Error Row:"+nCurRow+" Column:"+str+"\n \0");
        Debug.LogError(sbErr);
#else

        sbErr.Clear();
        sbErr.Append("CSVTable Value Error Row:" + nCurRow + " Column:" + str + "\n");
        Debug.WriteLine(sbErr.ToString());
#endif

    }

    public static string[] ClassArrayParsing(string str)
    {
        char[] sep = { ';', '{', '}' };
        string[] vals = str.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        return vals;
    }


    public static string[] AtomicArrayParsing(string str)
    {
        char[] sep = { ';', '{', '}' };
        string[] vals = str.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        return vals;
    }


    public static void SetValue<T>(ref T t, string str) where T : new()
    {
        //  if(str == null || str.Length < 1)
        //     return;

        if (typeof(T).IsEnum)
        {
            object b = Enum.Parse(typeof(T), str);
            t = (T)b;
            return;

/*          Array a = Enum.GetValues(typeof(T));
            int count = 0;
            foreach (T o in a)
            {
                int strint = 0;

                int.TryParse(str, out strint);
                //if (str.Contains(o.ToString()))
                if (strint == count)
                {
                    t = o;
                    return;
                }
                count++;
            }
            return;*/
        }

        t = new T();
        I_CSVMember w = (I_CSVMember)t;


        char[] sep = { ';', '{', '}' };
        string[] vals = str.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        if (vals.Length < w.GetmemberCnt())
            return;



        if (vals.Length % w.GetmemberCnt() > 0)
        {
            ErrorMsg(str);
            return;
        }

        w.SetValue(vals);

    }


    public static void SetValue<T>(ref T[] t, string str) where T : new()
    {
        //  if(str==null||str.Length<1)
        //      return;

        if (typeof(T).IsEnum)
        {
            string[] vals = AtomicArrayParsing(str);
            t = new T[vals.Length];
            for (int i = 0; i < t.Length; ++i)
            {
                SetValue(ref t[i], vals[i]);
            }
            return;
        }


        {
            string[] vals = ClassArrayParsing(str);

            I_CSVMember w = (I_CSVMember)new T();
            t = new T[vals.Length / w.GetmemberCnt()];

            if (vals.Length < w.GetmemberCnt())
                return;


            if (vals.Length % w.GetmemberCnt() > 0)
            {
                ErrorMsg(str);
                return;
            }

            for (int i = 0; i < t.Length; ++i)
            {
                t[i] = new T();
                w = (I_CSVMember)t[i];
                w.SetValue(vals, i * w.GetmemberCnt());
            }
        }
    }


    public static void SetValue(ref bool n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        try
        {
            n = Convert.ToBoolean(str);
        }
        catch(Exception e)
        {
            string err = str + "," + e.Message;
            ErrorMsg(err);
        }
    }

    public static void SetValue(ref int n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        try
        {
            n = Convert.ToInt32(str);
        }
        catch (Exception e)
        {
            string err = str + "," + e.Message;
            ErrorMsg(err);
        }
    }

    public static void SetValue(ref uint n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        try
        {
            n = Convert.ToUInt32(str);
        }
        catch (Exception e)
        {
            string err = str + "," + e.Message;
            ErrorMsg(err);
        }
    }

    public static void SetValue(ref short n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        try
        {
            n = Convert.ToInt16(str);
        }
        catch (Exception e)
        {
            string err = str + "," + e.Message;
            ErrorMsg(err);
        }
    }

    public static void SetValue(ref sbyte n, string str)
    {
        if (str == null || str.Length < 1)
            return;
        try
        {
            n = Convert.ToSByte(str);

        }
        catch (Exception e)
        {
            string err = str + "," + e.Message;
            ErrorMsg(err);
        }
    }

    public static void SetValue(ref byte n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        try
        {
            n = Convert.ToByte(str);
        }
        catch (Exception e)
        {
            string err = str + "," + e.Message;
            ErrorMsg(err);
        }
    }

    public static void SetValue(ref float n, string str)
    {
        if (str == null || str.Length < 1)
            return;
        try
        {
            n = Convert.ToSingle(str);
        }
        catch (Exception e)
        {
            string err = str + "," + e.Message;
            ErrorMsg(err);
        }
    }

    public static void SetValue(ref string n, string str)
    {
        if (str == null || str.Length < 1 || str.CompareTo("NONE") == 0)
        {
            n = String.Empty;
            return;
        }
        n = str;
    }

    public static void SetValue(ref bool[] n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        string[] vals = AtomicArrayParsing(str);

        n = new bool[vals.Length];

        for(int i = 0; i < n.Length; ++i)
        {
            SetValue(ref n[i], vals[i]);
        }
    }

    public static void SetValue(ref int[] n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        string[] vals = AtomicArrayParsing(str);

        n = new int[vals.Length];

        for (int i = 0; i < n.Length; ++i)
        {
            SetValue(ref n[i], vals[i]);
        }
    }

    public static void SetValue(ref short[] n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        string[] vals = AtomicArrayParsing(str);

        n = new short[vals.Length];

        for(int i = 0; i < n.Length; ++i)
        {
            SetValue(ref n[i], vals[i]);
        }
    }

    public static void SetValue(ref sbyte[] n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        string[] vals = AtomicArrayParsing(str);

        n = new sbyte[vals.Length];

        for (int i = 0; i < n.Length; ++i)
        {
            SetValue(ref n[i], vals[i]);

        }
    }

    public static void SetValue(ref byte[] n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        string[] vals = AtomicArrayParsing(str);

        n = new byte[vals.Length];

        for (int i = 0; i < n.Length; ++i)
        {
            SetValue(ref n[i], vals[i]);

        }
    }

    public static void SetValue(ref float[] n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        string[] vals = AtomicArrayParsing(str);

        n = new float[vals.Length];

        for (int i = 0; i < n.Length; ++i)
        {
            SetValue(ref n[i], vals[i]);

        }
    }


    public static void SetValue(ref string[] n, string str)
    {
        if (str == null || str.Length < 1)
            return;

        string[] vals = AtomicArrayParsing(str);

        n = new string[vals.Length];

        for (int i = 0; i < n.Length; ++i)
        {
            SetValue(ref n[i], vals[i]);

        }
    }





    public static void SetBitFlag<T>(ref int flag, string str) where T : new()
    {
        if (str == null || str.Length < 1)
            return;


        if (!typeof(T).IsEnum && !typeof(T).IsPrimitive)
            return;

        string[] vals = AtomicArrayParsing(str);
        T[] t = new T[vals.Length];
        for (int i = 0; i < t.Length; ++i)
        {
            SetValue(ref t[i], vals[i]);
            object o = t[i];
            flag |= (1 << (int)o);
        }


    }


    public static string GetColString(string colname, string[] cols, Dictionary<string, int> dicCol)
    {
        int iCol;

        if (!dicCol.TryGetValue(colname, out iCol))
        {

#if UNITY_EDITOR || UNITY_5_0_2 || UNITY_5_0 || UNITY_5 || UNITY_IPHONE || UNITY_ANDROID
            Debug.LogError("Column Name Error:" + colname);
#endif
            return string.Empty;
        }

        return cols[iCol];

    }

    /*
    public static bool LoadCSV<T>(string strPath, ref T[] data, ref Dictionary<string, T> dataDic) where T : new()
    {

        // Encoding mSourceEncoding=Encoding.GetEncoding("ks_c_5601-1987");

        Encoding mSourceEncoding = Encoding.Unicode;


#if UNITY_ANDROID


        //TextAsset tx = (TextAsset)Resources.Load(strPath.Replace(".txt",""));
        //StreamReader Sr = new StreamReader(strPath, Encoding.Default, true);

        if (File.Exists(strPath) == false)
        {
            Debug.LogError(strPath + "is not find");
        }

        FileStream Fs = null;
        try
        {
            Fs = new FileStream(strPath, FileMode.Open, FileAccess.Read);
        }
        catch(Exception e)
        {
            //UIMgr.Instance.OnUIEvent((uint)UIEvent.TABLE_READ_FAIL, strPath);
            Debug.LogError(e);
        }
        
        MemoryStream memoryStream = new MemoryStream();
        
        memoryStream.SetLength(Fs.Length);
        Fs.Read(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);        
        memoryStream.Position = 0;

        StreamReader sr = new StreamReader(memoryStream);


        using(CsvParser csvParser=new CsvParser(sr))
        {
#else 
        using (CsvParser csvParser = new CsvParser(new StreamReader(new FileStream(strPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), mSourceEncoding)))
        {
#endif

            bool bDataFieldFind = false;

            int dataIndex = 0, maxIndex = 0;
            nCurRow = 0;

            csvParser.Configuration.Delimiter = "\t";

            List<T> tempList = new List<T>();

            Dictionary<string, int> dicCol = new Dictionary<string, int>();

            while (true)
            {
                string[] columns = csvParser.Read();

                nCurRow = csvParser.Row;

                if (columns == null || columns.Length == 0)
                {
                    //data=tempList.ToArray();

                    data = new T[maxIndex + 1];

                    foreach (T t in tempList)
                    {
                        I_CSVTable csvTbl = (I_CSVTable)t;
                        data[csvTbl.GetIndex()] = t;
                    }

                    break;
                }

                if (bDataFieldFind == true)
                {
                    T t = new T();
                    tempList.Add(t);
                    I_CSVTable table = (I_CSVTable)t;
                    columns[0] = "";
                    //table.SetValue(columns, 1);

                    table.SetValue(columns, dicCol);

                    int idx = new int();
                    CSVTableHelper.SetValue(ref idx, columns[1]);

                    if (idx > maxIndex)
                        maxIndex = idx;


                    if (columns != null && columns[2] != null && columns[2].Length > 0)
                    {
                        dataDic.Add(columns[2], t);
                    }
                    ++dataIndex;
                }

                switch (columns[1].Trim())
                {
                    case "<data_define>":
                        {
                            bDataFieldFind = true;
                            columns = csvParser.Read();       ///변수명

                            for(int jj=1; jj < columns.Length;++jj)
                            {
                                if (!string.IsNullOrEmpty(columns[jj]))
                                   dicCol[columns[jj]] = jj;
                            }

                            csvParser.Read();       ///타입
                            csvParser.Read();       ///설명

                        }
                        break;
                }


            }
        }

        return true;


    }
    */
}



