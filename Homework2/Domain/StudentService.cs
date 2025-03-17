namespace Fuse8.BackendInternship.Domain;

public class StudentService
{
    /// <summary>
    /// Возвращает 5 лучших студентов по кол-ву баллов
    /// </summary>
    /// <param name="students">Студенты, сдавшие тестовое задание</param>
    /// <param name="testTaskResults">Результаты проверки тестовых заданий</param>
    /// <remarks>
    /// Для каждого студента из коллекции <paramref name="students"/> есть ровно один результат в коллекции <paramref name="testTaskResults"/>.
    /// Если баллы совпадают, то лучше будет тот, кто сдал раньше. При этом время сдачи тестового задание уникально среди всех студентов
    /// </remarks>
    /// <returns>
    /// Имена студентов в формате "{FirstName} {LastName}"
    /// </returns>
    public static string[] GetBestStudentsFullName(Student[] students, TestTaskResult[] testTaskResults)
    {
        // TODO: реализовать логику с использованием LINQ без создания дополнительных коллекций (HashSet и т.д.)

        const int stateFundedStudentQuantity = 5;
        return Array.Empty<string>();
    }

    /// <summary>
    /// Получает полную информацию по каждому студенту
    /// </summary>
    /// <param name="students">Студенты, сдавшие тестовое задание</param>
    /// <param name="testTaskResults">Результаты проверки тестовых заданий</param>
    /// <param name="groups">Группы, в которых состоят студенты</param>
    /// <remarks>
    /// Каждый студент из коллекции <paramref name="students"/> есть ровно в одной группе из в коллекции <paramref name="groups"/>, но не все сдали тестовые задания.
    /// Поэтому не для всех студентов из коллекции <paramref name="students"/> может быть соответствующая запись в коллекции <paramref name="testTaskResults"/>
    /// </remarks>
    /// <returns>
    /// Полную информацию по каждому студенту
    /// </returns>
    public static StudentFullInfoModel[] GetStudentsFullInfo(Student[] students, TestTaskResult[] testTaskResults, Group[] groups)
    {
        // TODO: реализовать логику с использованием LINQ без создания дополнительных коллекций (HashSet и т.д.)

        return Array.Empty<StudentFullInfoModel>();
    }

    /// <summary>
    /// Получает информацию о студенте, который лучше всех сдал тестовое задание в каждой группе
    /// </summary>
    /// <param name="students">Студенты, сдавшие тестовое задание</param>
    /// <remarks>
    /// Не каждый студент сдал тестовое задание.
    /// У не сдавших будет null в значениях <see cref="StudentFullInfoModel.TestTaskGradeSum"/> и <see cref="StudentFullInfoModel.TestTaskPassedAt"/>.
    /// Если баллы совпадают, то лучше будет тот, кто сдал раньше. При этом время сдачи тестового задание уникально среди всех студентов
    /// </remarks>
    /// <returns>
    /// Словарь, ключом которого является имя группы (<see cref="StudentFullInfoModel.GroupName"/>),
    /// а значением является полное имя студента в формате "{FirstName} {LastName}"
    /// </returns>
    public static Dictionary<string, string> GetBestStudentsByGroup(StudentFullInfoModel[] students)
    {
        // TODO: реализовать логику с использованием LINQ без создания дополнительных коллекций (HashSet и т.д.)

        return new Dictionary<string, string>();
    }

    /// <summary>
    /// Получает список имен, которые есть в обеих группах.
    /// </summary>
    /// <param name="studentsFromFirstGroup">Студенты из первой группы</param>
    /// <param name="studentsFromSecondGroup">Студенты из второй группы</param>
    /// <remarks>
    /// По сути находит тезок в обеих группах. Если тезок не будет, то должен вернуться пустой массив.
    /// </remarks>
    /// <returns>
    /// Список уникальных имен (<see cref="Student.FirstName"/>), которые встречаются в каждой группе
    /// </returns>
    public static string[] GetStudentsWithSameNames(Student[] studentsFromFirstGroup, Student[] studentsFromSecondGroup)
    {
        // TODO: реализовать логику с использованием LINQ без создания дополнительных коллекций (HashSet и т.д.)

        return Array.Empty<string>();
    }

    /// <summary>
    /// Получает дедуплицированный список всех имен студентов из обеих групп
    /// </summary>
    /// <param name="studentsFromFirstGroup">Студенты из первой группы</param>
    /// <param name="studentsFromSecondGroup">Студенты из второй группы</param>
    /// <remarks>
    /// Если ни в одной группе не будет студентов, то должен вернуться пустой массив
    /// </remarks>
    /// <returns>
    /// Дедуплицированный список всех имен (<see cref="Student.FirstName"/>)
    /// </returns>
    public static string[] GetAllUniqueStudentNames(Student[] studentsFromFirstGroup, Student[] studentsFromSecondGroup)
    {
        // TODO: реализовать логику. Из LINQ операторов можно использовать только "Select". Можно использовать дополнительные коллекции (HashSet и т.д.)

        return Array.Empty<string>();
    }

    /// <summary>
    /// Получает все полные имена студентов из всех групп
    /// </summary>
    /// <param name="groupWithStudents">Список групп со студентами</param>
    /// <returns>
    /// Полные имена студентов в формате "{FirstName} {LastName}".
    /// Дубликаты НЕ убираются
    /// </returns>
    public static string[] GetAllStudentNames(GroupWithStudents[] groupWithStudents)
    {
        // TODO: реализовать логику с использованием LINQ без создания дополнительных коллекций (HashSet и т.д.)

        return Array.Empty<string>();
    }
}

public record Student
{
    public required int Id { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required int GroupId { get; init; }
}

public record TestTaskResult(int StudentId, int GradeSum, DateTimeOffset PassedAt);

public record Group(int Id, string GroupName);

public record StudentFullInfoModel
{
    public required int StudentId { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required int? TestTaskGradeSum { get; init; }

    public required DateTimeOffset? TestTaskPassedAt { get; init; }

    public required int GroupId { get; init; }

    public required string GroupName { get; init; }
}

public record GroupWithStudents(Student[] Students);