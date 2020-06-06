﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SQLite4Unity3d;
#if !UNITY_EDITOR
using System.IO;
#endif

public class Verse
{
    public string[] words;
}
public class Chapter
{
    public List<Verse> verses;
}
public class Book
{
    public List<Chapter> chapters;
}
public class Bible
{
    public List<Book> books;
}

public class Program : MonoBehaviour
{
    const string KEY_BOOK = "KEY_BOOK";
    const string KEY_CHAPTER = "KEY_CHAPTER";
    const string KEY_VERSE = "KEY_VERSE";

    public AudioSource audioSource;

    public Text text_book;
    public InputField if_test;
    public InputField if_answer;
    public Button[] buttonAnswers;

    Bible bible;
    private SQLiteConnection _connection;

    int[] count = new int[50];

    int p_book = 0;
    int p_chapter = 0;
    int p_verse = 30;
    int p_word = 0;

    DateTime btnBlink;
    DateTime answerBlink;

    IEnumerator PlayCorrect()
    {
        var audioClip = Resources.Load<AudioClip>("Audio/click_correct");
        audioSource.clip = audioClip;
        audioSource.Play();
        yield return null;
    }
    IEnumerator PlayWrong()
    {
        var audioClip = Resources.Load<AudioClip>("Audio/click_wrong");
        audioSource.clip = audioClip;
        audioSource.Play();
        yield return null;
    }
    IEnumerator PlayNext()
    {
        var audioClip = Resources.Load<AudioClip>("Audio/next");
        audioSource.clip = audioClip;
        audioSource.Play();
        yield return null;
    }

    public void Button1()
    {
        if (MatchWord(buttonAnswers[0].GetComponentInChildren<Text>().text))
        {
            buttonAnswers[0].GetComponentInChildren<Image>().color = Color.green;
        }
        else
        {
            buttonAnswers[0].GetComponentInChildren<Image>().color = Color.red;
        }
        btnBlink = DateTime.Now;
    }

    public void Button2()
    {
        if (MatchWord(buttonAnswers[1].GetComponentInChildren<Text>().text))
        {
            buttonAnswers[1].GetComponentInChildren<Image>().color = Color.green;
        }
        else
        {
            buttonAnswers[1].GetComponentInChildren<Image>().color = Color.red;
        }
        btnBlink = DateTime.Now;
    }

    public void Button3()
    {
        if (MatchWord(buttonAnswers[2].GetComponentInChildren<Text>().text))
        {
            buttonAnswers[2].GetComponentInChildren<Image>().color = Color.green;
        }
        else
        {
            buttonAnswers[2].GetComponentInChildren<Image>().color = Color.red;
        }
        btnBlink = DateTime.Now;
    }

    public void Button4()
    {
        if (MatchWord(buttonAnswers[3].GetComponentInChildren<Text>().text))
        {
            buttonAnswers[3].GetComponentInChildren<Image>().color = Color.green;
        }
        else
        {
            buttonAnswers[3].GetComponentInChildren<Image>().color = Color.red;
        }
        btnBlink = DateTime.Now;
    }

    bool MatchWord(string word)
    {
        try
        {
            if (word == bible.books[p_book].chapters[p_chapter].verses[p_verse].words[p_word])
            {
                if (NextWord())
                {
                    if_answer.text += word + " ";
                    StartCoroutine("PlayCorrect");
                }
                else
                {
                    if_answer.text = "";
                    StartCoroutine("PlayNext");
                }
                RefreshButton();
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e, this);
        }

        StartCoroutine("PlayWrong");
        return false;
    }

    //BibleVerse GetVerseInfo(int b, int c, int v)
    //{
    //    foreach (BibleVerse bv in bibleVerse)
    //    {
    //        if (bv.book == b && bv.chapter == c && bv.verse == v)
    //        {
    //            return bv;
    //        }
    //    }
    //    return null;
    //}

    bool NextWord()
    {
        p_word = p_word + 1;
        //Debug.Log("B_Count:" + bible.books.Count);
        //Debug.Log("C_Count:" + bible.books[p_book].chapters.Count);
        //Debug.Log("V_Count:" + bible.books[p_book].chapters[p_chapter].verses.Count);
        //Debug.Log("W_Count:" + bible.books[p_book].chapters[p_chapter].verses[p_verse].words.Length);
        //Debug.Log("B: " + p_book + ", C: " + p_chapter + ", V: " + p_verse + ", W: " + p_word);
        if (p_word >= bible.books[p_book].chapters[p_chapter].verses[p_verse].words.Length)
        {
            NextVerse();
            p_word = 0;
            return false;
        }
        return true;
    }
    void NextVerse()
    {
        //Debug.Log("NextVerse");
        p_verse = p_verse + 1;
        if (p_verse >= bible.books[p_book].chapters[p_chapter].verses.Count)
        {
            NextChapter();
            p_verse = 0;
        }
    }
    void NextChapter()
    {
        //Debug.Log("NextChapter");
        p_chapter = p_chapter +1;
        if (p_chapter >= bible.books[p_book].chapters.Count)
        {
            // NextBook
            NextBook();
            p_chapter = 0;
        }
    }
    void NextBook()
    {
        p_book = p_book + 1;
        if (p_book >= bible.books.Count)
        {
            p_book = 0;
        }
    }

    string[] Get4Words(int b, int c, int v, int w)
    {
        string[] result = new string[] { "", "", "", "" };
        for (int i=0; i<4; i++)
        {
            string word = GetNextNthWord(b, c, v, w, i);
            result[i] = word;
        }
        return result;
    }

    string GetNextNthWord(int b, int c, int v, int w, int n)
    {
        string result = "";
        if (w+n < bible.books[b].chapters[c].verses[v].words.Length)
        {
            result = bible.books[b].chapters[c].verses[v].words[w + n];
        }
        else
        {
            w = (w + n) - bible.books[b].chapters[c].verses[v].words.Length;
            v++;
            if (v >= bible.books[b].chapters[c].verses.Count)
            {
                v = 0;
                c++;
                if (c >= bible.books[b].chapters.Count)
                {
                    c = 0;
                    b++;
                }
            }
            result = bible.books[b].chapters[c].verses[v].words[w];
        }
        return result;
    }

    string GetWord(int b, int c, int v, int w)
    {
        //Debug.Log("GetWord: " + c.ToString() + v.ToString() + w.ToString());
        try
        {
            return bible.books[b].chapters[c].verses[v].words[w];
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }
        return null;
    }

    void RefreshButton()
    {
        try
        {
            text_book.text = "창세기 " + (p_chapter + 1).ToString() + ":" + (p_verse + 1).ToString();
            string[] words = Get4Words(p_book, p_chapter, p_verse, p_word);
            List<string> rest_words = new List<string>();
            int nRand = new System.Random().Next(2);
            if (nRand > 0)
            {
                rest_words.Add(words[2]);
                rest_words.Add(words[3]);
                rest_words.Add(words[0]);
                rest_words.Add(words[1]);
            }
            else
            {
                rest_words.Add(words[3]);
                rest_words.Add(words[2]);
                rest_words.Add(words[0]);
                rest_words.Add(words[1]);
            }
            Debug.Log("Words: " + string.Join(", ", rest_words));
            for (int i = 0; i < 4; i++)
            {
                if (buttonAnswers[i].GetComponentInChildren<Text>().text == words[0])
                {
                    rest_words.Remove(words[0]);
                }
                else if (buttonAnswers[i].GetComponentInChildren<Text>().text == words[1])
                {
                    rest_words.Remove(words[1]);
                }
                else
                {
                    buttonAnswers[i].GetComponentInChildren<Text>().text = rest_words[0];
                    rest_words.RemoveAt(0);
                }
            }
            if_test.text = string.Join(" ", bible.books[p_book].chapters[p_chapter].verses[p_verse].words);
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }
    }

    IEnumerator TestButton()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.005f);
            int nBtn = new System.Random().Next(4);
            switch (nBtn)
            {
                case 0:
                    Button1();
                    break;
                case 1:
                    Button2();
                    break;
                case 2:
                    Button3();
                    break;
                case 3:
                    Button4();
                    break;
            }
        }
    }

    void LoadDatabase(string DatabaseName)
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
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadOnly);
            Debug.Log("Final PATH: " + dbPath);
        Debug.Log("Connection: " + _connection.ToString());

    }

    class bible_korHRV
    {
        public int book;
        public int chapter;
        public int verse;
        public string content;
    }

    IEnumerable<bible_korHRV> GetLines()
    {
        return _connection.Table<bible_korHRV>();
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadDatabase("hrv.db");
        //Debug.Log(_connection.GetTableInfo("bible_korHRV").Count);

        //IEnumerable<bible_korHRV> lines = GetLines();
        //Debug.Log("lines:" + lines.ToString());
        //foreach (bible_korHRV line in lines)
        //{
        //    Debug.Log(line.book.ToString() + line.chapter.ToString() + line.verse.ToString() + line.content);
        //}
        //Debug.Log(_connection.Table<bible_korHRV>().Where(x => x.book == 2).FirstOrDefault().content);

        p_book = PlayerPrefs.GetInt(KEY_BOOK, 0);
        p_chapter = PlayerPrefs.GetInt(KEY_CHAPTER, 0);
        p_verse = PlayerPrefs.GetInt(KEY_VERSE, 0);

        //p_book = 0;
        //p_chapter = 5;
        //p_verse = 17;


        TextAsset file = Resources.Load<TextAsset>("Bible/1genesis");
        Debug.Log(file.name);
        //Debug.Log(file.text);

        bible = new Bible();
        bible.books = new List<Book>();
        Book book = new Book();
        book.chapters = new List<Chapter>();

        var lines = file.text.Split('\n');
        int c = -1;
        Chapter chapter = new Chapter();
        chapter.verses = new List<Verse>();
        foreach (var line in lines)
        {
            Debug.Log("LINE: " + line);
            int sep = line.IndexOf(' ');
            if (sep > 1)
            {
                string info = line.Substring(0, sep);
                string ch = info.Split(':')[0];
                string ve = info.Split(':')[1];
                int iCh = int.Parse(ch) - 1;
                int iVe = int.Parse(ve) - 1;

                if (c != iCh)
                {
                    if (c != -1)
                    {
                        book.chapters.Add(chapter);
                        chapter = new Chapter();
                        chapter.verses = new List<Verse>();
                    }
                    c = iCh;
                }

                Verse verse = new Verse();
                verse.words = line.Substring(sep + 1).Split(' ');
                Debug.Log("Verse: " + verse.words[0]);
                chapter.verses.Add(verse);
            }
            else
            {
                Debug.Log("Err");
            }
        }
        bible.books.Add(book);
        foreach(Book _book in bible.books)
        {
            //Debug.Log("C: " + _book.chapters.ToString());
            foreach (Chapter _chapter in _book.chapters)
            {
                //Debug.Log("V: " + _chapter.verses.Count.ToString());
                foreach(Verse verse in chapter.verses)
                {
                    //Debug.Log("W: " + verse.words.Length.ToString());
                }
            }
        }
        RefreshButton();

        //StartCoroutine("TestButton");
    }

    // Update is called once per frame
    void Update()
    {
        TimeSpan timeSpan = System.DateTime.Now - btnBlink;
        //Debug.Log(timeSpan.TotalMilliseconds);
        if (timeSpan.TotalMilliseconds > 200)
        {
            for (int i = 0; i < 4; i++)
            {
                buttonAnswers[i].GetComponentInChildren<Image>().color = Color.white;
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefs.SetInt(KEY_BOOK, p_book);
            PlayerPrefs.SetInt(KEY_CHAPTER, p_chapter);
            PlayerPrefs.SetInt(KEY_VERSE, p_verse);
        }
    }
}
