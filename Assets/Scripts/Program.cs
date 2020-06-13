using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
//#if !UNITY_EDITOR
//#endif

public class Program : MonoBehaviour
{
    const string KEY_BOOK = "KEY_BOOK";
    const string KEY_CHAPTER = "KEY_CHAPTER";
    const string KEY_VERSE = "KEY_VERSE";

    const string DB_NAME = "korHRV.db";
    const string TABLE_NAME = "bible_korHRV";

    public AudioSource audioSource;

    public Text text_book;
    public InputField if_test;
    public InputField if_answer;
    public Button[] buttonAnswers;
    public Transform panelBooks;
    public GameObject buttonBook;

    //Bible bible;
    private SQLiteConnection _connection;

    int[] count = new int[50];

    int p_book = 0;
    int p_chapter = 0;
    int p_verse = 0;
    int p_word = 0;

    int[] saved_chapter = new int[66];
    int[] saved_verse = new int[66];

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

    void StoreBibleData()
    {
        //Debug.Log(string.Format("StoreBibltData {0}:{1}", p_chapter, p_verse));
        PlayerPrefs.SetInt(KEY_BOOK, p_book);
        saved_chapter[p_book] = p_chapter;
        saved_verse[p_book] = p_verse;
        for (int i=0; i<66; i++)
        {
            PlayerPrefs.SetInt(KEY_CHAPTER+i.ToString(), saved_chapter[i]);
            PlayerPrefs.SetInt(KEY_VERSE+i.ToString(), saved_verse[i]);
            //Debug.Log(string.Format("StoreBibltData {0} {1}:{2}", i, saved_chapter[i], saved_verse[1]));
        }
    }

    void LoadBibltData()
    {
        p_book = PlayerPrefs.GetInt(KEY_BOOK, 18);
        for (int i=0; i<66; i++)
        {
            saved_chapter[i] = PlayerPrefs.GetInt(KEY_CHAPTER+i.ToString(), 0);
            saved_verse[i] = PlayerPrefs.GetInt(KEY_VERSE+i.ToString(), 0);
            //Debug.Log(string.Format("LoadBibltData {0} {1}:{2}", i, saved_chapter[i], saved_verse[1]));
        }
        p_chapter = saved_chapter[p_book];
        p_verse = saved_verse[p_book];
    }

    int GetBookIndex(string book_name)
    {
        for (int i=0; i<66; i++)
        {
            if (BIBLE_DEF.BOOK_NAME[i] == book_name)
            {
                return i;
            }
            if (BIBLE_DEF.BOOK_NAME_SHORT[i] == book_name)
            {
                return i;
            }
        }
        return -1;
    }

    void SelectBook(Button button)
    {
        StoreBibleData();
        string book_name = button.GetComponentInChildren<Text>().text;
        Debug.Log("SelectBook" + book_name + GetBookIndex(book_name));
        panelBooks.gameObject.SetActive(false);
        p_book = GetBookIndex(book_name);
        p_chapter = saved_chapter[p_book];
        p_verse = saved_verse[p_book];
        RefreshUI();
    }

    void MakeBookButtons()
    {
        int x_origin = -340;
        int y_origin = 620;
        int x_step = 115;
        int y_step = -115;
        Debug.Log("MakeBookButtons");
        int nBook = 0;
        for (int i=0; i< 11; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                GameObject go = Instantiate(buttonBook);
                Button btnBook = go.GetComponentInChildren<Button>();
                btnBook.onClick.AddListener(() => { SelectBook(btnBook); });
                Text btnBookText = go.GetComponentInChildren<Text>();
                btnBookText.text = BIBLE_DEF.BOOK_NAME_SHORT[nBook];
                nBook++;
                go.transform.SetParent(panelBooks, false);
                int x_pos = x_origin + (j * x_step);
                int y_pos = y_origin + (i * y_step);
                //Debug.Log(string.Format("PosZ{0},{1} => {2},{3}", i, j, x_pos, y_pos));
                go.transform.localPosition =  new Vector3(x_pos, y_pos, 0);
            }
        }
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
            //Debug.Log("Compare " + word + GetContent(p_book, p_chapter, p_verse));
            if (word == GetContentWord(p_book, p_chapter, p_verse, p_word))
            {
                //Debug.Log("Correct");
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
                RefreshUI();
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

    bool NextWord()
    {
        p_word = p_word + 1;
        //Debug.Log("B_Count:" + bible.books.Count);
        //Debug.Log("C_Count:" + bible.books[p_book].chapters.Count);
        //Debug.Log("V_Count:" + bible.books[p_book].chapters[p_chapter].verses.Count);
        //Debug.Log("W_Count:" + bible.books[p_book].chapters[p_chapter].verses[p_verse].words.Length);
        //Debug.Log("B: " + p_book + ", C: " + p_chapter + ", V: " + p_verse + ", W: " + p_word);
        if (p_word >= GetContentWords(p_book, p_chapter, p_verse).Length)
        {
            p_word = 0;
            NextVerse();
            return false;
        }
        return true;
    }
    void NextVerse()
    {
        Debug.Log("NextVerse");
        p_verse = p_verse + 1;
        if (GetContent(p_book, p_chapter, p_verse) == "")
        {
            p_verse = 0;
            NextChapter();
        }
        StoreBibleData();
    }
    void NextChapter()
    {
        Debug.Log("NextChapter");
        p_chapter = p_chapter + 1;
        if (GetContent(p_book, p_chapter, p_verse) == "")
        {
            // NextBook
            p_chapter = 0;
            NextBook();
        }
        StoreBibleData();
    }
    void NextBook()
    {
        p_book = p_book + 1;
        if (GetContent(p_book, p_chapter, p_verse) == "")
        {
            p_book = 0;
        }
        StoreBibleData();
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
        if (w+n < GetContentWords(b, c, v).Length)
        {
            result = GetContentWords(b, c, v)[w + n];
        }
        else
        {
            w = (w + n) - GetContentWords(b, c, v).Length;
            v++;
            if (GetContent(b, c, v) == "")
            {
                v = 1;
                c++;
                if (GetContent(b, c, v) == "")
                {
                    c = 1;
                    b++;
                    if (b > 66)
                    {
                        b = 1;
                        c = 1;
                        v = 1;
                    }
                }
            }
            result = GetContentWord(b, c, v, w);
        }
        return result;
    }

    void RefreshUI()
    {
        try
        {
            text_book.text = BIBLE_DEF.BOOK_NAME[p_book] + " " + (p_chapter + 1).ToString() + ":" + (p_verse + 1).ToString();
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
            for (int i = 0; i < 4; i++)
            {
                //Debug.Log(string.Format("{0} Words: {1}", i, string.Join(", ", rest_words)));
                if (buttonAnswers[i].GetComponentInChildren<Text>().text == words[0])
                {
                    rest_words.Remove(words[0]);
                    words[0] = "";
                }
                else if (buttonAnswers[i].GetComponentInChildren<Text>().text == words[1])
                {
                    rest_words.Remove(words[1]);
                    words[1] = "";
                }
                else
                {
                    buttonAnswers[i].GetComponentInChildren<Text>().text = rest_words[0];
                    rest_words.RemoveAt(0);
                }
            }
            if_test.text = GetContent(p_book, p_chapter, p_verse);
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

    //void LoadDatabase(string DatabaseName)
    //{

    //    var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
    //    if (!File.Exists(dbPath))
    //    {
    //        dbPath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);
    //    }
    //    //string dbPath = "file://" + Application.streamingAssetsPath + "/" + DatabaseName;
    //    try
    //    {
    //        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadOnly);
    //        Debug.Log("Final PATH: " + dbPath);
    //        Debug.Log("Connection: " + _connection.ToString());
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log(e);
    //    }
    //}

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


    public class Bible_korHRV
    {
        [PrimaryKey, AutoIncrement]
        public int book { get; set; }
        public int chapter { get; set; }
        public int verse { get; set; }
        public string content { get; set; }
    }


    string GetContent(int nBook, int nChapter, int nVerse)
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
        catch(Exception e)
        {
            Debug.Log(e);
        }
        return result;
    }

    string[] GetContentWords(int nBook, int nChapter, int nVerse)
    {
        string content = GetContent(nBook, nChapter, nVerse);
        if (content.Length > 0)
        {
            return content.Split(' ');
        }
        return new string[] { "", "", "", "" };
    }

    string GetContentWord(int nBook, int nChapter, int nVerse, int nWord)
    {
        string content = GetContent(nBook, nChapter, nVerse);
        if (content.Length > 0)
        {
            return content.Split(' ')[nWord];
        }
        return "";
    }

    // Start is called before the first frame update
    void Start()
    {
        MakeBookButtons();

        DataService(DB_NAME);

        LoadBibltData();

        //p_book = 18;
        //p_chapter = 0;
        //p_verse = 0;

        RefreshUI();

        //StartCoroutine("TestButton");
    }

    // Update is called once per frame
    void Update()
    {
        TimeSpan timeSpan = System.DateTime.Now - btnBlink;
        //Debug.Log(timeSpan.TotalMilliseconds);
        if (timeSpan.TotalMilliseconds > 100)
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
            StoreBibleData();
        }
    }
}
