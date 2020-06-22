using System;

[Serializable]
public class Bookmark
{
    public int version;
    public int p_book;
    public int[] saved_chapter;
    public int[] saved_verse;
    public int[] read_count;

    public Bookmark()
    {
        version = 1;
        p_book = 18;
        saved_chapter = new int[BIBLE_DEF.BOOK_MAX];
        saved_verse = new int[BIBLE_DEF.BOOK_MAX];
        read_count = new int[BIBLE_DEF.BOOK_MAX];
        for (int i=0; i<BIBLE_DEF.BOOK_MAX; i++)
        {
            saved_chapter[i] = 0;
            saved_verse[i] = 0;
            read_count[i] = 0;
        }
    }
}

