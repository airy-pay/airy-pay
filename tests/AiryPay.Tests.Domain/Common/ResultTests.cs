using AiryPay.Domain.Common.Result;

namespace AiryPay.Tests.Domain.Common;

public enum TestErrorType
{
    Unknown,
    NotFound,
    ValidationFailed
}

public class ResultTests
{
    [Fact]
    public void Result_Success_ShouldBeSuccessful()
    {
        var result = Result.Success();

        result.Successful.Should().BeTrue();
        result.Failed.Should().BeFalse();
    }

    [Fact]
    public void Result_Fail_ShouldBeFailed()
    {
        var result = Result.Fail();

        result.Successful.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Fact]
    public void ResultOfTErrorType_Success_ShouldBeSuccessful()
    {
        var result = Result<TestErrorType>.Success();

        result.Successful.Should().BeTrue();
        result.Failed.Should().BeFalse();
    }

    [Fact]
    public void ResultOfTErrorType_Fail_ShouldContainErrorType()
    {
        var result = Result<TestErrorType>.Fail(TestErrorType.NotFound);

        result.Successful.Should().BeFalse();
        result.Failed.Should().BeTrue();
        result.ErrorType.Should().Be(TestErrorType.NotFound);
    }

    [Fact]
    public void ResultOfTEntityTErrorType_Success_ShouldContainEntity()
    {
        var entity = new TestEntity { Id = 1 };
        var result = Result<TestEntity, TestErrorType>.Success(entity);

        result.Successful.Should().BeTrue();
        result.Failed.Should().BeFalse();
        result.Entity.Should().Be(entity);
    }

    [Fact]
    public void ResultOfTEntityTErrorType_Fail_ShouldContainEntityAndErrorType()
    {
        var entity = new TestEntity { Id = 1 };
        var result = Result<TestEntity, TestErrorType>.Fail(entity, TestErrorType.ValidationFailed);

        result.Successful.Should().BeFalse();
        result.Failed.Should().BeTrue();
        result.Entity.Should().Be(entity);
        result.ErrorType.Should().Be(TestErrorType.ValidationFailed);
    }
}

public class TestEntity
{
    public int Id { get; set; }
}
