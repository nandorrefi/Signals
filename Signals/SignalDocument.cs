using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Signals
{
    public class SignalDocument : Document
    {
        private List<SignalValue> signals = new List<SignalValue>();
        private Random rand = new Random();
        private Thread liveDataThread;
        private bool dataIsLive = false;

        public bool DataIsLive { get { return dataIsLive; } }

        public IReadOnlyList<SignalValue> Signals 
        { 
            get { return signals; } 
        }

        public SignalDocument(string name) : base(name)
        {
            Random rand = new Random();
            for(int i = 0; i < 5; ++i)
            {
                signals.Add(new SignalValue( rand.NextDouble() * 100 - 50, DateTime.Now));
                Thread.Sleep(200);
            }
        }

        public override void SaveDocument(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                for(int i = 0; i < signals.Count; ++i)
                {
                    double value = signals[i].Value;
                    string date = signals[i].TimeStamp.ToUniversalTime().ToString("o");

                    sw.WriteLine($"{value}\t{date}");
                }
            }
        }

        public override void LoadDocument(string filePath)
        {
            using(StreamReader sr = new StreamReader(filePath))
            {
                string line;
                signals.Clear();

                while((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0)
                        continue;

                    string[] columns = line.Split('\t');

                    double value = double.Parse(columns[0]);
                    DateTime timeStamp = DateTime.Parse(columns[1]).ToLocalTime();

                    signals.Add(new SignalValue(value, timeStamp));
                }
            }

            UpdateAllViews();

            TraceValues();
        }

        private void TraceValues()
        {
            foreach (SignalValue signal in signals)
                Trace.WriteLine(signal.ToString());
        }

        public override void SwitchLiveDataSource()
        {
            try
            {
                if (!dataIsLive)
                {
                    dataIsLive = true;
                    liveDataThread = new Thread(GenerateNewSignal);
                    liveDataThread.IsBackground = true;
                    liveDataThread.Start();
                }
                else
                {
                    dataIsLive = false;
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void GenerateNewSignal()
        {
            while (dataIsLive)
            {
                SignalValue randSignal = new SignalValue(rand.NextDouble() * 100 - 50, DateTime.Now);
                signals.Add(randSignal);

                Thread.Sleep(rand.Next(500));
            }
            
        }

    }
}
