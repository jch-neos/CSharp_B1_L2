public class Program {
  public static void Main() {
    while (true) {
      var Counter = new SyncedInt();
      const int MAX = 500;
      var threads = new Thread[MAX];
      for (int num = 0; num < MAX; num++) {
        threads[num] = new Thread(new ThreadStart(delegate {
          Thread.Sleep(1);
          Counter.IncrementBy(1);
        }));
        threads[num].Start();
      }
      for (int num = 0; num < MAX; num++) {
        threads[num].Join();
      }
      Console.WriteLine(Counter);
    }
  }
}

public class SyncedInt {
  int value = 0;
  object syncRoot = new();
  int Value => value;
  public void IncrementBy(int increment) {
    lock (syncRoot) {
      value += increment;
    }
  }
  public override string ToString() {
    return value.ToString();
  }
}