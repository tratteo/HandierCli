// Copyright Matteo Beltrame

using HandierCli.Statics;

namespace HandierCli.Progress;

public class ConsoleSpinner : IDisposable
{
    private string[] animation;
    private string info;
    private float deltaFrame;
    private string completedInfo;
    private CancellationTokenSource? tokenSource;

    private ConsoleSpinner()
    {
        info = "Working ";
        deltaFrame = 1 / 8F;
        animation = new string[] { "-  ", "-- ", "---", " --", "  -", "   ", "  -", " --", "---", "-- ", "-  ", "   " };
        completedInfo = "Job completed";
    }

    /// <summary>
    ///   Create a new instance of a console spinner builder
    /// </summary>
    /// <returns> </returns>
    public static Builder Factory() => new();

    /// <summary>
    ///   Manually start the spinner
    /// </summary>
    public void Start()
    {
        tokenSource?.Cancel();
        tokenSource = new CancellationTokenSource();
        Task.Run(() => RenderTask(tokenSource.Token));
    }

    /// <summary>
    ///   Manually stop the spinner. Does not print the <see cref="completedInfo"/>
    /// </summary>
    public void Stop()
    {
        tokenSource?.Cancel();
        ConsoleExtensions.ClearConsoleLine();
    }

    /// <summary>
    ///   Manually stop the spinner and print the <see cref="completedInfo"/>
    /// </summary>
    /// <param name="text"> </param>
    public void Completed(string? text = null)
    {
        text ??= completedInfo;
        Stop();
        Console.WriteLine(text);
        Console.Out.Flush();
    }

    /// <summary>
    ///   Change the <see cref="info"/>
    /// </summary>
    /// <param name="description"> </param>
    public void Report(string description) => info = description;

    /// <summary>
    ///   Await the specified task, animating the spinner
    /// </summary>
    /// <returns> </returns>
    public async Task Await(Task awaiter)
    {
        Stop();
        Console.Write(info);
        await Task.Run(async () =>
        {
            Start();
            while (!awaiter.IsCompleted)
            {
                await Task.Delay((int)(deltaFrame * 1000));
            }
        });

        Completed(completedInfo);
    }

    /// <inheritdoc cref="Await(Task)"/>
    public async Task<T> Await<T>(Task<T> awaiter)
    {
        Stop();
        Console.Write(info);
        await Task.Run(async () =>
        {
            Start();
            while (!awaiter.IsCompleted)
            {
                await Task.Delay((int)(deltaFrame * 1000));
            }
        });

        Completed(completedInfo);
        return awaiter.Result;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }

    private async Task RenderTask(CancellationToken cancellation)
    {
        while (!cancellation.IsCancellationRequested)
        {
            foreach (var frame in animation)
            {
                await Task.Delay((int)(deltaFrame * 1000), cancellation);
                ConsoleExtensions.ClearConsoleLine();
                Console.Write(info + frame);
                Console.Out.Flush();
            }
        }
    }

    public class Builder
    {
        private readonly ConsoleSpinner awaiter;

        internal Builder()
        {
            awaiter = new ConsoleSpinner();
        }

        public static implicit operator ConsoleSpinner(Builder builder) => builder.Build();

        /// <summary>
        ///   Set the default <see cref="info"/>
        /// </summary>
        /// <returns> </returns>
        public Builder Info(string info)
        {
            awaiter.info = info;
            return this;
        }

        /// <summary>
        ///   Customize the frames of the spinner
        /// </summary>
        /// <returns> </returns>
        public Builder Frames(float frameRate, params string[] frames)
        {
            awaiter.animation = frames;
            awaiter.deltaFrame = 1F / frameRate;
            return this;
        }

        /// <summary>
        ///   Set the default <see cref="completedInfo"/>
        /// </summary>
        /// <returns> </returns>
        public Builder Completed(string completedInfo)
        {
            awaiter.completedInfo = completedInfo;
            return this;
        }

        public ConsoleSpinner Build() => awaiter;
    }
}