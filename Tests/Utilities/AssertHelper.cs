using System.Linq;
using Xunit;
using FluentValidation.Results;

public class AssertHelper
{
    public static void FailsWithMessage(ValidationResult result, string errorMessage)
    {
        Assert.False(result.IsValid);
        Assert.Equal(1, result.Errors.Count);
        Assert.Equal(errorMessage, result.Errors.First().ErrorMessage);
    }
}