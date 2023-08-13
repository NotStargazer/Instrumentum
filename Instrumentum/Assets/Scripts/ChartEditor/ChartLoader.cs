using System.IO;
using csFastFloat;
using SFB;
using UnityEngine;

namespace Instrumentum.ChartEditor
{
    public static class ChartLoader
    {
        private static string CURRENT_CHART;
        
        public static Chart Load()
        {
            var file = StandaloneFileBrowser.OpenFilePanel(
                title:       "Load Chart", 
                directory:   Application.persistentDataPath,
                extension:   "imChart",
                multiselect: false);

            if (file.Length > 0)
            {
                CURRENT_CHART = file[0];
                var textChart = File.ReadAllLines(CURRENT_CHART);
                return ParseInstruManiaChart(textChart);
            }

            return null;
        }
        
        public static void Save(Chart chart, bool saveAs)
        {
            
        }

        private static Chart ParseInstruManiaChart(string[] textChart)
        {
            var chart = new Chart();

            var lineIndex = 0;

            var dataTrack = GetTrack(ref lineIndex, textChart, "[Song]");
            var syncTrack =  GetTrack(ref lineIndex, textChart, "[Sync]");
            var eventTrack = GetTrack(ref lineIndex, textChart, "[Event]");
            var noteTrack =  GetTrack(ref lineIndex, textChart, "[Note]");

            //Parse Song Data
            if (ParseSongData(textChart, dataTrack, chart)) return null;
            if (ParseSyncData(textChart, syncTrack, chart)) return null;
            if (ParseEventData(textChart, eventTrack, chart)) return null;
            if (ParseNoteData(textChart, noteTrack, chart)) return null;
            
            return chart;
        }

        private static bool ParseSongData(string[] textChart, LineRange dataTrack, Chart chart)
        {
            for (var songI = dataTrack.StartLine; songI <= dataTrack.EndLine; songI++)
            {
                var line = textChart[songI].Trim();
                if (line.StartsWith("Title"))
                {
                    chart.SongTitle = line.Split('=')[1].Trim();
                    continue;
                }

                if (line.StartsWith("Sub Title"))
                {
                    chart.SongSubTitle = line.Split('=')[1].Trim();
                    continue;
                }

                if (line.StartsWith("Artist"))
                {
                    chart.SongArtist = line.Split('=')[1].Trim();
                    continue;
                }

                if (line.StartsWith("Sub Artist"))
                {
                    chart.SongSubArtist = line.Split('=')[1].Trim();
                    continue;
                }

                if (line.StartsWith("Genre"))
                {
                    chart.Genre = line.Split('=')[1].Trim();
                    continue;
                }

                if (line.StartsWith("Song File"))
                {
                    chart.SongFileName = line.Split('=')[1].Trim();
                    continue;
                }

                if (line.StartsWith("Jacket File"))
                {
                    chart.JacketFileName = line.Split('=')[1].Trim();
                    continue;
                }

                if (line.StartsWith("BPM"))
                {
                    var bpm = line.Split('=')[1].Trim();
                    if (!FastFloatParser.TryParseFloat(bpm, out chart.Bpm))
                        return true;
                    continue;
                }

                if (line.StartsWith("Time Signature"))
                {
                    var ts = line.Split('=')[1].Trim().Split('/');
                    var timeSignature = new TimeSignature();
                    if (!IntParseFast(ts[0], out timeSignature.Upper))
                        return true;

                    if (!IntParseFast(ts[1], out timeSignature.Lower))
                        return true;

                    chart.TimeSignature = timeSignature;
                }
            }

            return false;
        }
        
        private static bool ParseSyncData(string[] textChart, LineRange syncTrack, Chart chart)
        {
            for (var songS = syncTrack.StartLine; songS <= syncTrack.EndLine; songS++)
            {
                var line = textChart[songS].Split('=');
                if (!IntParseFast(line[0].Trim(), out var time))
                    return true;
                var sync = line[1].Trim();
                
                if (sync.StartsWith('B'))
                {
                    var bpmString = sync.Remove(0, 1).TrimStart();

                    if (!FastFloatParser.TryParseFloat(bpmString, out var bpmValue))
                        return true;

                    chart.BpmChanges.Add(new Bpm(time, bpmValue));

                    continue;
                }

                if (sync.StartsWith('T'))
                {
                    var tsString = sync.Remove(0, 1).TrimStart();
                    var ts = tsString.Split('/');

                    if (!IntParseFast(ts[0], out var upper))
                        return true;

                    if (!IntParseFast(ts[1], out var lower))
                        return true;

                    chart.TimeSignatureChanges.Add(new TimeSignature(time, upper, lower));
                }
            }

            return false;
        }

        private static bool ParseEventData(string[] textChart, LineRange eventTrack, Chart chart)
        {
            for (var songE = eventTrack.StartLine; songE <= eventTrack.EndLine; songE++)
            {
                var line = textChart[songE].Split('=');
                if (!IntParseFast(line[0].TrimStart(), out var time))
                    return true;
                var chartEvent = line[1].Trim();
                
                if (chartEvent.StartsWith('S'))
                { }

                if (chartEvent.StartsWith('E'))
                { }
            }
            
            return false;
        }
        
        private static bool ParseNoteData(string[] textChart, LineRange noteTrack, Chart chart)
        {
            for (var songN = noteTrack.StartLine; songN <= noteTrack.EndLine; songN++)
            {
                var line = textChart[songN].Split('=');
                if (!IntParseFast(line[0].Trim(), out var time))
                    return true;
                var noteString = line[1].Trim();

                if (noteString.StartsWith('N'))
                {
                    var noteData = noteString.Split();
                    if (!IntParseFast(noteData[1], out var id))
                        return true;
                    if (!IntParseFast(noteData[2], out var mod))
                        return true;
                    if (!IntParseFast(noteData[3], out var ptrId))
                        return true;
                    if (!IntParseFast(noteData[4], out var ptrTime))
                        return true;

                    chart.Notes.Add(new Note(id, mod, new NotePtr(ptrId, ptrTime)));
                }
            }
            
            return false;
        }

        private static LineRange GetTrack(ref int lineIndex, string[] chart, string trackName)
        {
            var track = new LineRange();
            for (; lineIndex < chart.Length; lineIndex++)
            {
                if (chart[lineIndex].StartsWith(trackName))
                {
                    track.StartLine = lineIndex + 2;
                    do
                    {
                        lineIndex++;
                    } while (!chart[lineIndex].StartsWith('}'));

                    track.EndLine = lineIndex - 1;
                    break;
                }
            }

            return track;
        }
        
        private struct LineRange
        {
            public int StartLine;
            public int EndLine;
        }
        
        private static bool IntParseFast(string s, out int result)
        {
            int value = 0;
            var length = s.Length;
            for (int i = 0; i < length; i++)
            {
                var c = s[i];
                if (!char.IsDigit(c))
                {
                    result = -1;
                    return false;
                }
                value = 10 * value + (c - 48);
            }
            result = value;
            return true;
        }
    }
}