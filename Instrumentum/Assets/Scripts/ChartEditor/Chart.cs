using System.Collections.Generic;

namespace Instrumentum.ChartEditor
{
    public class Chart
    {
        public string SongFileName;
        public string JacketFileName;
        public string Charter;

        public string SongTitle;
        public string SongSubTitle;
        public string SongArtist;
        public string SongSubArtist;
        public string Genre;

        public float Bpm;
        public TimeSignature TimeSignature;
        public int Difficulty;
        public int Level;
        public List<Note> Notes = new List<Note>();
        public List<Bpm> BpmChanges = new List<Bpm>();
        public List<TimeSignature> TimeSignatureChanges = new List<TimeSignature>();
    }

    public struct Note
    {
        public Note(int time, int id, int mod, NotePtr ptr)
        {
            Time = time;
            Id = id;
            Mod = mod;
            Ptr = ptr;
        }

        public int Time;
        public int Id;
        public int Mod;
        public NotePtr Ptr;
    }
    
    public struct Bpm
    {
        public Bpm(int time, float value)
        {
            Time = time;
            Value = value;
        }
        
        public int Time;
        public float Value;
    }

    public struct TimeSignature
    {
        public TimeSignature(int time, int upper, int lower)
        {
            Time = time;
            Upper = upper;
            Lower = lower;
        }

        public int Time;
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