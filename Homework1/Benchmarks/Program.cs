using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Fuse8.BackendInternship.Domain;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;

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



//| Method | word | Mean | Error | StdDev | Median | Ratio | RatioSD | Rank | Gen0 | Allocated | Alloc Ratio |
//| ------------------- | --------------------- | ----------------:| --------------:| --------------:| ----------------:| ------:| --------:| -----:| -------:| ----------:| ------------:|
//| WordIsExists | 146269 | 39.16 ns | 0.758 ns | 1.111 ns | 38.97 ns | 1.00 | 0.00 | 1 | 0.0408 | 128 B | 1.00 |
//| WordIsExistsIntern | 146269 | 113.23 ns | 1.794 ns | 1.678 ns | 112.31 ns | 2.87 | 0.11 | 2 | 0.0408 | 128 B | 1.00 |
//|                    |                      |                 |               |               |                 |       |         |      |        |           |             |
//| WordIsExistsIntern | ёкающий / A | 1,690,283.69 ns | 16,749.223 ns | 15,667.234 ns | 1,684,558.50 ns | 0.73 | 0.01 | 1 | - | 129 B | 0.99 |
//| WordIsExists | ёкающий / A | 2,302,410.10 ns | 22,277.624 ns | 19,748.533 ns | 2,297,837.30 ns | 1.00 | 0.00 | 2 | - | 130 B | 1.00 |
//|                    |                      |                 |               |               |                 |       |         |      |        |           |             |
//| WordIsExistsIntern | полиморфный / A | 880,322.91 ns | 15,112.880 ns | 12,619.939 ns | 886,840.23 ns | 0.76 | 0.01 | 1 | - | 128 B | 1.00 |
//| WordIsExists | полиморфный / A | 1,163,174.33 ns | 9,248.487 ns | 7,722.905 ns | 1,162,991.11 ns | 1.00 | 0.00 | 2 | - | 128 B | 1.00 |
//|                    |                      |                 |               |               |                 |       |         |      |        |           |             |
//| WordIsExistsIntern | cHeBuRashka | 1,752,911.04 ns | 26,233.070 ns | 21,905.802 ns | 1,743,039.55 ns | 0.71 | 0.01 | 1 | - | 129 B | 0.99 |
//| WordIsExists | cHeBuRashka | 2,476,232.19 ns | 15,624.344 ns | 14,615.022 ns | 2,476,919.14 ns | 1.00 | 0.00 | 2 | - | 130 B | 1.00 |
//|                    |                      |                 |               |               |                 |       |         |      |        |           |             |
//| WordIsExistsIntern | HeLlOwORlD2007 =))))) | 1,688,041.27 ns | 31,544.731 ns | 72,479.265 ns | 1,656,928.42 ns | 0.72 | 0.05 | 1 | - | 129 B | 0.99 |
//| WordIsExists | HeLlOwORlD2007 =))))) | 2,418,969.44 ns | 30,971.735 ns | 28,970.980 ns | 2,413,040.43 ns | 1.00 | 0.00 | 2 | - | 130 B | 1.00 |


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
        yield return _words[(_words.Count -1) / 2];
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

//| Method | Mean | Error | StdDev | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
//| ------------------- | ---------:| --------:| ---------:| ------:| --------:| -----:| ----------:| ------------:|
//| CalculatePerformed | 222.6 ns | 4.48 ns | 5.16 ns | 0.53 | 0.02 | 1 | - | NA |
//| Calculate | 424.1 ns | 8.47 ns | 12.93 ns | 1.00 | 0.00 | 2 | - | NA |

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