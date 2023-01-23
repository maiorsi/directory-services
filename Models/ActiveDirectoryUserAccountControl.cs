// <copyright file="ActiveDirectoryUserAccountControl.cs" owner="maiorsi">
// Licenced under the MIT Licence.
// </copyright>

using System;

namespace DirectoryServices.Models
{
    /// <summary>
    /// Flags that control the behaviour of the user account.
    /// </summary>
    [Flags()]
    public enum ActiveDirectoryUserAccountControl : int
    {
        SCRIPT = 0x00000001,
        ACCOUNT_DISABLED = 0x00000002,
        HOMEDIR_REQUIRED = 0x00000008,
        LOCKOUT = 0x00000010,
        PASSWD_NOTREQD = 0x00000020,
        PASSWD_CANT_CHANGE = 0x00000040,
        ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x00000080,
        TEMP_DUPLICATE_ACCOUNT = 0x00000100,
        NORMAL_ACCOUNT = 0x00000200,
        INTERDOMAIN_TRUST_ACCOUNT = 0x00000800,
        WORKSTATION_TRUST_ACCOUNT = 0x00001000,
        SERVER_TRUST_ACCOUNT = 0x00002000,
        UNUSED_1 = 0x00004000,
        UNUSED_2 = 0x00008000,
        DONT_EXPIRE_PASSWORD = 0x00010000,
        MNS_LOGON_ACCOUNT = 0x00020000,
        SMARTCARD_REQUIRED = 0x00040000,
        TRUSTED_FOR_DELEGATION = 0x00080000,
        NOT_DELEGATED = 0x00100000,
        USE_DES_KEY_ONLY = 0x00200000,
        DONT_REQUIRE_PREAUTH = 0x00400000,
        PASSWORD_EXPIRED = 0x00800000,
        TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x01000000,
        PARTIAL_SECRETS_ACCOUNT = 0x04000000,
        USE_AES_KEYS = 0x08000000
    }
}