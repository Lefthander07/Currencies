using System.Reflection;
using System.Security.AccessControl;

namespace Fuse8.BackendInternship.Domain;

public static class AssemblyHelpers
{
	/// <summary>
	/// Получает информацию о базовых типах классов из namespace "Fuse8.BackendInternship.Domain", у которых есть наследники.
	/// </summary>
	/// <remarks>
	///	Информация возвращается только по самым базовым классам.
	/// Информация о промежуточных базовых классах не возвращается
	/// </remarks>
	/// <returns>Список типов с количеством наследников</returns>
	public static (string BaseTypeName, int InheritorCount)[] GetTypesWithInheritors()
	{
        var assemblyClassTypes = Assembly.GetAssembly(typeof(AssemblyHelpers))
        !.DefinedTypes
        .Where(p => p.IsClass && p.Namespace == typeof(AssemblyHelpers).Namespace && !p.IsAbstract).ToArray();

        var baseTypes = new Dictionary<Type, List<Type>>();
		foreach (var type in assemblyClassTypes)
		{
                var baseType = GetBaseType(type);
            if (baseType != null && baseType.Namespace == typeof(AssemblyHelpers).Namespace)
			{
                    if (!baseTypes.ContainsKey(baseType))
                    {
                        baseTypes[baseType] = new List<Type>();
                    }
                    baseTypes[baseType].Add(type);
            }
        }

        // Формируем результат — кортежи с базовыми типами и количеством их наследников
        return baseTypes
            .Select(entry => (entry.Key.Name, entry.Value.Count))
            .ToArray();
    }

    /// <summary>
    /// Получает базовый тип для класса
    /// </summary>
    /// <param name="type">Тип, для которого необходимо получить базовый тип</param>
    /// <returns>
    /// Первый тип в цепочке наследований. Если наследования нет, возвращает null
    /// </returns>
    /// <example>
    /// Класс A, наследуется от B, B наследуется от C
    /// При вызове GetBaseType(typeof(A)) вернется C
    /// При вызове GetBaseType(typeof(B)) вернется C
    /// При вызове GetBaseType(typeof(C)) вернется C
    /// </example>
    private static Type? GetBaseType(Type type)
	{
		var baseType = type;

		while (baseType.BaseType is not null && baseType.BaseType != typeof(object))
		{
			baseType = baseType.BaseType;
		}

		return baseType == type
			? null
			: baseType;
	}
}