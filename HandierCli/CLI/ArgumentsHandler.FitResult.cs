// Copyright Matteo Beltrame

namespace HandierCli.CLI;

public partial class ArgumentsHandler
{
    public class FitResult
    {
        public bool Successful { get; init; }

        /// <summary>
        ///   List of all the arguments that had a failure in fitting the string values
        /// </summary>
        public List<(Argument, string)> FailedFits { get; init; }

        public string Reason { get; init; }

        private FitResult()
        {
            Successful = false;
            FailedFits = new List<(Argument, string)>();
            Reason = string.Empty;
        }

        public static FitResult Success() => new FitResult() { Successful = true };

        public static FitResult Failure(List<(Argument, string)> failed, string reason) => new FitResult() { Successful = false, FailedFits = failed, Reason = reason };

        public static FitResult Failure(List<(Argument, string)> failed) => Failure(failed, string.Empty);

        public static FitResult Failure(string reason) => Failure(new List<(Argument, string)>(), reason);
    }
}