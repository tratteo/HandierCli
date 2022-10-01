using System.Text;

namespace HandierCli;

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
        blockCount = 10;
        displayParcentage = true;
        animation = new string[] { ">   ", "->  ", "--> ", "--->", " -->", "  ->", "   >", "   <", "  <-", " <--", "<---", "<-- ", "<-  ", "<   " };
        animationInterval = TimeSpan.FromSeconds(1F / 8);
        fillChar = '█';
        leftBorderChar = '|';
        rightBorderChar = '|';
        emptyChar = ' ';
        timer = new Timer(TimerHandler!);
    }

    public static Builder Factory() => new Builder();

    public void Reset()
    {
        lock (timer)
        {
            disposed = true;
            UpdateText(string.Empty);
        }
        Start();
    }

    public void Report(double value, string? infoText = null)
    {
        if (infoText is not null) this.infoText = infoText;
        // Make sure value is in [0..1] range
        value = Math.Max(0, Math.Min(1, value));
        Interlocked.Exchange(ref currentProgress, value);
    }

    public void Dispose()
    {
        lock (timer)
        {
            disposed = true;
            UpdateText(string.Empty);
        }
        GC.SuppressFinalize(this);
    }

    public void Report(double value) => Report(value, null);

    private void Start()
    {
        // A progress bar is only for temporary display in a console window. If the console output is redirected to a file, draw nothing.
        // Otherwise, we'll end up with a lot of garbage in the target file.
        if (!Console.IsOutputRedirected)
        {
            ResetTimer();
        }
    }

    private void TimerHandler(object state)
    {
        lock (timer)
        {
            if (disposed) return;

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

        public Builder()
        {
            bar = new ConsoleProgressBar();
        }

        public static implicit operator ConsoleProgressBar(Builder builder) => builder.Build();

        public Builder Spinner(int fps, params string[] animation)
        {
            bar.animation = animation;
            bar.animationInterval = TimeSpan.FromSeconds(1.0 / fps);
            return this;
        }

        public Builder Lenght(int length)
        {
            bar.blockCount = length;
            return this;
        }

        public Builder HidePercentage()
        {
            bar.displayParcentage = false;
            return this;
        }

        public Builder Style(char fillChar, char emptyChar, char leftBorderChar, char rightBorderChar)
        {
            bar.leftBorderChar = leftBorderChar;
            bar.rightBorderChar = rightBorderChar;
            bar.fillChar = fillChar;
            bar.emptyChar = emptyChar;
            return this;
        }

        public ConsoleProgressBar Build()
        {
            bar.Start();
            return bar;
        }
    }
}