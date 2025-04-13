using ArchUnitNET.xUnit;
using Budmang.Architecture.Tests.Infrastructure;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Budmang.Architecture.Tests;

public class CommonTests: BaseArchitecturalTests
{
    [Fact]
    public void Domain_should_not_reference_other_projects()
    {
        var examinedTypes = GetExaminedTypes("Budmang.Common.Domain");
        var forbiddenTypes = GetForbiddenTypes("Budmang.Common.Domain");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes).WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void Infrastructure_should_not_reference_other_projects_apart_from_domain()
    {
        var examinedTypes = GetExaminedTypes("Budmang.Common.Infrastructure");
        var forbiddenTypes = GetForbiddenTypes("Budmang.Common.Infrastructure", "Budmang.Common.Domain");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes).WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }
}
