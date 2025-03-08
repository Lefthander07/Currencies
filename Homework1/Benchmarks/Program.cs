using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Fuse8.BackendInternship.Domain;

BenchmarkRunner.Run<StringInternBenchmark>();
BenchmarkRunner.Run<CalculateBenchmark>();
/*
 * Результаты бенчмарка StringInternBenchmark позволяют сделать вывод, что WordIsExistsIntern практически
 * всегда работает быстрее, чем WordIsExists.
 * Это связано с тем WordIsExistsIntern использует интернированные строки 
 * и сравнивает ссылки через ReferenceEquals, что должно быть быстрее,
 * так как не нужно проводить полное сравнение содержимого строк.
 * 
 * Однако для первой строки из словаря WordIsExists сработал быстрее, чем WordIsExistsIntern.
 * Поиск в интернете привел меня к следующему выводу: пул интернированных строк может быть размещен в памяти таким образом, 
 * что первое обращение к нему может быть более затратным. В вашем случае это может происходить, 
 * если строка первого слова уже присутствует в пуле, 
 * и сам процесс её поиска и ассоциации с пулом может добавлять задержки.
 */


[MemoryDiagnoser(displayGenColumns: true)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class StringInternBenchmark
{
    private readonly List<string> _words = new();
    public StringInternBenchmark()
    {
        foreach (var word in File.ReadLines(@".\SpellingDictionaries\ru_RU.dic"))
            _words.Add(string.Intern(word));
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(SampleData))]
    public bool WordIsExists(string word)
        => _words.Any(item => word.Equals(item, StringComparison.Ordinal));

    [Benchmark]
    [ArgumentsSource(nameof(SampleData))]
    public bool WordIsExistsIntern(string word)
    {
        var internedWord = string.Intern(word);
        return _words.Any(item => ReferenceEquals(internedWord, item));
    }

    public IEnumerable<string> SampleData()
    {
        var sb1 = new StringBuilder();
        sb1.Append("HeLlOwORlD2007=)))))");
        var string_from_sb1 = sb1.ToString();

        var sb2 = new StringBuilder();
        sb2.Append("cHeBuRashka");
        var string_from_sb2 = sb2.ToString();

        // Возвращаем эти строки
        yield return _words.First();
        yield return _words[_words.Count - 1 / 2];
        yield return _words.Last();

        yield return string_from_sb1;
        yield return string_from_sb2;
    }
}


/*
 * Результаты бенчмарка CalculateBenchmark позволяют сделать вывод, что CalculatePerformed
 * выполняется быстрее, чем Calculate. Это связано с тем, что значимые типы передаются по ссылке, 
 * а не по значению. Это позволяет избежать дорогостоящей в плане времени операции копирования.
 */
[MemoryDiagnoser(displayGenColumns: true)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class CalculateBenchmark
{
    private AccountProcessor _accountProcessor;
    private BankAccount _bankAccount;

    public CalculateBenchmark()
    {
        _accountProcessor = new AccountProcessor();
        _bankAccount = new BankAccount
        {
            TotalAmount = 100,
            LastOperation = new BankOperation { OperationInfo0 = 100, OperationInfo1 = 200, OperationInfo2 = 300},
            PreviousOperation = new BankOperation { OperationInfo0 = 500, OperationInfo1 = 600, OperationInfo2 = 700 }
        };

    }

    [Benchmark(Baseline = true)]
    public decimal Calculate()
    {
        return _accountProcessor.Calculate(_bankAccount);
    }


    [Benchmark]
    public decimal CalculatePerformed()
    {
        return _accountProcessor.CalculatePerformed(_bankAccount);
    }
}