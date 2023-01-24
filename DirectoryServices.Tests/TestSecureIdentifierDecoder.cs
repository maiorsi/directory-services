using DirectoryServices.Helpers;

using System;

namespace DirectoryServices.Tests;

public class TestSecureIdentifierDecoder
{
    private const string BinarySidString = "AQUAAAAAAAUVAAAAzFGCAfoSAjc3M1xJGzAAAA==";
    private const string SidString = "S-1-5-21-25317836-922882810-1230779191-12315";

    [Fact]
    public void TestDecodeBinarySid()
    {
        byte[] bytes = Convert.FromBase64String(BinarySidString);

        var decoded = SecureIdentifierDecoder.Decode(bytes);

        var success = decoded == SidString;

        Assert.True(success, "Should be true!");
    }
}