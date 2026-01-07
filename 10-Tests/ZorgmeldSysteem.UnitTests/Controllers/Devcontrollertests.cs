using Microsoft.AspNetCore.Mvc;
using Xunit;
using ZorgmeldSysteem.WebApi.Controllers;

namespace ZorgmeldSysteem.UnitTests.Controllers;

/// <summary>
/// Unit Tests voor DevController
/// </summary>
public class DevControllerTests
{
    #region GenerateHash Tests

    [Fact]
    public void GenerateHash_ReturnsOkResult_WithHashedPassword()
    {
        // Arrange
        var controller = new DevController();
        string testPassword = "TestPassword123";

        // Act
        var result = controller.GenerateHash(testPassword);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);

        // Check of het resultaat de juiste properties heeft
        var resultValue = okResult.Value;
        var passwordProperty = resultValue?.GetType().GetProperty("password");
        var hashProperty = resultValue?.GetType().GetProperty("hash");
        var verifiedProperty = resultValue?.GetType().GetProperty("verified");

        Assert.NotNull(passwordProperty);
        Assert.NotNull(hashProperty);
        Assert.NotNull(verifiedProperty);
    }

    [Fact]
    public void GenerateHash_ReturnsVerifiedTrue_ForGeneratedHash()
    {
        // Arrange
        var controller = new DevController();
        string testPassword = "MySecurePassword";

        // Act
        var result = controller.GenerateHash(testPassword);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultValue = okResult.Value!;
        var verifiedProperty = resultValue.GetType().GetProperty("verified");
        var verified = (bool)verifiedProperty!.GetValue(resultValue)!;

        // De hash moet verified zijn (BCrypt.Verify moet true returnen)
        Assert.True(verified);
    }

    [Fact]
    public void GenerateHash_GeneratesDifferentHashes_ForSamePassword()
    {
        // Arrange
        var controller = new DevController();
        string testPassword = "SamePassword";

        // Act
        var result1 = controller.GenerateHash(testPassword);
        var result2 = controller.GenerateHash(testPassword);

        // Assert
        var okResult1 = Assert.IsType<OkObjectResult>(result1);
        var okResult2 = Assert.IsType<OkObjectResult>(result2);

        var value1 = okResult1.Value!;
        var value2 = okResult2.Value!;

        var hash1 = value1.GetType().GetProperty("hash")!.GetValue(value1)!.ToString();
        var hash2 = value2.GetType().GetProperty("hash")!.GetValue(value2)!.ToString();

        // BCrypt genereert elke keer een andere hash (door de salt)
        Assert.NotEqual(hash1, hash2);
    }

    [Theory]
    [InlineData("Password1")]
    [InlineData("AnotherPass123")]
    [InlineData("SuperSecure!@#")]
    public void GenerateHash_WorksForDifferentPasswords(string password)
    {
        // Arrange
        var controller = new DevController();

        // Act
        var result = controller.GenerateHash(password);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultValue = okResult.Value!;
        var passwordValue = resultValue.GetType().GetProperty("password")!.GetValue(resultValue)!.ToString();
        var hashValue = resultValue.GetType().GetProperty("hash")!.GetValue(resultValue)!.ToString();
        var verifiedValue = (bool)resultValue.GetType().GetProperty("verified")!.GetValue(resultValue)!;

        Assert.Equal(password, passwordValue);
        Assert.NotNull(hashValue);
        Assert.True(verifiedValue);
    }

    [Fact]
    public void GenerateHash_ReturnsCorrectMessage()
    {
        // Arrange
        var controller = new DevController();

        // Act
        var result = controller.GenerateHash("test");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultValue = okResult.Value!;
        var messageValue = resultValue.GetType().GetProperty("message")!.GetValue(resultValue)!.ToString();

        Assert.Equal("Kopieer de hash en gebruik in SQL UPDATE", messageValue);
    }

    #endregion

    #region TestHash Tests

    [Fact]
    public void TestHash_ReturnsOkResult_WithValidationResult()
    {
        // Arrange
        var controller = new DevController();

        // Genereer eerst een hash
        var generateResult = controller.GenerateHash("TestPass123");
        var generateOkResult = Assert.IsType<OkObjectResult>(generateResult);

        var generateValue = generateOkResult.Value!;
        string hash = generateValue.GetType().GetProperty("hash")!.GetValue(generateValue)!.ToString()!;

        var request = new HashTestRequest
        {
            Password = "TestPass123",
            Hash = hash
        };

        // Act
        var result = controller.TestHash(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void TestHash_ReturnsTrue_ForMatchingPasswordAndHash()
    {
        // Arrange
        var controller = new DevController();
        string password = "CorrectPassword";

        // Genereer een hash voor het wachtwoord
        var hashResult = controller.GenerateHash(password);
        var hashOkResult = Assert.IsType<OkObjectResult>(hashResult);

        var hashValue = hashOkResult.Value!;
        string hash = hashValue.GetType().GetProperty("hash")!.GetValue(hashValue)!.ToString()!;

        var request = new HashTestRequest
        {
            Password = password,
            Hash = hash
        };

        // Act
        var result = controller.TestHash(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultValue = okResult.Value!;
        var isValid = (bool)resultValue.GetType().GetProperty("isValid")!.GetValue(resultValue)!;

        Assert.True(isValid);
    }

    [Fact]
    public void TestHash_ReturnsFalse_ForNonMatchingPasswordAndHash()
    {
        // Arrange
        var controller = new DevController();

        // Genereer een hash voor een ander wachtwoord
        var hashResult = controller.GenerateHash("CorrectPassword");
        var hashOkResult = Assert.IsType<OkObjectResult>(hashResult);

        var hashValue = hashOkResult.Value!;
        string hash = hashValue.GetType().GetProperty("hash")!.GetValue(hashValue)!.ToString()!;

        var request = new HashTestRequest
        {
            Password = "WrongPassword", // Ander wachtwoord!
            Hash = hash
        };

        // Act
        var result = controller.TestHash(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);


        var resultValue = okResult.Value!;
        var isValid = (bool)resultValue.GetType().GetProperty("isValid")!.GetValue(resultValue)!;

        Assert.False(isValid);
    }

    [Fact]
    public void TestHash_ReturnsCorrectPropertiesInResponse()
    {
        // Arrange
        var controller = new DevController();
        var request = new HashTestRequest
        {
            Password = "TestPassword",
            Hash = "$2a$11$someValidBCryptHashHere..."
        };

        // Act
        var result = controller.TestHash(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultValue = okResult.Value!;
        var passwordValue = resultValue.GetType().GetProperty("password")!.GetValue(resultValue)!.ToString();
        var hashValue = resultValue.GetType().GetProperty("hash")!.GetValue(resultValue)!.ToString();
        var isValidProperty = resultValue.GetType().GetProperty("isValid");

        Assert.Equal("TestPassword", passwordValue);
        Assert.Equal("$2a$11$someValidBCryptHashHere...", hashValue);
        Assert.NotNull(isValidProperty);
    }

    [Theory]
    [InlineData("Pass1", "Pass1", true)]
    [InlineData("Pass2", "DifferentPass", false)]
    public void TestHash_ValidatesCorrectly(string hashPassword, string testPassword, bool expectedValid)
    {
        // Arrange
        var controller = new DevController();

        // Genereer hash voor hashPassword
        var hashResult = controller.GenerateHash(hashPassword);
        var hashOkResult = Assert.IsType<OkObjectResult>(hashResult);

        var hashValue = hashOkResult.Value!;
        string hash = hashValue.GetType().GetProperty("hash")!.GetValue(hashValue)!.ToString()!;

        var request = new HashTestRequest
        {
            Password = testPassword,
            Hash = hash
        };

        // Act
        var result = controller.TestHash(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultValue = okResult.Value!;
        var isValid = (bool)resultValue.GetType().GetProperty("isValid")!.GetValue(resultValue)!;

        Assert.Equal(expectedValid, isValid);
    }

    #endregion

    #region Integration Test - GenerateHash + TestHash

    [Fact]
    public void GenerateHashAndTestHash_WorkTogether()
    {
        // Arrange
        var controller = new DevController();
        string originalPassword = "IntegrationTest123";

        // Act - Stap 1: Genereer hash
        var generateResult = controller.GenerateHash(originalPassword);
        var generateOkResult = Assert.IsType<OkObjectResult>(generateResult);

        var generateValue = generateOkResult.Value!;
        string generatedHash = generateValue.GetType().GetProperty("hash")!.GetValue(generateValue)!.ToString()!;

        // Act - Stap 2: Test de hash
        var testRequest = new HashTestRequest
        {
            Password = originalPassword,
            Hash = generatedHash
        };
        var testResult = controller.TestHash(testRequest);

        // Assert
        var testOkResult = Assert.IsType<OkObjectResult>(testResult);

        var testValue = testOkResult.Value!;
        var isValid = (bool)testValue.GetType().GetProperty("isValid")!.GetValue(testValue)!;

        Assert.True(isValid);
    }

    #endregion
}