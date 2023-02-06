using DirectoryServices.Helpers;

internal class Program
{
    private static void Main(string[] args)
    {
        static void OutputOptions()
        {
            Console.WriteLine("1. Encode Sid String");
            Console.WriteLine("2. Encode Guid String");
            Console.WriteLine("3. Decode Sid String");
            Console.WriteLine("4. Decode Guid String");
        }

        static void EncodeSidString()
        {
            Console.WriteLine("Enter a SId string...");

            var sid = Console.ReadLine();
            var encoded = SecureIdentifierHelper.Encode(sid);
            var base64 = Convert.ToBase64String(encoded);

            Console.WriteLine(base64);
        }

        static void EncodeGuidString()
        {
            Console.WriteLine("Enter a Guid string...");

            var guid = Console.ReadLine();
            var encoded = GuidHelper.Encode(guid);
            var base64 = Convert.ToBase64String(encoded);

            Console.WriteLine(base64);
        }

        static void DecodeBinarySid()
        {
            Console.WriteLine("Enter a binary (base64) encoded SId string...");

            var base64 = Console.ReadLine();
            var bytes = Convert.FromBase64String(base64);
            var decoded = SecureIdentifierHelper.Decode(bytes);

            Console.WriteLine(decoded);
        }

        static void DecodeBinaryGuid()
        {
            Console.WriteLine("Enter a binary (base64) encoded Guid string...");

            var base64 = Console.ReadLine();
            var bytes = Convert.FromBase64String(base64);
            var decoded = GuidHelper.Decode(bytes);

            Console.WriteLine(decoded);
        }

        Console.WriteLine("Directory Services Helper Prompt");

        OutputOptions();

        Console.WriteLine("Enter your selection...");

        var input = Console.ReadLine();

        switch (input)
        {
            case "1":
                EncodeSidString();
                break;
            case "2":
                EncodeGuidString();
                break;
            case "3":
                DecodeBinarySid();
                break;
            case "4":
                DecodeBinaryGuid();
                break;
            default:
                Console.WriteLine("Invalid selection!");
                OutputOptions();
                break;
        }
    }
}