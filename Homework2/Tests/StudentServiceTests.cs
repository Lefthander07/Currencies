namespace Fuse8.BackendInternship.Tests;

public class StudentServiceTests
{
    [Fact(DisplayName = "GetBestStudentsFullName корректно возвращает 5 лучших студентов")]
    public void GetFiveBestStudents()
    {
        var group = CreateGroups(count: 1).First();
        Student[] students =
        {
            new() { Id = 1, FirstName = "Vector", LastName = "Obleshev", GroupId = group.Id },
            new() { Id = 2, FirstName = "Antony", LastName = "Hope", GroupId = group.Id },
            new() { Id = 3, FirstName = "Leo", LastName = "DaVinci", GroupId = group.Id },
            new() { Id = 4, FirstName = "Josh", LastName = "Clash", GroupId = group.Id },
            new() { Id = 5, FirstName = "Baobab", LastName = "Tree", GroupId = group.Id },
            new() { Id = 6, FirstName = "Vladimir", LastName = "Pulkov", GroupId = group.Id },
        };
        TestTaskResult[] bestTestTaskResults =
        {
            new(StudentId: students[0].Id, GradeSum: 100, PassedAt: DateTimeOffset.Now.AddSeconds(5)),
            new(StudentId: students[1].Id, GradeSum: 98, PassedAt: DateTimeOffset.Now.AddSeconds(4)),
            new(StudentId: students[2].Id, GradeSum: 78, PassedAt: DateTimeOffset.Now.AddSeconds(3)),
            new(StudentId: students[3].Id, GradeSum: 75, PassedAt: DateTimeOffset.Now.AddSeconds(2)),
            new(StudentId: students[5].Id, GradeSum: 70, PassedAt: DateTimeOffset.Now.AddSeconds(0)),
        };
        var badTestTaskResult = new TestTaskResult(StudentId: students[4].Id, GradeSum: 70, PassedAt: DateTimeOffset.Now.AddSeconds(1));
        var testTaskResults = bestTestTaskResults.Concat(new[] { badTestTaskResult }).ToArray();

        var actualFullNames = StudentService.GetBestStudentsFullName(students, testTaskResults);

        var expectedNames = new [] {"Vector Obleshev", "Antony Hope", "Leo DaVinci", "Josh Clash", "Vladimir Pulkov"};
        Assert.Equal(expected: expectedNames.Length, actualFullNames.Length);
        Assert.Equivalent(expected: expectedNames, actualFullNames);
    }

    [Fact(DisplayName = "GetStudentsFullInfo возвращает информацию обо всех студентах")]
    public void GetStudentsFullInfo()
    {
        var groups = CreateGroups(count: 2);
        Student[] firstGroupStudents =
        {
            new() { Id = 1, FirstName = "Vector", LastName = "Obleshev", GroupId = groups[0].Id },
            new() { Id = 2, FirstName = "Antony", LastName = "Hope", GroupId = groups[0].Id },
        };
        Student[] secondGroupStudents =
        {
            new() { Id = 3, FirstName = "Leo", LastName = "DaVinci", GroupId = groups[1].Id },
            new() { Id = 4, FirstName = "Josh", LastName = "Clash", GroupId = groups[1].Id },
        };
        var students = firstGroupStudents.Concat(secondGroupStudents).ToArray();
        TestTaskResult[] testTaskResults =
        {
            new(StudentId: firstGroupStudents[0].Id, GradeSum: 100, PassedAt: DateTimeOffset.Now.AddSeconds(2)),
            new(StudentId: secondGroupStudents[0].Id, GradeSum: 98, PassedAt: DateTimeOffset.Now.AddSeconds(1)),
        };

        var studentsFullInfo = StudentService.GetStudentsFullInfo(students, testTaskResults, groups);

        StudentFullInfoModel[] expectedStudentsFullInfo =
        {
            new()
            {
                StudentId = firstGroupStudents[0].Id, FirstName = firstGroupStudents[0].FirstName, LastName = firstGroupStudents[0].LastName, GroupId = groups[0].Id,
                GroupName = groups[0].GroupName, TestTaskGradeSum = testTaskResults[0].GradeSum, TestTaskPassedAt = testTaskResults[0].PassedAt
            },
            new()
            {
                StudentId = firstGroupStudents[1].Id, FirstName = firstGroupStudents[1].FirstName, LastName = firstGroupStudents[1].LastName, GroupId = groups[0].Id,
                GroupName = groups[0].GroupName, TestTaskGradeSum = null, TestTaskPassedAt = null
            },
            new()
            {
                StudentId = secondGroupStudents[0].Id, FirstName = secondGroupStudents[0].FirstName, LastName = secondGroupStudents[0].LastName, GroupId = groups[1].Id,
                GroupName = groups[1].GroupName, TestTaskGradeSum = testTaskResults[1].GradeSum, TestTaskPassedAt = testTaskResults[1].PassedAt
            },
            new()
            {
                StudentId = secondGroupStudents[1].Id, FirstName = secondGroupStudents[1].FirstName, LastName = secondGroupStudents[1].LastName, GroupId = groups[1].Id,
                GroupName = groups[1].GroupName, TestTaskGradeSum = null, TestTaskPassedAt = null
            },
        };
        Assert.Equal(expected: expectedStudentsFullInfo.Length, studentsFullInfo.Length);
        Assert.Equivalent(expected: expectedStudentsFullInfo, studentsFullInfo);
    }

    [Fact(DisplayName = "GetBestStudentsByGroup возвращает информацию о лучших студентах в каждой группе")]
    public void GetBestStudentsByGroup()
    {
        var groups = CreateGroups(count: 4);

        StudentFullInfoModel[] students =
        {
            // Оба сдали, второй студент является лучшим, потому что больше баллов
            new() { StudentId = 1, FirstName = "Vector", LastName = "Obleshev", GroupId = groups[0].Id, GroupName = groups[0].GroupName, TestTaskGradeSum = 90, TestTaskPassedAt = DateTimeOffset.Now.AddSeconds(1) },
            new() { StudentId = 2, FirstName = "Antony", LastName = "Hope", GroupId = groups[0].Id, GroupName = groups[0].GroupName, TestTaskGradeSum = 100, TestTaskPassedAt = DateTimeOffset.Now.AddSeconds(2) },

            // Оба сдали, второй студент является лучшим, потому что раньше сдал
            new() { StudentId = 3, FirstName = "Leo", LastName = "DaVinci", GroupId = groups[1].Id, GroupName = groups[1].GroupName, TestTaskGradeSum = 100, TestTaskPassedAt = DateTimeOffset.Now.AddSeconds(4) },
            new() { StudentId = 4, FirstName = "Josh", LastName = "Clash", GroupId = groups[1].Id, GroupName = groups[1].GroupName, TestTaskGradeSum = 100, TestTaskPassedAt = DateTimeOffset.Now.AddSeconds(3) },

            // Сдал только первый - он и является лучшим
            new() { StudentId = 5, FirstName = "Baobab", LastName = "Tree", GroupId = groups[2].Id, GroupName = groups[2].GroupName, TestTaskGradeSum = 99, TestTaskPassedAt = DateTimeOffset.Now.AddSeconds(5) },
            new() { StudentId = 6, FirstName = "Vladimir", LastName = "Pulkov", GroupId = groups[2].Id, GroupName = groups[2].GroupName, TestTaskGradeSum = null, TestTaskPassedAt = null},

            // Никто не сдал, в результаты не должна попасть эта группа
            new() { StudentId = 7, FirstName = "Hermione", LastName = "Granger", GroupId = groups[3].Id, GroupName = groups[3].GroupName, TestTaskGradeSum = null, TestTaskPassedAt = null},
            new() { StudentId = 8, FirstName = "Roman", LastName = "Bellic", GroupId = groups[3].Id, GroupName = groups[3].GroupName, TestTaskGradeSum = null, TestTaskPassedAt = null},
        };

        var bestStudentsByGroup = StudentService.GetBestStudentsByGroup(students);

        var expectedResults = new Dictionary<string, string>
        {
            [groups[0].GroupName] = $"{students[1].FirstName} {students[1].LastName}", // В первой группе второй студент
            [groups[1].GroupName] = $"{students[3].FirstName} {students[3].LastName}", // Во второй группе второй студент
            [groups[2].GroupName] = $"{students[4].FirstName} {students[4].LastName}", // В третьей группе первый студент
        };

        Assert.Equal(expected: expectedResults.Count, bestStudentsByGroup.Count);
        Assert.Equivalent(expected: expectedResults, bestStudentsByGroup);
    }

    [Fact(DisplayName = "GetStudentsWithSameNames возвращает имена тезок (исключая дубликаты)")]
    public void GetStudentsWithSameNames()
    {
        var groups = CreateGroups(count: 2);
        Student[] firstGroupStudents =
        {
            new() { Id = 1, FirstName = "Vector", LastName = "Obleshev", GroupId = groups[0].Id },
            new() { Id = 2, FirstName = "Leo", LastName = "Hope", GroupId = groups[0].Id },
            new() { Id = 3, FirstName = "Josh", LastName = "Tree", GroupId = groups[0].Id },
            new() { Id = 4, FirstName = "Josh", LastName = "Granger", GroupId = groups[0].Id },
        };
        Student[] secondGroupStudents =
        {
            new() { Id = 5, FirstName = "Leo", LastName = "DaVinci", GroupId = groups[1].Id },
            new() { Id = 6, FirstName = "Leo", LastName = "Pulkov", GroupId = groups[1].Id },
            new() { Id = 7, FirstName = "Josh", LastName = "Clash", GroupId = groups[1].Id },
            new() { Id = 8, FirstName = "Roman", LastName = "Bellic", GroupId = groups[1].Id },
        };

        var resultNames = StudentService.GetStudentsWithSameNames(firstGroupStudents, secondGroupStudents);

        var expectedNames = new[] { "Leo", "Josh" };
        Assert.Equal(expected: expectedNames.Length, resultNames.Length);
        Assert.Equivalent(expected: expectedNames, resultNames);
    }

    [Fact(DisplayName = "GetStudentsWithSameNames возвращает пустую коллекцию, если нет пересечений")]
    public void GetStudentsWithSameNames_Empty()
    {
        var groups = CreateGroups(count: 2);
        Student[] firstGroupStudents =
        {
            new() { Id = 1, FirstName = "Vector", LastName = "Obleshev", GroupId = groups[0].Id },
            new() { Id = 2, FirstName = "Vector", LastName = "Hope", GroupId = groups[0].Id },
        };
        Student[] secondGroupStudents =
        {
            new() { Id = 5, FirstName = "Hermione", LastName = "DaVinci", GroupId = groups[1].Id },
            new() { Id = 6, FirstName = "Hermione", LastName = "Pulkov", GroupId = groups[1].Id },
        };

        var resultNames = StudentService.GetStudentsWithSameNames(firstGroupStudents, secondGroupStudents);

        Assert.Empty(resultNames);
    }

    [Fact(DisplayName = "GetAllUniqueStudentNames возвращает коллекцию всех имен людей из обеих групп (без дубликатов)")]
    public void GetAllUniqueStudentNames()
    {
        var groups = CreateGroups(count: 2);
        Student[] firstGroupStudents =
        {
            new() { Id = 1, FirstName = "Vector", LastName = "Obleshev", GroupId = groups[0].Id },
            new() { Id = 2, FirstName = "Leo", LastName = "Hope", GroupId = groups[0].Id },
            new() { Id = 3, FirstName = "Josh", LastName = "Tree", GroupId = groups[0].Id },
            new() { Id = 4, FirstName = "Josh", LastName = "Granger", GroupId = groups[0].Id },
        };
        Student[] secondGroupStudents =
        {
            new() { Id = 5, FirstName = "Leo", LastName = "DaVinci", GroupId = groups[1].Id },
            new() { Id = 6, FirstName = "Leo", LastName = "Pulkov", GroupId = groups[1].Id },
            new() { Id = 7, FirstName = "Josh", LastName = "Clash", GroupId = groups[1].Id },
            new() { Id = 8, FirstName = "Roman", LastName = "Bellic", GroupId = groups[1].Id },
        };

        var resultNames = StudentService.GetAllUniqueStudentNames(firstGroupStudents, secondGroupStudents);

        var expectedNames = new[] { "Vector", "Leo", "Josh", "Roman" };
        Assert.Equal(expected: expectedNames.Length, resultNames.Length);
        Assert.Equivalent(expected: expectedNames, resultNames);
    }

    [Fact(DisplayName = "GetAllUniqueStudentNames возвращает имена только из первой группы, если вторая пустая (без дубликатов)")]
    public void GetAllUniqueStudentNames_OnlyFirstGroup()
    {
        var groups = CreateGroups(count: 1);
        Student[] firstGroupStudents =
        {
            new() { Id = 1, FirstName = "Vector", LastName = "Obleshev", GroupId = groups[0].Id },
            new() { Id = 2, FirstName = "Leo", LastName = "Hope", GroupId = groups[0].Id },
            new() { Id = 3, FirstName = "Josh", LastName = "Tree", GroupId = groups[0].Id },
            new() { Id = 4, FirstName = "Josh", LastName = "Granger", GroupId = groups[0].Id },
        };

        var resultNames = StudentService.GetAllUniqueStudentNames(firstGroupStudents, studentsFromSecondGroup: Array.Empty<Student>());

        var expectedNames = new[] { "Vector", "Leo", "Josh" };
        Assert.Equal(expected: expectedNames.Length, resultNames.Length);
        Assert.Equivalent(expected: expectedNames, resultNames);
    }

    [Fact(DisplayName = "GetAllUniqueStudentNames возвращает имена только из второй группы, если первая пустая (без дубликатов)")]
    public void GetAllUniqueStudentNames_OnlySecondGroup()
    {
        var groups = CreateGroups(count: 1);
        Student[] secondGroupStudents =
        {
            new() { Id = 5, FirstName = "Leo", LastName = "DaVinci", GroupId = groups[0].Id },
            new() { Id = 6, FirstName = "Leo", LastName = "Pulkov", GroupId = groups[0].Id },
            new() { Id = 7, FirstName = "Josh", LastName = "Clash", GroupId = groups[0].Id },
            new() { Id = 8, FirstName = "Roman", LastName = "Bellic", GroupId = groups[0].Id },
        };

        var resultNames = StudentService.GetAllUniqueStudentNames(studentsFromFirstGroup: Array.Empty<Student>(), secondGroupStudents);

        var expectedNames = new[] { "Leo", "Josh", "Roman" };
        Assert.Equal(expected: expectedNames.Length, resultNames.Length);
        Assert.Equivalent(expected: expectedNames, resultNames);
    }

    [Fact(DisplayName = "GetAllUniqueStudentNames возвращает пустую коллекцию, если ни в одной группе нет студента")]
    public void GetAllUniqueStudentNames_Empty()
    {
        var resultNames = StudentService.GetAllUniqueStudentNames(studentsFromFirstGroup: Array.Empty<Student>(), studentsFromSecondGroup: Array.Empty<Student>());

        Assert.Empty(resultNames);
    }

    [Fact(DisplayName = "GetAllStudentNames возвращает имена всех студентов из обоих групп")]
    public void GetAllStudentNames()
    {
        var groups = CreateGroups(count: 2);
        var firstGroupStudents = new GroupWithStudents(
            Students:
            new Student[]
            {
                new() { Id = 1, FirstName = "Vector", LastName = "Obleshev", GroupId = groups[0].Id },
                new() { Id = 2, FirstName = "Leo", LastName = "Hope", GroupId = groups[0].Id },
            });
        var secondGroupStudents = new GroupWithStudents(
            Students:
            new Student[]
            {
                new() { Id = 5, FirstName = "Leo", LastName = "DaVinci", GroupId = groups[1].Id },
                new() { Id = 6, FirstName = "Leo", LastName = "DaVinci", GroupId = groups[1].Id },
                new() { Id = 7, FirstName = "Josh", LastName = "Clash", GroupId = groups[1].Id },
            });

        var resultNames = StudentService.GetAllStudentNames(new []{firstGroupStudents, secondGroupStudents});

        var expectedNames = new[] { "Vector Obleshev", "Leo Hope", "Leo DaVinci", "Leo DaVinci", "Josh Clash" };
        Assert.Equal(expected: expectedNames.Length, resultNames.Length);
        Assert.Equivalent(expected: expectedNames, resultNames);
    }

    [Fact(DisplayName = "GetAllStudentNames возвращает имена только из первой группы, если вторая пустая")]
    public void GetAllStudentNames_OnlyFirstGroup()
    {
        var groups = CreateGroups(count: 1);
        var firstGroupStudents = new GroupWithStudents(
            Students:
            new Student[]
            {
                new() { Id = 1, FirstName = "Vector", LastName = "Obleshev", GroupId = groups[0].Id },
                new() { Id = 2, FirstName = "Leo", LastName = "Hope", GroupId = groups[0].Id },
            });
        var secondGroupStudents = new GroupWithStudents(Students: Array.Empty<Student>());

        var resultNames = StudentService.GetAllStudentNames(new []{firstGroupStudents, secondGroupStudents});

        var expectedNames = new[] { "Vector Obleshev", "Leo Hope" };
        Assert.Equal(expected: expectedNames.Length, resultNames.Length);
        Assert.Equivalent(expected: expectedNames, resultNames);
    }

    [Fact(DisplayName = "GetAllStudentNames возвращает имена только из второй группы, если первая пустая")]
    public void GetAllStudentNames_OnlySecondGroup()
    {
        var groups = CreateGroups(count: 1);
        var firstGroupStudents = new GroupWithStudents(Students: Array.Empty<Student>());
        var secondGroupStudents = new GroupWithStudents(
            Students:
            new Student[]
            {
                new() { Id = 5, FirstName = "Leo", LastName = "DaVinci", GroupId = groups[0].Id },
                new() { Id = 6, FirstName = "Leo", LastName = "DaVinci", GroupId = groups[0].Id },
                new() { Id = 7, FirstName = "Josh", LastName = "Clash", GroupId = groups[0].Id },
            });

        var resultNames = StudentService.GetAllStudentNames(new []{firstGroupStudents, secondGroupStudents});

        var expectedNames = new[] { "Leo DaVinci", "Leo DaVinci", "Josh Clash" };
        Assert.Equal(expected: expectedNames.Length, resultNames.Length);
        Assert.Equivalent(expected: expectedNames, resultNames);
    }

    [Fact(DisplayName = "GetAllStudentNames возвращает пустую коллекцию, если ни в одной группе нет студента")]
    public void GetAllStudentNames_Empty()
    {
        var firstGroupStudents = new GroupWithStudents(Students: Array.Empty<Student>());
        var secondGroupStudents = new GroupWithStudents(Students: Array.Empty<Student>());

        var resultNames = StudentService.GetAllStudentNames(new []{firstGroupStudents, secondGroupStudents});

        Assert.Empty(resultNames);
    }

    private static Group[] CreateGroups(int count)
    {
        var groups = new List<Group>();
        for (var index = 1; index <= count; index++)
        {
            groups.Add(new Group(Id: index, GroupName: $"Group {index}"));
        }

        return groups.ToArray();
    }
}