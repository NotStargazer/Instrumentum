using System.IO;
using csFastFloat;
using SFB;
using UnityEngine;

namespace Instrumentum.ChartEditor
{
    public static class ChartLoader
    {
        private static readonly int TRACK_TOKEN = '[';
        private static readonly int TRACK_CLOSE_TOKEN = ']';
        private static readonly int SCOPE_TOKEN = '{';
        private static readonly int SCOPE_CLOSE_TOKEN = '}';
        private static readonly int PARSE_TOKEN = '=';

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
                return ParseInstruManiaChart(CURRENT_CHART);
            }

            return null;
        }

        public static void Save(Chart chart, bool saveAs)
        {
        }

        private static Chart ParseInstruManiaChart(string chartPath)
        {
            var chart = new Chart();

            using (var sr = new StreamReader(chartPath))
            {
                var currentTrack = string.Empty;

                while (!sr.EndOfStream)
                {
                    var token = sr.Read();

                    if (token == TRACK_TOKEN)
                    {
                        token = sr.Read();
                        do
                        {
                            currentTrack += (char)token;
                            token = sr.Read();
                            if (sr.EndOfStream)
                            {
                                return null;
                            }
                        } while (token != TRACK_CLOSE_TOKEN);
                    }

                    if (token == SCOPE_TOKEN)
                    {
                        switch (currentTrack)
                        {
                            case "Song":
                                if (ParseSongData(sr, chart))
                                    return null;
                                break;
                            case "Sync":
                                if (ParseSyncData(sr, chart))
                                    return null;
                                break;
                            case "Event":
                                if (ParseEventData(sr, chart))
                                    return null;
                                break;
                            case "Note":
                                if (ParseNoteData(sr, chart))
                                    return null;
                                break;
                        }

                        currentTrack = string.Empty;
                    }
                }
            }

            return chart;
        }

        private static bool ParseSongData(StreamReader sr, Chart chart)
        {
            var token = sr.Read();
            var dataPoint = string.Empty;
            var data = string.Empty;
            while (token != SCOPE_CLOSE_TOKEN)
            {
                while (!sr.EndOfStream)
                {
                    token = sr.Read();

                    if (token == SCOPE_CLOSE_TOKEN)
                    {
                        return false;
                    }
                    
                    if (token == PARSE_TOKEN)
                    {
                        dataPoint = dataPoint.Trim();
                        data = sr.ReadLine()?.Trim();
                        break;
                    }

                    dataPoint += (char)token;
                }

                switch (dataPoint)
                {
                    case "Title":
                        chart.SongTitle = data;
                        break;
                    case "Sub Title":
                        chart.SongSubTitle = data;
                        break;
                    case "Artist":
                        chart.SongArtist = data;
                        break;
                    case "Sub Artist":
                        chart.SongArtist = data;
                        break;
                    case "Genre":
                        chart.Genre = data;
                        break;
                    case "Song File":
                        chart.SongFileName = data;
                        break;
                    case "Jacket File":
                        chart.JacketFileName = data;
                        break;
                    case "BPM":
                        if (!FastFloatParser.TryParseFloat(data, out var bpm))
                            return true;
                        chart.Bpm = bpm;
                        break;
                    case "Time Signature":
                        if (data != null)
                        {
                            var rawTimeSignature = data.Split('/');
                            var timeSignature = new TimeSignature();
                            if (!IntParseFast(rawTimeSignature[0], out timeSignature.Upper))
                                return true;

                            if (!IntParseFast(rawTimeSignature[1], out timeSignature.Lower))
                                return true;

                            chart.TimeSignature = timeSignature;
                        }
                        break;
                }

                data = string.Empty;
                dataPoint = string.Empty;
            }

            return false;
        }

        private static bool ParseSyncData(StreamReader sr, Chart chart)
        {
            var token = sr.Read();
            var time = 0;
            var dataPoint = string.Empty;
            var rawData = string.Empty;
            while (token != SCOPE_CLOSE_TOKEN)
            {                
                while (!sr.EndOfStream)
                {
                    token = sr.Read();

                    if (token == SCOPE_CLOSE_TOKEN)
                    {
                        return false;
                    }
                    
                    if (token == PARSE_TOKEN)
                    {
                        if (!IntParseFast(dataPoint.Trim(), out time))
                            return true;
                        rawData = sr.ReadLine()?.Trim();
                        break;
                    }

                    dataPoint += (char)token;
                }

                if (rawData == null)
                {
                    continue;
                }
                
                var dataSplit = rawData.Split();
                var identifier = dataSplit[0];
                var data = dataSplit[1];
                
                rawData = string.Empty;
                dataPoint = string.Empty;

                if (identifier == "B")
                {
                    if (!FastFloatParser.TryParseFloat(data, out var bpmValue))
                        return true;

                    chart.BpmChanges.Add(new Bpm(time, bpmValue));

                    continue;
                }

                if (identifier == "T")
                {
                    var ts = data.Split('/');

                    if (!IntParseFast(ts[0], out var upper))
                        return true;

                    if (!IntParseFast(ts[1], out var lower))
                        return true;

                    chart.TimeSignatureChanges.Add(new TimeSignature(time, upper, lower));
                }
            }

            return false;
        }

        private static bool ParseEventData(StreamReader sr, Chart chart)
        {
            var token = sr.Read();
            var time = 0;
            var dataPoint = string.Empty;
            var rawData = string.Empty;
            while (token == SCOPE_CLOSE_TOKEN)
            {
                while (!sr.EndOfStream)
                {
                    token = sr.Read();

                    if (token == SCOPE_CLOSE_TOKEN)
                    {
                        return false;
                    }

                    if (token == PARSE_TOKEN)
                    {
                        if (!IntParseFast(dataPoint.Trim(), out time))
                            return true;
                        rawData = sr.ReadLine()?.Trim();
                        break;
                    }

                    dataPoint += (char)token;
                }

                if (rawData == null)
                {
                    continue;
                }

                var dataSplit = rawData.Split();
                var identifier = dataSplit[0];
                var data = dataSplit[1];

                rawData = string.Empty;
                dataPoint = string.Empty;

                if (identifier == "S")
                {
                }

                if (identifier == "C")
                {
                }
            }

            return false;
        }

        private static bool ParseNoteData(StreamReader sr, Chart chart)
        {
            var token = sr.Read();
            var time = 0;
            var dataPoint = string.Empty;
            var rawData = string.Empty;
            while (token != SCOPE_CLOSE_TOKEN)
            {
                while (!sr.EndOfStream)
                {
                    token = sr.Read();

                    if (token == SCOPE_CLOSE_TOKEN)
                    {
                        return false;
                    }

                    if (token == PARSE_TOKEN)
                    {
                        if (!IntParseFast(dataPoint.Trim(), out time))
                            return true;
                        rawData = sr.ReadLine()?.Trim();
                        break;
                    }

                    dataPoint += (char)token;
                }

                if (rawData == null)
                {
                    continue;
                }

                var dataSplit = rawData.Split();
                var identifier = dataSplit[0];
                var nId = dataSplit[1];
                var nMod = dataSplit[2];
                var nPtrId = dataSplit[3];
                var nPtrTime = dataSplit[4];
                    
                rawData = string.Empty;
                dataPoint = string.Empty;
                    
                if (identifier == "N")
                {
                    if (!IntParseFast(nId.Trim(), out var id))
                        return true;
                    if (!IntParseFast(nMod.Trim(), out var mod))
                        return true;
                    if (!IntParseFast(nPtrId.Trim(), out var ptrId))
                        return true;
                    if (!IntParseFast(nPtrTime.Trim(), out var ptrTime))
                        return true;
                    chart.Notes.Add(new Note(time, id, mod, new NotePtr(ptrTime, ptrId)));
                }
            }
            return false;
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