using System.Collections.Concurrent;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class Program {
  public static void Main(string[] args) {
    BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
  }
}

public class Tests {

  [Params(1000, 10_000, 100_000, 1_000_000, 10_000_000, 100_000_000)]
  public int Count { get; set; }
  IEnumerable<int> values = Enumerable.Empty<int>();
  [GlobalSetup]
  public void Setup() {
    values = Enumerable.Range(1, Count);
  }
  [Benchmark()]
  public double ParallelPi2() {
    double res = 0;
    foreach (var item in values.AsParallel().Select(n => (1.0 - (2 * (n & 1))) / (1.0 * n * n))) {
      res += item;
    }
    return -12 * res;
  }
  [Benchmark]
  public double SequentialPi2() {
    double res = 0;
    foreach (var item in values.AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism).Select(n => (1.0 - (2 * (n & 1))) / (1.0 * n * n))) {
      res += item;
    }
    return -12 * res;
  }
}
