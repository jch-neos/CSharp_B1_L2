using Microsoft.ConcurrencyVisualizer.Instrumentation;
using System.Collections.Concurrent;
using System.Linq;public class Program {
  public static void Main() {    ExecutionMode();
  }  internal static void ExecutionMode() {    int count = Environment.ProcessorCount * 2;    using (Markers.EnterSpan(-2, nameof(ParallelExecutionMode.Default))) {      int result = ParallelEnumerable          .Range(0, count)          .SelectMany((value, index) => EnumerableEx.Return(value))          .Visualize(ParallelEnumerable.Select, value => value + ComputingWorkload())          .ElementAt(count - 1);    }    using (Markers.EnterSpan(-3, nameof(ParallelExecutionMode.ForceParallelism))) {      int result = ParallelEnumerable          .Range(0, count)          .WithExecutionMode(ParallelExecutionMode.ForceParallelism)          .SelectMany((value, index) => EnumerableEx.Return(value))          .Visualize(ParallelEnumerable.Select, value => value + ComputingWorkload())          .ElementAt(count - 1);    }  }
  internal static int ComputingWorkload(int value = 0, int baseIteration = 10_000_000) {
    for (int i = 0; i < baseIteration * (value + 1); i++) { }
    return value;
  }
}
static class Extenstions {
  internal const string ParallelSpan = "Parallel";
  internal const string SequentialSpan = "Sequential";
  internal static TResult Visualize<TSource, TMiddle, TResult>(
    this IEnumerable<TSource> source,
    Func<IEnumerable<TSource>, Func<TSource, TMiddle>, TResult> query,
   Func<TSource, TMiddle> iteratee,
   Func<TSource, string> spanFactory = null,
    string span = SequentialSpan) {
    spanFactory = spanFactory ?? (value => value.ToString());
    MarkerSeries markerSeries = Markers.CreateMarkerSeries(span);
    return query(
        source,
        value => {
          using (markerSeries.EnterSpan(
              Thread.CurrentThread.ManagedThreadId, spanFactory(value))) {
            return iteratee(value);
          }
        });
  }
  internal static TResult Visualize<TSource, TMiddle, TResult>(
      this ParallelQuery<TSource> source,
      Func<ParallelQuery<TSource>, Func<TSource, TMiddle>, TResult> query,
     Func<TSource, TMiddle> iteratee,
     Func<TSource, string> spanFactory = null,
      string span = ParallelSpan) {
    spanFactory = spanFactory ?? (value => value.ToString());
    MarkerSeries markerSeries = Markers.CreateMarkerSeries(span);
    return query(
        source,
        value => {
          using (markerSeries.EnterSpan(
              Thread.CurrentThread.ManagedThreadId, spanFactory(value))) {
            return iteratee(value);
          }
        });
  }
  internal static void Visualize<TSource>(
      this IEnumerable<TSource> source,
      Action<IEnumerable<TSource>, Action<TSource>> query,
     Action<TSource> iteratee, string span = SequentialSpan, int category = -1) {
    using (Markers.EnterSpan(category, span)) {
      MarkerSeries markerSeries = Markers.CreateMarkerSeries(span);
      query(
          source,
          value => {
            using (markerSeries.EnterSpan(Thread.CurrentThread.ManagedThreadId, value.ToString())) {
              iteratee(value);
            }
          });
    }
  }
  internal static void Visualize<TSource>(
      this ParallelQuery<TSource> source,
      Action<ParallelQuery<TSource>, Action<TSource>> query,
     Action<TSource> iteratee, string span = ParallelSpan, int category = -2) {
    using (Markers.EnterSpan(category, span)) {
      MarkerSeries markerSeries = Markers.CreateMarkerSeries(span);
      query(
          source,
          value => {
            using (markerSeries.EnterSpan(Thread.CurrentThread.ManagedThreadId, value.ToString())) {
              iteratee(value);
            }
          });
    }
  }
}
