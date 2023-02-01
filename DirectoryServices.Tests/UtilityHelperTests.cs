// <copyright file="UtilityHelperTests.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using DirectoryServices.Helpers;

namespace DirectoryServices.Tests;

public class UtilityHelperTests
{
    private const string BinaryGuidString = "9eLQGLjeDE6kgxMT1ghj+g==";
    private const string GuidString = "18d0e2f5-deb8-4e0c-a483-1313d60863fa";
    private const string BinarySidString = "AQQAAAAAAAUVAAAAwdFKz9WmazDFb3Y8";
    private const string SidString = "S-1-5-21-3477787073-812361429-1014394821";

    [Fact]
    public void TestEncodeBinaryGuid()
    {
        var encoded = GuidHelper.Encode(GuidString);

        var encodedBase64 = Convert.ToBase64String(encoded);

        var success = encodedBase64 == BinaryGuidString;

        Assert.True(success, $"Expected '{BinaryGuidString}'; got '{encodedBase64}'!");
    }

    [Fact]
    public void TestDecodeBinaryGuid()
    {
        byte[] bytes = Convert.FromBase64String(BinaryGuidString);

        var decoded = GuidHelper.Decode(bytes);

        var success = decoded == GuidString;

        Assert.True(success, $"Expected '{GuidString}'; got '{decoded}'!");
    }

    [Fact]
    public void TestEncodeSidString()
    {
        var binary = SecureIdentifierHelper.Encode(SidString);

        var encoded = Convert.ToBase64String(binary);

        var success = encoded == BinarySidString;

        Assert.True(success, $"Expected '{BinarySidString}'; got '{encoded}'!");
    }

    [Fact]
    public void TestDecodeBinarySid()
    {
        byte[] bytes = Convert.FromBase64String(BinarySidString);

        var decoded = SecureIdentifierHelper.Decode(bytes);

        var success = decoded == SidString;

        Assert.True(success, $"Expected '{SidString}'; got '{decoded}'!");
    }
}