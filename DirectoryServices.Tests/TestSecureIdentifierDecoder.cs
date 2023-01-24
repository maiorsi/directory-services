using DirectoryServices.Helpers;

using System;

namespace DirectoryServices.Tests;

public class TestSecureIdentifierDecoder
{
    private const string BinarySidString = "AQQAAAAAAAUVAAAAwdFKz9WmazDFb3Y8";
    private const string SidString = "S-1-5-21-3477787073-812361429-1014394821";

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