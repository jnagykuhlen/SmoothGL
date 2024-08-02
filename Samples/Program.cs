using SmoothGL.Samples;
using SmoothGL.Samples.Windows;

ISample[] samples =
[
    new Sample<HelloWorldSampleWindow>("Hello World Sample"),
    new Sample<AdvancedTechniquesSampleWindow>("Advanced Techniques Sample")
];

for (var i = 0; i < samples.Length; ++i)
    Console.WriteLine($"({i + 1}) {samples[i].Title}");

Console.WriteLine();
Console.WriteLine("Please enter a number to select the sample to start.");

var selectedSampleIndex = ReadIndex(samples.Length);
if (selectedSampleIndex == null)
    return;

var selectedSample = samples[selectedSampleIndex.Value];

Console.WriteLine($"Starting {selectedSample.Title}.");
selectedSample.CreateWindow().Run();


static int? ReadIndex(int exclusiveMaximum)
{
    int index;
    do
    {
        var keyInfo = Console.ReadKey(true);
        if (keyInfo.Key == ConsoleKey.Escape)
            return null;

        index = keyInfo.KeyChar - '1';
    } while (index < 0 || index >= exclusiveMaximum);

    return index;
}
