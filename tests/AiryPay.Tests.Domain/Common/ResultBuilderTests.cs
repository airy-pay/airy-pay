using AiryPay.Domain.Common.Result;

namespace AiryPay.Tests.Domain.Common;

public class ResultBuilderTests
{
    [Fact]
    public void ResultBuilderOfTErrorType_WithSuccess_ShouldBuildSuccessfulResult()
    {
        var builder = new ResultBuilder<TestErrorType>();

        var result = builder.WithSuccess().Build();

        result.Successful.Should().BeTrue();
        result.Failed.Should().BeFalse();
    }

    [Fact]
    public void ResultBuilderOfTErrorType_WithError_ShouldBuildFailedResultWithErrorType()
    {
        var builder = new ResultBuilder<TestErrorType>();

        var result = builder.WithError(TestErrorType.NotFound).Build();

        result.Successful.Should().BeFalse();
        result.Failed.Should().BeTrue();
        result.ErrorType.Should().Be(TestErrorType.NotFound);
    }

    [Fact]
    public void ResultBuilderOfTErrorType_ImplicitConversion_ShouldWork()
    {
        Result<TestErrorType> result = new ResultBuilder<TestErrorType>().WithSuccess();

        result.Successful.Should().BeTrue();
    }

    [Fact]
    public void ResultBuilderOfTEntityTErrorType_WithSuccess_ShouldBuildSuccessfulResultWithEntity()
    {
        var entity = new TestEntity { Id = 42 };
        var builder = new ResultBuilder<TestEntity, TestErrorType>(entity);

        var result = builder.WithSuccess().Build();

        result.Successful.Should().BeTrue();
        result.Entity.Should().Be(entity);
    }

    [Fact]
    public void ResultBuilderOfTEntityTErrorType_WithSuccessEntity_ShouldUpdateEntityAndBuildSuccess()
    {
        var entity = new TestEntity { Id = 42 };
        var builder = new ResultBuilder<TestEntity, TestErrorType>(entity);
        var newEntity = new TestEntity { Id = 99 };

        var result = builder.WithSuccess(newEntity).Build();

        result.Successful.Should().BeTrue();
        result.Entity.Should().Be(newEntity);
    }

    [Fact]
    public void ResultBuilderOfTEntityTErrorType_WithError_ShouldBuildFailedResultWithEntityAndErrorType()
    {
        var entity = new TestEntity { Id = 42 };
        var builder = new ResultBuilder<TestEntity, TestErrorType>(entity);

        var result = builder.WithError(TestErrorType.ValidationFailed).Build();

        result.Successful.Should().BeFalse();
        result.Entity.Should().Be(entity);
        result.ErrorType.Should().Be(TestErrorType.ValidationFailed);
    }

    [Fact]
    public void ResultBuilderOfTEntityTErrorType_WithErrorEntity_ShouldUpdateEntityAndBuildFail()
    {
        var entity = new TestEntity { Id = 42 };
        var builder = new ResultBuilder<TestEntity, TestErrorType>(entity);
        var errorEntity = new TestEntity { Id = 99 };

        var result = builder.WithError(errorEntity, TestErrorType.NotFound).Build();

        result.Successful.Should().BeFalse();
        result.Entity.Should().Be(errorEntity);
        result.ErrorType.Should().Be(TestErrorType.NotFound);
    }

    [Fact]
    public void ResultBuilderOfTEntityTErrorType_WithEntity_ShouldUpdateEntity()
    {
        var entity = new TestEntity { Id = 42 };
        var builder = new ResultBuilder<TestEntity, TestErrorType>(entity);
        var newEntity = new TestEntity { Id = 99 };

        var result = builder.WithEntity(newEntity).WithSuccess().Build();

        result.Entity.Should().Be(newEntity);
    }

    [Fact]
    public void ResultBuilderOfTEntityTErrorType_ImplicitConversion_ShouldWork()
    {
        var entity = new TestEntity { Id = 1 };
        Result<TestEntity, TestErrorType> result = new ResultBuilder<TestEntity, TestErrorType>(entity).WithSuccess();

        result.Successful.Should().BeTrue();
        result.Entity.Should().Be(entity);
    }
}
