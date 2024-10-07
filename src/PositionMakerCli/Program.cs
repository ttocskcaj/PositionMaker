namespace PositionMakerCli;

using System.Text.Json;
using PositionMakerCli.PositionGenerator;
using Cocona;
using System;
using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        CoconaApp.Run<Program>(args);
    }

    public void Run(int count, string directory, string? filenameTemplate)
    {
        Generate(count, directory, filenameTemplate);
    }

    private static void Generate(int count, string directory, string? filenameTemplate)
    {
        if (string.IsNullOrEmpty(filenameTemplate))
        {
            filenameTemplate = "{Side}_{Count}.json";
        }

        string sideAFilename = filenameTemplate.Replace("{Side}", "side_a").Replace("{Count}", count.ToString());
        string sideBFilename = filenameTemplate.Replace("{Side}", "side_b").Replace("{Count}", count.ToString());

        string sideAPath = Path.Combine(directory, sideAFilename);
        string sideBPath = Path.Combine(directory, sideBFilename);

        var positionGenerator = new GtrPositionGenerator(count);

        using var streamWriterA = new StreamWriter(sideAPath);
        using var jsonWriterA = new Utf8JsonWriter(streamWriterA.BaseStream, new JsonWriterOptions { Indented = true });
        using var streamWriterB = new StreamWriter(sideBPath);
        using var jsonWriterB = new Utf8JsonWriter(streamWriterB.BaseStream, new JsonWriterOptions { Indented = true });
        jsonWriterA.WriteStartArray();
        jsonWriterB.WriteStartArray();

        // 10% of each file won't have any valid links
        var unlinkedCount = (int)(count * 0.1);
        AnsiConsole.MarkupLine("[bold yellow]Generating Position Data[/]");
        AnsiConsole.MarkupLine("Creating two JSON files: [blue]Side A[/] and [blue]Side B[/]");
        AnsiConsole.MarkupLine("Each file will contain:");
        AnsiConsole.MarkupLine("  • [green]{0}[/] linked positions", count);
        AnsiConsole.MarkupLine("  • [green]{0}[/] unlinked positions", unlinkedCount);
        AnsiConsole.MarkupLine("Total positions per file: [bold green]{0}[/]", count + unlinkedCount);

        AnsiConsole
            .Progress()
            .AutoClear(true)
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn(),
            })
            .Start(ctx =>
            {
                var task1 = ctx.AddTask("[green]Generating Main Pass[/]");
                var task2 = ctx.AddTask("[green]Generating Unlinked Records[/]");

                for (var i = 0; i < count; i++)
                {
                    var positionA = positionGenerator.Generate(null);
                    var positionB = positionGenerator.Generate(positionA);
                    JsonSerializer.Serialize(jsonWriterA, positionA);
                    JsonSerializer.Serialize(jsonWriterB, positionB);
                    task1.Increment(100.0 / count);
                }
                task1.Value = 100;


                for (var i = 0; i < unlinkedCount; i++)
                {
                    var positionA = positionGenerator.Generate(null);
                    var positionB = positionGenerator.Generate(null);
                    JsonSerializer.Serialize(jsonWriterA, positionA);
                    JsonSerializer.Serialize(jsonWriterB, positionB);
                    task2.Increment(100.0 / unlinkedCount);
                }

                task2.Value = 100;

                jsonWriterA.WriteEndArray();
                jsonWriterB.WriteEndArray();
            });

        AnsiConsole.MarkupLine($"[green]JSON files saved successfully in {directory}.[/]");
        
        // Summarize the generated files
        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("[bold]File Summary:[/]");

        foreach (var file in new[] { sideAPath, sideBPath })
        {
            var fileInfo = new FileInfo(file);
            var fileSize = fileInfo.Length;
            var sizeString = fileSize < 1024 * 1024
                ? $"{fileSize / 1024.0:F2} KB"
                : $"{fileSize / (1024.0 * 1024.0):F2} MB";

            AnsiConsole.MarkupLine($"[blue]{Path.GetFileName(file)}[/]:");
            AnsiConsole.MarkupLine($"  • Size: [green]{sizeString}[/]");
            AnsiConsole.MarkupLine($"  • Total positions: [green]{count + unlinkedCount}[/]");
            AnsiConsole.MarkupLine($"    - Linked: [green]{count}[/]");
            AnsiConsole.MarkupLine($"    - Unlinked: [green]{unlinkedCount}[/]");
            AnsiConsole.MarkupLine("");
        }
    }
}