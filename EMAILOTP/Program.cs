using System;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Running unit tests...");

        var testClass = new EmailOTPModuleTests();

        RunTest(testClass.TestGenerateOTPSuccess, "TestGenerateOTPSuccess");
        RunTest(testClass.TestGenerateOTPInvalidEmailFormat, "TestGenerateOTPInvalidEmailFormat");
        RunTest(testClass.TestGenerateOTPInvalidEmailDomain, "TestGenerateOTPInvalidEmailDomain");
        RunTest(testClass.TestCheckOTPCorrect, "TestCheckOTPCorrect");
        RunTest(testClass.TestCheckOTPMaxAttempts, "TestCheckOTPMaxAttempts");
        RunTest(testClass.TestCheckOTPTimeout, "TestCheckOTPTimeout");

        Console.WriteLine("Unit tests execution completed.");
    }

    private static void RunTest(Action testMethod, string testName)
    {
        try
        {
            testMethod.Invoke(); 
            Console.WriteLine($"{testName}: Passed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{testName}: Failed - {ex.Message}");
        }
    }
}
