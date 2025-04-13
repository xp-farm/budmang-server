using System.Reflection;
using System.Text.RegularExpressions;
using ArchUnitNET.Loader;
using ArchDomain = ArchUnitNET.Domain;

namespace Budmang.Architecture.Tests.Infrastructure;

public abstract class BaseArchitecturalTests
{
    protected static readonly List<string> Modules =
    [
        "Stakeholders",
        "Budgets"
    ];
    protected readonly ArchDomain.Architecture Architecture;

    protected BaseArchitecturalTests()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        foreach (var dll in Directory.GetFiles(path!, "Budmang.*.dll"))
        {
            Assembly.LoadFile(dll);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        Architecture = new ArchLoader().LoadAssemblies(assemblies
            .Where(a => a.FullName!.StartsWith("Budmang"))
            .Select(a => Assembly.Load(a.FullName!))
            .ToArray()
        ).Build();
    }

    protected IEnumerable<ArchDomain.IType> GetExaminedTypes(string assemblyName)
    {
        return Architecture.Assemblies
            .Where(a => Regex.IsMatch(a.FullName, assemblyName))
            .SelectMany(a => Architecture.Types.Where(c => c.Assembly.Equals(a)));
    }

    protected IEnumerable<ArchDomain.IType> GetForbiddenTypes(params string[] exemptAssemblyNames)
    {
        return Architecture.Assemblies
            .Where(a => exemptAssemblyNames.All(n => !Regex.IsMatch(a.FullName, n)))
            .SelectMany(a => Architecture.Types.Where(c => c.Assembly.Equals(a)));
    }
}
