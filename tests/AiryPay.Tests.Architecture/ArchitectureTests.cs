using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;

namespace AiryPay.Tests.Architecture;

public class ArchitectureTests
{
    private const string RootNamespace = "AiryPay.";
    
    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        var layer = Assembly.Load(RootNamespace + nameof(AiryPay.Domain));
        var otherLayers = new[]
        {
            RootNamespace + nameof(AiryPay.Application),
            RootNamespace + nameof(AiryPay.Discord),
            RootNamespace + nameof(AiryPay.Web),
            RootNamespace + nameof(AiryPay.Infrastructure)
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
        var layer = Assembly.Load(RootNamespace + nameof(AiryPay.Application));
        var otherLayers = new[]
        {
            RootNamespace + nameof(AiryPay.Discord),
            RootNamespace + nameof(AiryPay.Web),
            RootNamespace + nameof(AiryPay.Infrastructure)
        };

        var result = Types
            .InAssembly(layer)
            .ShouldNot()
            .HaveDependencyOnAll(otherLayers)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Web_Should_Not_HaveDependencyOnOtherProjects()
    {
        var layer = Assembly.Load(RootNamespace + nameof(AiryPay.Web));
        var otherLayers = new[]
        {
            RootNamespace + nameof(AiryPay.Infrastructure),
            RootNamespace + nameof(AiryPay.Discord),
            RootNamespace + nameof(AiryPay.Domain)
        };

        var result = Types
            .InAssembly(layer)
            .ShouldNot()
            .HaveDependencyOnAll(otherLayers)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Discord_Should_Not_HaveDependencyOnOtherProjects()
    {
        var layer = Assembly.Load(RootNamespace + nameof(AiryPay.Discord));
        var otherLayers = new[]
        {
            RootNamespace + nameof(AiryPay.Infrastructure),
            RootNamespace + nameof(AiryPay.Web),
            RootNamespace + nameof(AiryPay.Domain)
        };

        var result = Types
            .InAssembly(layer)
            .ShouldNot()
            .HaveDependencyOnAll(otherLayers)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}