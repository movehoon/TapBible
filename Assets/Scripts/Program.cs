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
    const string KEY_BOOKMARK = "KEY_BOOKMARK";

    public AudioSource audioSource;

    public Text text_book;
    public Text text_process;
    public Button[] buttonAnswers;
    public Transform panelBooks;
    public GameObject buttonBook;

    public BibleManager bm;
    Bookmark bookmark = new Bookmark();

    int[] count = new int[50];

    int p_book = 0;
    int p_chapter = 0;
    int p_verse = 0;
    int p_word = 0;

    //int[] saved_chapter = new int[66];
    //int[] saved_verse = new int[66];

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
        bookmark.p_book = p_book;
        bookmark.saved_chapter[p_book] = p_chapter;
        bookmark.saved_verse[p_book] = p_verse;
        string save_pointer = JsonUtility.ToJson(bookmark);
        PlayerPrefs.SetString(KEY_BOOKMARK, save_pointer);
        Debug.Log("StoreBiblePointer:" + save_pointer);
    }

    void LoadBibleData()
    {
        string saved_pointer = PlayerPrefs.GetString(KEY_BOOKMARK);
        Debug.Log("LoadBiblePointer:" + saved_pointer);
        bookmark = JsonUtility.FromJson<Bookmark>(saved_pointer);
        if (bookmark == null)
        {
            bookmark = new Bookmark();
        }
        p_book = bookmark.p_book;
        p_chapter = bookmark.saved_chapter[p_book];
        p_verse = bookmark.saved_verse[p_book];
    }

    //void StoreBibleData()
    //{
    //    //Debug.Log(string.Format("StoreBibltData {0}:{1}", p_chapter, p_verse));
    //    PlayerPrefs.SetInt(KEY_BOOK, p_book);
    //    saved_chapter[p_book] = p_chapter;
    //    saved_verse[p_book] = p_verse;
    //    for (int i=0; i<66; i++)
    //    {
    //        PlayerPrefs.SetInt(KEY_CHAPTER+i.ToString(), saved_chapter[i]);
    //        PlayerPrefs.SetInt(KEY_VERSE+i.ToString(), saved_verse[i]);
    //        //Debug.Log(string.Format("StoreBibltData {0} {1}:{2}", i, saved_chapter[i], saved_verse[1]));
    //    }
    //}

    //void LoadBibltData()
    //{
    //    p_book = PlayerPrefs.GetInt(KEY_BOOK, 18);
    //    for (int i=0; i<66; i++)
    //    {
    //        saved_chapter[i] = PlayerPrefs.GetInt(KEY_CHAPTER+i.ToString(), 0);
    //        saved_verse[i] = PlayerPrefs.GetInt(KEY_VERSE+i.ToString(), 0);
    //        //Debug.Log(string.Format("LoadBibltData {0} {1}:{2}", i, saved_chapter[i], saved_verse[1]));
    //    }
    //    p_chapter = saved_chapter[p_book];
    //    p_verse = saved_verse[p_book];
    //}

    int GetBookIndex(string book_name)
    {
        for (int i=0; i<BIBLE_DEF.BOOK_MAX; i++)
        {
            if (book_name.Contains(BIBLE_DEF.BOOK_NAME[i]))
            {
                return i;
            }
            if (book_name.Contains(BIBLE_DEF.BOOK_NAME_SHORT[i]))
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
        p_chapter = bookmark.saved_chapter[p_book];
        p_verse = bookmark.saved_verse[p_book];
        RefreshUI();
    }

    void CreateBookButtons()
    {
        int x_origin = -340;
        int y_origin = 620;
        int x_step = 115;
        int y_step = -115;
        Debug.Log("CreateBookButtons");
        int nBook = 0;
        for (int i=0; i< 11; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                GameObject go = Instantiate(buttonBook);
                Button btnBook = go.GetComponentInChildren<Button>();
                btnBook.onClick.AddListener(() => { SelectBook(btnBook); });
                Text btnBookText = go.GetComponentInChildren<Text>();
                if (nBook < 39) {
                    btnBookText.text = "<color=#0000aaff>" + BIBLE_DEF.BOOK_NAME_SHORT[nBook] + "</color>";
                }
                else
                {
                    btnBookText.text = "<color=#aa0000ff>" + BIBLE_DEF.BOOK_NAME_SHORT[nBook] + "</color>";
                }
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

    string GetProcessing(string[] words, int word_index)
    {
        string result = "";
        for(int i=0; i<words.Length; i++)
        {
            if (i == word_index)
            {
                result += "<b><color=#008000ff>" + words[i] + "</color></b> ";  
            }
            else
            {
                result += words[i] + " ";
            }
        }
        return result;
    }

    bool MatchWord(string word)
    {
        try
        {
            //Debug.Log("Compare " + word + GetContent(p_book, p_chapter, p_verse));
            if (word == bm.GetContentWord(p_book, p_chapter, p_verse, p_word))
            {
                //Debug.Log("Correct");
                if (NextWord())
                {
                    StartCoroutine("PlayCorrect");
                }
                else
                {
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
        if (p_word >= bm.GetContentWords(p_book, p_chapter, p_verse).Length)
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
        if (bm.GetContent(p_book, p_chapter, p_verse) == "")
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
        if (bm.GetContent(p_book, p_chapter, p_verse) == "")
        {
            // NextBook
            p_chapter = 0;
            bookmark.read_count[p_book]++;
            NextBook();
        }
        StoreBibleData();
    }
    void NextBook()
    {
        p_book = p_book + 1;
        if (bm.GetContent(p_book, p_chapter, p_verse) == "")
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
        if (w+n < bm.GetContentWords(b, c, v).Length)
        {
            result = bm.GetContentWords(b, c, v)[w + n];
        }
        else
        {
            w = (w + n) - bm.GetContentWords(b, c, v).Length;
            v++;
            if (bm.GetContent(b, c, v) == "")
            {
                v = 0;
                c++;
                if (bm.GetContent(b, c, v) == "")
                {
                    c = 0;
                    b++;
                    if (b > 66)
                    {
                        b = 0;
                        c = 0;
                        v = 0;
                    }
                }
            }
            result = bm.GetContentWord(b, c, v, w);
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
            text_process.text = GetProcessing(bm.GetContentWords(p_book, p_chapter, p_verse), p_word);
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

    // Start is called before the first frame update
    void Start()
    {
        CreateBookButtons();

        LoadBibleData();

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
