using System;
using System.Collections.Generic;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.Media;
using System.Text.RegularExpressions;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Invoke, "Note", SupportsShouldProcess = true)]
    [CmdletDescription("Plays sound on the pc-speaker")]
    public class InvokeNote: PSCmdlet
    {

        public enum TuneType
        {
            NokiaTune,
            TubularBells,
            SuperMario,
            Mario,
            SweetChildOMine,
            ImperialMarch,
            Furelise,
            MaryHadALittleLamb,
            MissionImpossible,
            AWholeNewWorld
        }

        private void BeepIt(int Amplitude, int Frequency, int Duration)
        {
            double A = ((Amplitude * (System.Math.Pow(2, 15))) / 1000) - 1;
            double DeltaFT = 2 * Math.PI * Frequency / 44100.0;

            int Samples = 441 * Duration / 10;
            int Bytes = Samples * 4;
            int[] Hdr = { 0X46464952, 36 + Bytes, 0X45564157, 0X20746D66, 16, 0X20001, 44100, 176400, 0X100004, 0X61746164, Bytes };
            using (MemoryStream MS = new MemoryStream(44 + Bytes))
            {
                using (BinaryWriter BW = new BinaryWriter(MS))
                {
                    for (int I = 0; I < Hdr.Length; I++)
                    {
                        BW.Write(Hdr[I]);
                    }
                    for (int T = 0; T < Samples; T++)
                    {
                        short Sample = System.Convert.ToInt16(A * Math.Sin(DeltaFT * T));
                        BW.Write(Sample);
                        BW.Write(Sample);
                    }
                    BW.Flush();
                    MS.Seek(0, SeekOrigin.Begin);
                    using (SoundPlayer SP = new SoundPlayer(MS))
                    {
                        SP.PlaySync();
                    }
                }
            }
        }

        private void LoadNotes(bool morse)
        {
            if (morse)
            {
                noteTable = new Dictionary<string, int[]>();
                noteTable.Add("A", new int[] { 50, 150 });
                noteTable.Add("B", new int[] { 150, 50, 50, 50 });
                noteTable.Add("C", new int[] { 150, 50, 150, 50 });
                noteTable.Add("D", new int[] { 150, 50, 50 });
                noteTable.Add("E", new int[] { 50 });
                noteTable.Add("F", new int[] { 50, 50, 150, 50 });
                noteTable.Add("G", new int[] { 150, 150, 50 });
                noteTable.Add("H", new int[] { 50, 50, 50, 50 });
                noteTable.Add("I", new int[] { 50, 50 });
                noteTable.Add("J", new int[] { 50, 150, 150, 150 });
                noteTable.Add("K", new int[] { 150, 50, 150 });
                noteTable.Add("L", new int[] { 50, 150, 50, 50 });
                noteTable.Add("M", new int[] { 150, 150 });
                noteTable.Add("N", new int[] { 150, 50 });
                noteTable.Add("O", new int[] { 150, 150, 150 });
                noteTable.Add("P", new int[] { 50, 150, 150, 50 });
                noteTable.Add("Q", new int[] { 150, 150, 50, 150 });
                noteTable.Add("R", new int[] { 50, 150, 50 });
                noteTable.Add("S", new int[] { 50, 50, 50 });
                noteTable.Add("T", new int[] { 150 });
                noteTable.Add("U", new int[] { 50, 50, 150 });
                noteTable.Add("V", new int[] { 50, 50, 50, 150 });
                noteTable.Add("W", new int[] { 50, 150, 150 });
                noteTable.Add("X", new int[] { 150, 50, 50, 150 });
                noteTable.Add("Y", new int[] { 150, 50, 150, 150 });
                noteTable.Add("Z", new int[] { 150, 150, 50, 50 });
                noteTable.Add("0", new int[] { 150, 150, 150, 150, 150  });
                noteTable.Add("1", new int[] { 50, 150, 150, 150, 150 });
                noteTable.Add("2", new int[] { 50, 50, 150, 150, 150 });
                noteTable.Add("3", new int[] { 50, 50, 50, 150, 150 });
                noteTable.Add("4", new int[] { 50, 50, 50, 50, 150 });
                noteTable.Add("5", new int[] { 50, 50, 50, 50, 50});
                noteTable.Add("6", new int[] { 150, 50, 50, 50, 50 });
                noteTable.Add("7", new int[] { 150, 150, 50, 50, 50 });
                noteTable.Add("8", new int[] { 150, 150, 150, 50, 50 });
                noteTable.Add("9", new int[] { 150, 150, 150, 150, 50 });
                noteTable.Add(".", new int[] { 50, 150, 50, 150, 50, 150});
                noteTable.Add(",", new int[] { 150, 150, 50, 50, 150, 150 });
                noteTable.Add("?", new int[] { 50, 50, 150, 150, 50, 50 });
                noteTable.Add("'", new int[] { 50, 150, 150, 150, 150, 50 });
                noteTable.Add("!", new int[] { 150, 50, 150, 50, 150, 150 });
                noteTable.Add("/", new int[] { 150, 50, 50, 150, 50 });
                noteTable.Add("(", new int[] { 150, 50, 150, 150, 50 });
                noteTable.Add(")", new int[] { 150, 50, 150, 150, 50, 150 });
                noteTable.Add("&", new int[] { 50, 150, 50, 50, 50});
                noteTable.Add(":", new int[] { 150, 150, 150, 50, 50, 50 });
                noteTable.Add(";", new int[] { 150, 50, 150, 50, 150, 50 });
                noteTable.Add("=", new int[] { 150, 50, 50, 50, 150 });
                noteTable.Add("+", new int[] { 50, 150, 50, 150, 50 });
                noteTable.Add("-", new int[] { 150, 50, 50, 50, 50, 150 });
                noteTable.Add("_", new int[] { 50, 50, 150, 150, 50, 150 });
                noteTable.Add("\"", new int[] { 50, 150, 50, 50, 150, 50 });
                noteTable.Add("$", new int[] { 50, 50, 50, 150, 50, 50, 150 });
                noteTable.Add("@", new int[] { 50, 150, 150, 50, 150, 50 });
                noteTable.Add("Å", new int[] { 50, 150, 150, 50, 150 });
                noteTable.Add("Ä", new int[] { 50, 150, 50, 150 });
                noteTable.Add("Ö", new int[] { 150, 150, 150, 50 });
                noteTable.Add("Ü", new int[] { 50, 50, 150, 150 });
                noteTable.Add(" ", new int[] { });
                noteTable.Add("START", new int[] { 150, 50, 150, 50, 150 });
                noteTable.Add(Environment.NewLine.ToString(), new int[] { 50, 150, 50, 150 });
                noteTable.Add("END", new int[] { 50, 50, 50, 150, 50, 150 });
            }
            else
            {
                noteTable = new Dictionary<string, int[]>();
                noteTable.Add("C", new int[] { 16, 33, 65, 131, 262, 523, 1046, 2093 });
                noteTable.Add("C#", new int[] { 17, 35, 69, 139, 277, 554, 1109, 2217 });
                noteTable.Add("D", new int[] { 18, 37, 73, 147, 294, 587, 1175, 2349 });
                noteTable.Add("D#", new int[] { 19, 39, 78, 155, 311, 622, 1244, 2489 });
                noteTable.Add("E", new int[] { 21, 41, 82, 165, 330, 659, 1328, 2637 });
                noteTable.Add("F", new int[] { 22, 44, 87, 175, 349, 698, 1397, 2794 });
                noteTable.Add("F#", new int[] { 23, 46, 92, 185, 370, 740, 1480, 2960 });
                noteTable.Add("G", new int[] { 24, 49, 98, 196, 392, 784, 1568, 3136 });
                noteTable.Add("G#", new int[] { 26, 52, 104, 208, 415, 831, 1661, 3322 });
                noteTable.Add("A", new int[] { 27, 55, 110, 220, 440, 880, 1760, 3520 });
                noteTable.Add("A#", new int[] { 29, 58, 116, 233, 466, 932, 1865, 3729 });
                noteTable.Add("B", new int[] { 31, 62, 123, 245, 494, 988, 1975, 3951 });
            }
        }

        private Match RegExpMatch(string input, string expression)
        {   
            Regex regExp = new Regex(expression);
            Match result = regExp.Match(input);
            if (result.Success == true)
                return result;
            else
                return null;
        }

        private int DurationFromString(string chr)
        {
            switch (chr)
            {
                case "W": return 800;
                case "H": return 400;
                case "Q": return 200;
                case "E": return 100;
                case "S": return 50;
                default: return 200;
            }
        }

        private void ParseNote(string inputData)
        {
            int duration = DurationFromString("H");
            int frequency = -1;
            int amplitude = 5;

            //Get the note
            string rawNote = inputData.Trim().ToUpper();

            if (rawNote.StartsWith("U"))
            {
                Match result = RegExpMatch(rawNote, @"^[U](\d*)([WHQES]{0,1})([0-9]{0,1})");
                if (result != null)
                {
                    frequency = int.Parse(result.Groups[1].Value);
                    if (result.Groups[2].Value != string.Empty) duration = DurationFromString(result.Groups[2].Value);
                    if (result.Groups[3].Value != string.Empty) amplitude = int.Parse(result.Groups[3].Value);
                }
            }
            else
            {
                Match result = RegExpMatch(rawNote, @"^([CDEFGAB])(\#{0,1})([0-7]{0,1})([WHQES]{0,1})([0-9]{0,1})");
                if (result != null)
                {
                    try
                    {
                        string note = string.Format("{0}{1}", result.Groups[1].Value, result.Groups[2].Value);
                        int octave = (result.Groups[3].Value != string.Empty) ? int.Parse(result.Groups[3].Value) : 4;

                        frequency = noteTable[note][octave];
                        if (result.Groups[4].Value != string.Empty) duration = DurationFromString(result.Groups[4].Value);
                        if (result.Groups[5].Value != string.Empty) amplitude = int.Parse(result.Groups[5].Value);
                    }
                    catch
                    {
                    }
                }
            }

            if (frequency != -1)
                BeepIt(amplitude * 100, frequency, duration);
            else
                WriteWarning(string.Format("Could not parse note: {0}", inputData));
        }

        #region "Private Parameters"
        private string song = null;
        private string morse = null;
        private int tuneType = -1;
        private bool printNotes = false;
        private Dictionary<string, int[]> noteTable;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "The document to convert to PDF", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public TuneType Tune
        {
            get { return (TuneType)tuneType; }
            set { tuneType = (int)value; }
        }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = "The document to convert to PDF", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter PrintNotes
        {
            get { return printNotes; }
            set { printNotes = value; }
        }

        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The document to convert to PDF", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string Notes
        {
            get { return song; }
            set { song = value; }
        }

        [Parameter(Position = 2, Mandatory = false, HelpMessage = "The document to convert to PDF", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string Morse
        {
            get { return morse; }
            set { morse = value; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            // Initialize the note-to-frequency table
            LoadNotes((morse==null)?false:true);

            Dictionary<TuneType, string> songs = new Dictionary<TuneType, string>();
            songs.Add(TuneType.Furelise,"E5E D#5E E5E D#5E E5E B4H D5E C5E A4E C4E E4E A4E B4E E4E G#4E B4E C5E E4E E5E D#5E E5E D#5E E5E B4E D5E C5E B4E E4E C5E B4E A4E");
            songs.Add(TuneType.NokiaTune,"U1318E U1174E F#5Q U830Q U1108E U987E D5Q E5Q U987E A5E C#5Q E5Q A5H");
            songs.Add(TuneType.TubularBells,"U329E A4E U329E U493E U329E G4E A4E U329E C5E U329E D5E U329E U493E C5E U329E A4E U329E U493E U329E G4E A4E U329E C5E U329E D5E U329E U493E C5E U329E U493E U329E A4E U329E U493E U329E G4E A4E U329E");
            songs.Add(TuneType.SuperMario,"E5E E5E U0E E5E U0E C5E E5E U0E G5E U0Q G4E U0Q C5E U0Q G4E U0Q E4E U0Q A4E U0E B4E U0E A#4E U0S A4E U0E G4E U0E E5E U0E G5E U0E A5E U0E F5E G5E U0E E5E U0E C5E U0E D5E B4E U0E C5E U0Q G4E U0Q E4E U0Q A4E U0E B4E U0E A#4E U0S A4E U0E G4E U0E E5E U0E G5E U0E A5E U0E F5E G5E U0E E5E U0E C5E U0E D5E B4E U0Q G5E F#5E F5E U0S D#5E U0E E5E U0E G#4E A4E C5E U0E A4E C5E D5E U0Q G5E F#5E F5E U0S D#5E U0E E5E U0E F5E U0E F5E F5E U0H G5E F#5E F5E U0S D#5E U0E E5E U0E G#4E A4E C5E U0E A4E C5E D5E U0Q D#5E U0Q D5E U0Q C5E U0H G5E F#5E F5E U0S D#5E U0E E5E U0E G#4E A4E C5E U0E A4E C5E D5E U0Q G5E F#5E F5E U0S D#5E U0E E5E U0E F5E U0E F5E F5E U0H G5E F#5E F5E U0S D#5E U0E E5E U0E G#4E A4E C5E U0E A4E C5E D5E U0Q D#5E U0Q D5E U0Q C5E"); 
            songs.Add(TuneType.Mario,"E5E E5E U0E E5E U0E C5E E5E U0E U783E");
            songs.Add(TuneType.SweetChildOMine,"D5E U1174E A5E G5E G6E A5E F#6E A5E D5E U1174E A5E G5E G6E A5E F#6E A5E E5E U1174E A5E G5E G6E A5E F#6E A5E E5E U1174E A5E G5E G6E A5E F#6E A5E G5E U1174E A5E G5E G6E A5E F#6E A5E G5E U1174E A5E G5E G6E A5E F#6E A5E D5E U1174E A5E G5E G6E A5E F#6E A5E D5E U1174E A5E G5E G6E A5E F#6E A5E D5E U1174E A5E G5E G6E A5E F#6E A5E D5E U1174E A5E G5E G6E A5E F#6E A5E E5E U1174E A5E G5E G6E A5E F#6E A5E E5E U1174E A5E G5E G6E A5E F#6E A5E G5E U1174E A5E G5E G6E A5E F#6E A5E G5E U1174E A5E G5E G6E A5E F#6E A5E U1318E A5E U1174E A5E U1318E A5E F#6E A5E G6E A5E F#6E A5E U1318E A5E U1174E A5E U1174H");
            songs.Add(TuneType.ImperialMarch,"A4H A4H A4H F4Q C5E A4H F4Q C5E A4H E5H E5H E5H F5Q C5E G#4H F4Q C5E A4H");
            songs.Add(TuneType.MaryHadALittleLamb,"U247Q A3Q G3Q A3Q U247Q U247Q U247H A3Q A3Q A3H U247Q D4Q D4H");
            songs.Add(TuneType.MissionImpossible,"G5E U0Q G5E U0Q A#5E U0E U1047E U0E G5E U0Q G5E U0Q U699E U0E F#5E U0E G5E U0Q G5E U0Q A#5E U0E U1047E U0E G5E U0Q G5E U0Q U699E U0E F#5E U0E A#5E G5E D5H U0S A#5E G5E C#5H U0S A#5E G5E C5H U0E A#4E C5E");
            songs.Add(TuneType.AWholeNewWorld, "E B F# G# G# E B F# G# B G# G# A F# G# E G# B F# G# G# E B B F# G# G# A G# E B B F# G# G# E B B F# G# B G# G# A F# G# E G# B F# G# G# C# A# D# B A C# F# G# B G# G# B G# B D# C# D# F# C# B B C# A C# E A B B E B B E B A");

            if (tuneType != -1)
                song = songs[(TuneType)tuneType];

            if (morse!=null)
            {
                int[] beeps;
                Console.WriteLine("");
                Console.Write("Keying: ");
                Console.Write("START ");
                beeps = noteTable["START"];
                foreach (int key in beeps)
                {
                    BeepIt(1 * 100, 600, key);
                    System.Threading.Thread.Sleep(150);
                }

                System.Threading.Thread.Sleep(300);

                foreach (char chr in morse.ToUpper().ToCharArray())
                {
                    try
                    {
                        Console.Write(chr.ToString());
                        beeps = noteTable[chr.ToString()];
                        foreach (int key in beeps)
                        {
                            BeepIt(1 * 100, 600, key);
                            System.Threading.Thread.Sleep(150);
                        }
                    }
                    catch
                    {
                    }
                    System.Threading.Thread.Sleep(300);
                }

                Console.Write(" END");
                beeps = noteTable["END"];
                foreach (int key in beeps)
                {
                    BeepIt(1 * 100, 600, key);
                    System.Threading.Thread.Sleep(150);
                }
                Console.WriteLine();
                Console.WriteLine();

            }
            else
            {
                if (song == null) ThrowTerminatingError(new ErrorRecord(new Exception("No tune specified"), "100", ErrorCategory.InvalidData, null));
                if (printNotes == true) WriteObject(song);

                foreach (string note in song.Split(' '))
                    ParseNote(note);
            }
        
        }
        #endregion


    }
}
