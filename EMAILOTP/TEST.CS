using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Email_OTP_Module;

[TestClass]
public class EmailOTPModuleTests
{
    [TestMethod]
    public void TestGenerateOTPSuccess()
    {
        Email_OTP_Module emailOtpModule = new Email_OTP_Module(); 
        var status = emailOtpModule.generate_OTP_email("test@dso.org.sg");
        Assert.AreEqual(Email_OTP_Module.Status.STATUS_EMAIL_OK, status);
    }

    [TestMethod]
    public void TestGenerateOTPInvalidEmailFormat()
    {
        Email_OTP_Module emailOtpModule = new Email_OTP_Module(); 
        var status = emailOtpModule.generate_OTP_email("invalid-email");
        Assert.AreEqual(Email_OTP_Module.Status.STATUS_EMAIL_INVALID, status);
    }

    [TestMethod]
    public void TestGenerateOTPInvalidEmailDomain()
    {
        Email_OTP_Module emailOtpModule = new Email_OTP_Module(); 
        var status = emailOtpModule.generate_OTP_email("test@dso.org.sg.com");
        Assert.AreEqual(Email_OTP_Module.Status.STATUS_EMAIL_INVALID, status);
    }

    [TestMethod]
    public void TestCheckOTPCorrect()
    {
        Email_OTP_Module emailOtpModule = new Email_OTP_Module(); 
        emailOtpModule.generate_OTP_email("test@dso.org.sg");
        var input = new MockedIOStream(emailOtpModule.GetCurrentOTP());
        var status = emailOtpModule.check_OTP(input);
        Assert.AreEqual(Email_OTP_Module.Status.STATUS_OTP_OK, status);
    }

    [TestMethod]
    public void TestCheckOTPMaxAttempts()
    {
        Email_OTP_Module emailOtpModule = new Email_OTP_Module(); 
        emailOtpModule.generate_OTP_email("test@dso.org.sg");
        var input = new MockedIOStream("000000"); // Incorrect OTP
        var status = emailOtpModule.check_OTP(input);
        Assert.AreEqual(Email_OTP_Module.Status.STATUS_OTP_FAIL, status);
    }

    [TestMethod]
    public void TestCheckOTPTimeout()
    {
        Email_OTP_Module emailOtpModule = new Email_OTP_Module(); 
        emailOtpModule.generate_OTP_email("test@dso.org.sg");
        var input = new MockedIOStream(null); 
        Task.Delay(TimeSpan.FromMinutes(1)).Wait(); // Wait for timeout
        var status = emailOtpModule.check_OTP(input);
        Assert.AreEqual(Email_OTP_Module.Status.STATUS_OTP_TIMEOUT, status);
    }
}


// Mocked iostream class for testing
public class MockedIOStream : iostream
{
    private string otp;

    public MockedIOStream(string otp)
    {
        this.otp = otp;
    }

    public override string readOTP()
    {
        return otp;
    }
}
