using System.Diagnostics;
using System.IO.Compression;

public class Program {
  private const string dataFile = "AllCountries.txt";
  private const string dataZip = "AllCountries.zip";

  public static void Main() {
    EnsureFile();
    for (int i = 0; i < 2; i++) {
      RunTest(parallel: true);
      RunTest(parallel: false);
    }
  }

  private static void EnsureFile() {
    if (!File.Exists(dataFile)) {
      Console.WriteLine("Downloading");
      var hc = new HttpClient();
      using (var file = File.Create(dataZip)) {
        using var stream = hc.GetStreamAsync("http://download.geonames.org/export/dump/allCountries.zip").Result;
        stream.CopyTo(file);
      }
      Console.WriteLine("Extracting");
      ZipFile.ExtractToDirectory(dataZip, Environment.CurrentDirectory);
      Console.WriteLine("Extracted");
      File.Delete(dataZip);
    }

  }

  private static void RunTest(bool parallel = false) {
    var start = new Stopwatch(); start.Start();
    const int nameColumn = 1;
    const int countryColumn = 8;
    const int elevationColumn = 15;
    var lines = File.ReadLines(Path.Combine(Environment.CurrentDirectory, dataFile));
    IEnumerable<(string, int, string)> q;
    if (parallel) {
      q = lines.AsParallel()
            .Select(line => line.Split(new char[] { '\t' }))
            .Select(fields => (fields: fields, elevation: int.TryParse(fields[elevationColumn], out var elevation) ? elevation : 0))
            .Where(fields => fields.elevation > 8000)
            .OrderBy(fields => fields.elevation)
            .Select(fields => (
                name: fields.Item1[nameColumn] ?? "",
                elevation: fields.elevation,
                country: fields.Item1[countryColumn]
              ));
    } else {
      q =
        lines
            .Select(line => line.Split(new char[] { '\t' }))
            .Select(fields => (fields: fields, elevation: int.TryParse(fields[elevationColumn], out var elevation) ? elevation : 0))
            .Where(fields => fields.elevation > 8000)
            .OrderBy(fields => fields.elevation)
            .Select(fields => (
                name: fields.Item1[nameColumn] ?? "",
                elevation: fields.elevation,
                country: fields.Item1[countryColumn]
              ));
    }
    foreach (var x in q) {
      // if (x != null)
      //   Console.WriteLine("{0} ({1} m) - located in {2}", x.name, x.elevation, x.country);

    }
    start.Stop();
    Console.WriteLine($"{(parallel ? "Parallel  " : "Sequential")}: {1E3 * start.ElapsedTicks / Stopwatch.Frequency:0.000}ms elapsed");
  }
}

