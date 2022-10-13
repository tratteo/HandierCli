using System.Text;

namespace HandierCli.Progress;

/// <summary>
///   Console progress bar. Credits to Daniel Wolf, MIT License
/// </summary>
public class ConsoleProgressBar : IDisposable, IProgress<double>
{
    private readonly Timer timer;
    private TimeSpan animationInterval;
    private int blockCount;
    private string[] animation;

    private double currentProgress;
    private string currentText = string.Empty;

    private bool stopped;
    private bool disposed;
    private int animationIndex;
    private char fillChar;
    private char emptyChar;
    private char leftBorderChar;
    private char rightBorderChar;
    private string infoText;
    private bool displayParcentage;

    private ConsoleProgressBar()
    {
        infoText = string.Empty;
        blockCount = 20;
        displayParcentage = true;
        animation = new string[] { ">   ", "->  ", "--> ", "--->", " -->", "  ->", "   >", "   <", "  <-", " <--", "<---", "<-- ", "<-  ", "<   " };
        animationInterval = TimeSpan.FromSeconds(1F / 8);
        fillChar = '█';
        leftBorderChar = '|';
        rightBorderChar = '|';
        emptyChar = ' ';
        timer = new Timer(TimerHandler!);
    }

    /// <summary>
    ///   Create an instance of a new progress bar builder
    /// </summary>
    /// <returns> </returns>
    public static Builder Factory() => new Builder();

    /// <summary>
    ///   Reset the status of the progress bar
    /// </summary>
    public void Reset()
    {
        lock (timer)
        {
            UpdateText(string.Empty);
            currentProgress = 0;
        }
        stopped = true;
    }

    /// <summary>
    ///   Report to the progress bar
    /// </summary>
    public void Report(double value, string? description = null)
    {
        if (description is not null) this.infoText = description;
        // Make sure value is in [0..1] range
        value = Math.Max(0, Math.Min(1, value));
        Interlocked.Exchange(ref currentProgress, value);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (timer)
        {
            disposed = true;
            UpdateText(string.Empty);
        }
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Report(double, string?)"/>
    public void Report(double value) => Report(value, null);

    /// <summary>
    ///   Start the rendering of the progress bar
    /// </summary>
    public void Start()
    {
        // A progress bar is only for temporary display in a console window. If the console output is redirected to a file, draw nothing.
        // Otherwise, we'll end up with a lot of garbage in the target file.
        if (!Console.IsOutputRedirected)
        {
            stopped = false;
            ResetTimer();
        }
    }

    private void TimerHandler(object state)
    {
        lock (timer)
        {
            if (disposed) return;
            if (!stopped)
            {
                var progressBlockCount = (int)(currentProgress * blockCount);
                var percent = (int)(currentProgress * 100);
                var text = string.Empty;
                if (displayParcentage)
                {
                    text = string.Format("{0}{1}{2}{3} {4,3}% {5} {6}",
                       leftBorderChar,
                       new string(fillChar, progressBlockCount),
                       new string(emptyChar, blockCount - progressBlockCount),
                       rightBorderChar,
                       percent,
                       animation.Length > 0 ? animation[animationIndex++ % animation.Length] : "",
                       infoText);
                }
                else
                {
                    text = string.Format("{0}{1}{2}{3} {4} {5}",
                       leftBorderChar,
                       new string(fillChar, progressBlockCount),
                       new string(emptyChar, blockCount - progressBlockCount),
                       rightBorderChar,
                       animation.Length > 0 ? animation[animationIndex++ % animation.Length] : "",
                       infoText);
                }
                UpdateText(text);
            }
            ResetTimer();
        }
    }

    private void UpdateText(string text)
    {
        // Get length of common portion
        var commonPrefixLength = 0;
        var commonLength = Math.Min(currentText.Length, text.Length);
        while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength])
        {
            commonPrefixLength++;
        }

        // Backtrack to the first differing character
        var outputBuilder = new StringBuilder();
        outputBuilder.Append('\b', currentText.Length - commonPrefixLength);

        // Output new suffix
        outputBuilder.Append(text.AsSpan(commonPrefixLength));

        // If the new text is shorter than the old one: delete overlapping characters
        var overlapCount = currentText.Length - text.Length;
        if (overlapCount > 0)
        {
            outputBuilder.Append(' ', overlapCount);
            outputBuilder.Append('\b', overlapCount);
        }

        Console.Write(outputBuilder);
        currentText = text;
    }

    private void ResetTimer() => timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));

    public class Builder
    {
        private readonly ConsoleProgressBar bar;
        private bool startOnBuild = false;

        internal Builder()
        {
            bar = new ConsoleProgressBar();
        }

        public static implicit operator ConsoleProgressBar(Builder builder) => builder.Build();

        /// <summary>
        ///   Customize the appearance of the spinner at the end of the bar
        /// </summary>
        /// <returns> </returns>
        public Builder Spinner(int fps, params string[] animation)
        {
            bar.animation = animation;
            bar.animationInterval = TimeSpan.FromSeconds(1.0 / fps);
            return this;
        }

        /// <summary>
        ///   Define the block length of the bar
        /// </summary>
        /// <returns> </returns>
        public Builder Lenght(int length)
        {
            bar.blockCount = length;
            return this;
        }

        /// <summary>
        ///   Hide the percentage status
        /// </summary>
        /// <returns> </returns>
        public Builder HidePercentage()
        {
            bar.displayParcentage = false;
            return this;
        }

        /// <summary>
        ///   Customize the style of the progress bar
        /// </summary>
        /// <returns> </returns>
        public Builder Style(char fillChar, char emptyChar, char leftBorderChar, char rightBorderChar)
        {
            bar.leftBorderChar = leftBorderChar;
            bar.rightBorderChar = rightBorderChar;
            bar.fillChar = fillChar;
            bar.emptyChar = emptyChar;
            return this;
        }

        /// <summary>
        ///   Start the progress bar rendering when the progress bar is built
        /// </summary>
        /// <returns> </returns>
        public Builder StartOnBuild()
        {
            startOnBuild = true;
            return this;
        }

        public ConsoleProgressBar Build()
        {
            if (startOnBuild)
            {
                bar.Start();
            }
            return bar;
        }
    }
}