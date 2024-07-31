namespace SmoothGL.Samples;

public class Program
{
    public static void Main(string[] args)
    {
        SampleWindow[] samples =
        {
            new HelloWorldSample(),
            new AdvancedTechniquesSample()
        };

        for (var i = 0; i < samples.Length; ++i)
            Console.WriteLine("({0}) {1}", (char)('1' + i), samples[i].Title);

        Console.WriteLine();
        Console.WriteLine("Please enter a number to select the sample to start.");

        int selectedSampleIndex;
        do
        {
            var keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Escape)
                return;

            selectedSampleIndex = keyInfo.KeyChar - '1';
        } while (selectedSampleIndex < 0 || selectedSampleIndex >= samples.Length);

        Console.WriteLine("Starting {0}.", samples[selectedSampleIndex].Title);
        samples[selectedSampleIndex].Run();
    }
}