using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmoothGL.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SampleWindow[] samples = new SampleWindow[]
            {
                new HelloWorldSample(),
                new AdvancedTechniquesSample()
            };

            for (int i = 0; i < samples.Length; ++i)
                Console.WriteLine("({0}) {1}", (char)('1' + i), samples[i].Title);

            Console.WriteLine();
            Console.WriteLine("Please enter a number to select the sample to start.");

            int selectedSampleIndex;
            do
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Escape)
                    return;

                selectedSampleIndex = keyInfo.KeyChar - '1';
            }
            while (selectedSampleIndex < 0 || selectedSampleIndex >= samples.Length);

            Console.WriteLine("Starting {0}.", samples[selectedSampleIndex].Title);
            samples[selectedSampleIndex].Run(60.0, 60.0);
        }
    }
}
