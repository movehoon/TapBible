using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BibleManager : MonoBehaviour
{
    const string DB_NAME = "korHRV.db";
    const string TABLE_NAME = "bible_korHRV";

    //Bible bible;
    private SQLiteConnection _connection;


    public class Bible_korHRV
    {
        [PrimaryKey, AutoIncrement]
        public int book { get; set; }
        public int chapter { get; set; }
        public int verse { get; set; }
        public string content { get; set; }
    }


    public string GetContent(int nBook, int nChapter, int nVerse)
    {
        nBook += 1;
        nChapter += 1;
        nVerse += 1;
        string result = "";
        try
        {
            //Debug.Log(string.Format("GetContent: {0} {1} {2}", nBook, nChapter, nVerse));
            Bible_korHRV b = _connection.Table<Bible_korHRV>().Where(x => x.book == nBook && x.chapter == nChapter && x.verse == nVerse).First();
            result = b.content;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return result;
    }

    public string[] GetContentWords(int nBook, int nChapter, int nVerse)
    {
        string content = GetContent(nBook, nChapter, nVerse);
        if (content.Length > 0)
        {
            return content.Split(' ');
        }
        return new string[] { "", "", "", "" };
    }

    public string GetContentWord(int nBook, int nChapter, int nVerse, int nWord)
    {
        string content = GetContent(nBook, nChapter, nVerse);
        if (content.Length > 0)
        {
            return content.Split(' ')[nWord];
        }
        return "";
    }

    public void DataService(string DatabaseName)
    {

#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
        var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
        // then save to Application.persistentDataPath
        File.Copy(loadDb, filepath);
        
#elif UNITY_STANDALONE_OSX
        var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
        // then save to Application.persistentDataPath
        File.Copy(loadDb, filepath);
#else
    var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
    // then save to Application.persistentDataPath
    File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);

    }

    private void Awake()
    {
        DataService(DB_NAME);
    }
}
