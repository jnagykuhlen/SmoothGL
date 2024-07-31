using SmoothGL.Samples;
using SmoothGL.Samples.Windows;

ISample[] samples =
[
    new Sample<HelloWorldSampleWindow>("Hello World Sample"),
    new Sample<AdvancedTechniquesSampleWindow>("Advanced Techniques Sample")
];

for (var i = 0; i < samples.Length; ++i)
    Console.WriteLine("({0}) {1}", i + 1, samples[i].Title);

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
samples[selectedSampleIndex].CreateWindow().Run();

