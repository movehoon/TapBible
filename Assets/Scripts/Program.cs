using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public class BibleVerse
//{
//    public int book;
//    public int chapter;
//    public int verse;
//    public string line;
//    public string[] words;
//}

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

    //List<BibleVerse> bibleVerse = new List<BibleVerse>();
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

    string[] Shuffle(string[] strArray, string answer, int answer_index)
    {
        List<string> result = new List<string>();
        List<string> input = new List<string>();
        foreach (string str in strArray)
        {
            if (str != answer)
            {
                input.Add(str);
            }
        }
        //Debug.Log("input count: " + input.Count.ToString());
        for (int i=0; i<4; i++)
        {
            if (i == answer_index)
            {
                result.Add(answer);
            }
            else
            {
                int pick = new System.Random().Next(input.Count);
                result.Add(input[pick]);
                input.RemoveAt(pick);
            }
        }
        return result.ToArray();
    }

    void RefreshButton()
    {
        try
        {
            text_book.text = "창세기 " + (p_chapter + 1).ToString() + ":" + (p_verse + 1).ToString();
            string[] words = Get4Words(p_book, p_chapter, p_verse, p_word);
            string answer = bible.books[p_book].chapters[p_chapter].verses[p_verse].words[p_word];
            int answer_index = new System.Random().Next(4);
            for (int i = 0; i < 4; i++)
            {
                if (buttonAnswers[i].GetComponentInChildren<Text>().text == answer)
                {
                    answer_index = i;
                }
            }
            string[] shuffled = Shuffle(words, answer, answer_index);
            for (int i = 0; i < 4; i++)
            {
                buttonAnswers[i].GetComponentInChildren<Text>().text = shuffled[i];
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

    // Start is called before the first frame update
    void Start()
    {
        p_book = PlayerPrefs.GetInt(KEY_BOOK, 0);
        p_chapter = PlayerPrefs.GetInt(KEY_CHAPTER, 0);
        p_verse = PlayerPrefs.GetInt(KEY_VERSE, 0);

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



                //bv.line = line.Substring(sep + 1);
                //bv.words = bv.line.Split(' ');
                //Debug.Log(bv.book.ToString() + "+" + bv.chapter.ToString() + "+" + bv.verse.ToString() + "+" + bv.line);
                //count[bv.chapter] = bv.verse;
                //bibleVerse.Add(bv);
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
