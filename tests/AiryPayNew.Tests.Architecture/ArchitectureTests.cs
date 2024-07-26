using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;

namespace AiryPayNew.Tests.Architecture;

public class ArchitectureTests
{
    private const string RootNamespace = "AiryPayNew.";
    
    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        var layer = Assembly.Load(RootNamespace + nameof(Domain));
        var otherLayers = new[]
        {
            RootNamespace + nameof(Application),
            RootNamespace + nameof(Presentation),
            RootNamespace + nameof(Infrastructure)
        };

        var result = Types
            .InAssembly(layer)
            .ShouldNot()
            .HaveDependencyOnAll(otherLayers)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherProjects()
    {
        var layer = Assembly.Load(RootNamespace + nameof(Application));
        var otherLayers = new[]
        {
            RootNamespace + nameof(Presentation),
            RootNamespace + nameof(Infrastructure)
        };

        var result = Types
            .InAssembly(layer)
            .ShouldNot()
            .HaveDependencyOnAll(otherLayers)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Presentation_Should_Not_HaveDependencyOnOtherProjects()
    {
        var layer = Assembly.Load(RootNamespace + nameof(Presentation));
        var otherLayers = new[]
        {
            RootNamespace + nameof(Infrastructure),
            RootNamespace + nameof(Domain)
        };

        var result = Types
            .InAssembly(layer)
            .ShouldNot()
            .HaveDependencyOnAll(otherLayers)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}