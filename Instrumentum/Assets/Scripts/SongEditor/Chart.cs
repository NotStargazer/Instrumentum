using System.Collections.Generic;

namespace Instrumentum.SongEditor
{
    public class Chart
    {
        //Metadata
        public string SongPath;

        //Song Data
        public string SongTitle;
        public string SongSubtitle;
        public string SongArtist;
        public string SongSubArtist;

        //Chart Data
        public float Bpm;
        public TimeSignature TimeSignature;
        public int Difficulty;
        public int Level;
        public List<Note> Notes;
        public List<float> BpmChanges;
        public List<TimeSignature> TimeSignatureChanges;
    }

    public struct Note
    {
        public int Time;
        public int Id;
        public int Mod;
        public NotePtr Ptr;
    }

    public struct TimeSignature
    {
        public TimeSignature(int upper, int lower)
        {
            Upper = upper;
            Lower = lower;
        }

        public int Upper;
        public int Lower;
    }

    public struct NotePtr
    {
        public NotePtr(int time, int id)
        {
            Time = time;
            Id = id;
        }

        public int Time;
        public int Id;
    }
}