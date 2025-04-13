using ArchUnitNET.xUnit;
using Budmang.Architecture.Tests.Infrastructure;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Budmang.Architecture.Tests;

public class LayerTests : BaseArchitecturalTests
{
    [Theory]
    [MemberData(nameof(GetModules))]
    public void Api_projects_should_only_reference_themselves_and_domain_common_blocks(string moduleName)
    {
        var examinedTypes = GetExaminedTypes($"Budmang.{moduleName}.Api");
        var forbiddenTypes = GetForbiddenTypes("Budmang.Common.Domain", $"Budmang.{moduleName}.Api");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes).WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Domain_projects_should_only_reference_themselves_Api_projects_and_domain_common_blocks(string moduleName)
    {
        var examinedTypes = GetExaminedTypes($"Budmang.{moduleName}.Domain");
        var forbiddenTypes = GetForbiddenTypes("Budmang.Common.Domain", "Budmang\\..+\\.Api", $"Budmang.{moduleName}.Domain");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes).WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Infra_projects_should_only_reference_themselves_their_Api_and_domain_projects_and_common_blocks(
        string moduleName)
    {
        var examinedTypes = GetExaminedTypes($"Budmang.{moduleName}.Infrastructure");
        var forbiddenTypes = GetForbiddenTypes("Budmang.Common.Domain", $"Budmang.{moduleName}.Domain");

        var rule = Types().That().Are(examinedTypes).Should().NotDependOnAny(forbiddenTypes).WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Domain_namespaces_should_only_reference_themselves_and_domain_common_blocks(string moduleName)
    {
        var allTypesFromCoreAssembly = GetExaminedTypes($"Budmang.{moduleName}.Domain").ToList();
        var domainTypes = allTypesFromCoreAssembly.Where(x => x.FullName.Contains(".Domain.")).ToList();
        var nonDomainTypes = allTypesFromCoreAssembly.Where(x => !x.FullName.Contains(".Domain."));
        var typesFromOtherAssemblies = GetForbiddenTypes("Budmang.Common.Domain", $"Budmang.{moduleName}.Domain");

        var otherAssemblyRule = Types().That().Are(domainTypes).Should().NotDependOnAny(typesFromOtherAssemblies).WithoutRequiringPositiveResults();
        var sameAssemblyRule = Types().That().Are(domainTypes).Should().NotDependOnAny(nonDomainTypes).WithoutRequiringPositiveResults();

        otherAssemblyRule.Check(Architecture);
        sameAssemblyRule.Check(Architecture);
    }

    [Theory]
    [MemberData(nameof(GetModules))]
    public void Services_should_not_reference_public_Apis_of_other_modules(string moduleName)
    {
        var allTypesFromCoreAssembly = GetExaminedTypes($"Budmang.{moduleName}.Domain").ToList();
        var useCaseTypes = allTypesFromCoreAssembly.Where(x => x.FullName.Contains(".UseCases.")).ToList();
        var typesFromOtherAssemblies = GetForbiddenTypes("Budmang.Api", $"Budmang.{moduleName}.Api");
        var publicApiTypesFromOtherAssemblies = typesFromOtherAssemblies.Where(x => x.FullName.Contains("Api.Public"));

        var rule = Types().That().Are(useCaseTypes).Should().NotDependOnAny(publicApiTypesFromOtherAssemblies).WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void Web_Api_should_not_reference_internal_Apis_of_modules()
    {
        var apiTypes = GetExaminedTypes("Budmang.Api").ToList();
        var typesFromOtherAssemblies = GetForbiddenTypes("Budmang.Api");
        var internalApiTypes = typesFromOtherAssemblies.Where(x => x.FullName.Contains("Api.Internal"));

        var rule = Types().That().Are(apiTypes).Should().NotDependOnAny(internalApiTypes);

        rule.Check(Architecture);
    }

    public static IEnumerable<object[]> GetModules() => Modules.Select(x => new object[] { x }).ToList();
}
