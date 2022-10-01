// Copyright Matteo Beltrame

namespace HandierCli;

public class ConsoleSpinner
{
    private readonly List<string> frames;
    private string info;
    private float deltaFrame;
    private string completedInfo;

    private ConsoleSpinner()
    {
        frames = new List<string>();
        info = string.Empty;
        deltaFrame = 1 / 4F;
        frames = new List<string>() { ".", "..", "...", "" };
        completedInfo = "Completed";
    }

    public static Builder Factory() => new();

    public async Task Await(Task awaiter)
    {
        Console.Write(info);
        await Task.Run(async () =>
        {
            while (!awaiter.IsCompleted)
            {
                foreach (var frame in frames)
                {
                    await Task.Delay((int)(deltaFrame * 1000));
                    ConsoleExtensions.ClearConsoleLine();
                    Console.Write(info + frame);
                    Console.Out.Flush();
                }
            }
        });

        ConsoleExtensions.ClearConsoleLine();
        Console.WriteLine(completedInfo);
        Console.Out.Flush();
    }

    public async Task<T> Await<T>(Task<T> awaiter)
    {
        System.Console.Write(info);
        await Task.Run(async () =>
        {
            while (!awaiter.IsCompleted)
            {
                foreach (var frame in frames)
                {
                    await Task.Delay((int)(deltaFrame * 1000));
                    ConsoleExtensions.ClearConsoleLine();
                    Console.Write(info + frame);
                    Console.Out.Flush();
                }
            }
        });

        ConsoleExtensions.ClearConsoleLine();
        Console.WriteLine(completedInfo);
        Console.Out.Flush();
        return awaiter.Result;
    }

    public class Builder
    {
        private readonly ConsoleSpinner awaiter;

        public Builder()
        {
            awaiter = new ConsoleSpinner();
        }

        public static implicit operator ConsoleSpinner(Builder builder) => builder.Build();

        public Builder Info(string info)
        {
            awaiter.info = info;
            return this;
        }

        public Builder Frames(float frameRate, params string[] frames)
        {
            awaiter.frames.Clear();
            awaiter.frames.AddRange(frames);
            awaiter.deltaFrame = 1F / frameRate;
            return this;
        }

        public Builder Completed(string completedInfo)
        {
            awaiter.completedInfo = completedInfo;
            return this;
        }

        public ConsoleSpinner Build() => awaiter;
    }
}