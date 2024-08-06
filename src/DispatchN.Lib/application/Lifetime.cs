//https://github.com/dotnet/dotnet/blob/main/src/command-line-api/src/System.CommandLine/Invocation/ProcessTerminationHandler.cs
using System.Runtime.InteropServices;

namespace Sorling.DispatchN.Lib.application;

internal sealed class Lifetime : IDisposable
{
   public const int SIGINT_EXIT_CODE = 130;
   public const int SIGTERM_EXIT_CODE = 143;

   internal readonly TaskCompletionSource<int> _processTerminationCompletionSource;
   private readonly CancellationTokenSource _handlerCancellationTokenSource;
   private readonly Task<int> _startedHandler;
   private readonly TimeSpan _processTerminationTimeout;
   private readonly PosixSignalRegistration? _sigIntRegistration, _sigTermRegistration;

   internal Lifetime(
       CancellationTokenSource handlerCancellationTokenSource,
       Task<int> startedHandler,
       TimeSpan processTerminationTimeout) {
      _processTerminationCompletionSource = new();
      _handlerCancellationTokenSource = handlerCancellationTokenSource;
      _startedHandler = startedHandler;
      _processTerminationTimeout = processTerminationTimeout;

      if (!OperatingSystem.IsAndroid()
          && !OperatingSystem.IsIOS()
          && !OperatingSystem.IsTvOS()
          && !OperatingSystem.IsBrowser()) {
         _sigIntRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, OnPosixSignal);
         _sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, OnPosixSignal);
         return;
      }

      Console.CancelKeyPress += OnCancelKeyPress;
      AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
   }

   public void Dispose() {

      if (_sigIntRegistration is not null) {
         _sigIntRegistration.Dispose();
         _sigTermRegistration!.Dispose();
         return;
      }

      Console.CancelKeyPress -= OnCancelKeyPress;
      AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
   }

   void OnPosixSignal(PosixSignalContext context) {
      context.Cancel = true;
      Cancel(context.Signal == PosixSignal.SIGINT ? SIGINT_EXIT_CODE : SIGTERM_EXIT_CODE);
   }

   void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e) {
      e.Cancel = true;
      Cancel(SIGINT_EXIT_CODE);
   }

   void OnProcessExit(object? sender, EventArgs e) => Cancel(SIGTERM_EXIT_CODE);

   void Cancel(int forcedTerminationExitCode) {
      // request cancellation
      Console.WriteLine("");
      Console.WriteLine($"Application termination requsted, forced termination in {_processTerminationTimeout}");
      _handlerCancellationTokenSource.Cancel();

      try {
         // wait for the configured interval
         if (!_startedHandler.Wait(_processTerminationTimeout)) {
            // if the handler does not finish within configured time,
            // use the completion source to signal forced completion (preserving native exit code)
            _processTerminationCompletionSource.SetResult(forcedTerminationExitCode);
            Console.WriteLine("Forced termination");
            Environment.Exit(forcedTerminationExitCode);
         }
      }
      catch (AggregateException) {
         // The task was cancelled or an exception was thrown during the task execution.
      }
   }
}

