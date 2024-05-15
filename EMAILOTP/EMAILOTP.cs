using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

public class Email_OTP_Module
{
    private string currentOTP;
    private DateTime otpExpiryTime;
    private const string EmailDomain = "@dso.org.sg";
    private const int OTPValidityDuration = 1; // in minutes
    private const int MaxOTPTries = 10;
    private const string OTPFormat = "Your OTP Code is {0}. The code is valid for 1 minute";

    public enum Status
    {
        STATUS_EMAIL_OK,
        STATUS_EMAIL_FAIL,
        STATUS_EMAIL_INVALID,
        STATUS_OTP_OK,
        STATUS_OTP_FAIL,
        STATUS_OTP_TIMEOUT
    }

    public Status generate_OTP_email(string user_email)
    {
        if (!IsValidEmail(user_email) || !user_email.EndsWith(EmailDomain))
        {
            return Status.STATUS_EMAIL_INVALID;
        }

        currentOTP = GenerateOTP();
        otpExpiryTime = DateTime.Now.AddMinutes(OTPValidityDuration);

        string emailBody = string.Format(OTPFormat, currentOTP);

        try
        {
            send_email(user_email, emailBody);
            return Status.STATUS_EMAIL_OK;
        }
        catch (Exception)
        {
            return Status.STATUS_EMAIL_FAIL;
        }
    }
    public Status check_OTP(iostream input)
    {
        int attempts = 0;
        CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromMinutes(OTPValidityDuration));

        while (attempts < MaxOTPTries && DateTime.Now < otpExpiryTime)
        {
            Console.WriteLine($"Attempt {attempts + 1}: Checking OTP. Current Time: {DateTime.Now}, Expiry Time: {otpExpiryTime}");
            Task<string> task = Task.Run(() => input.readOTP(), cts.Token);

            try
            {
                string userOTP = task.Result;
                Console.WriteLine($"Received OTP: {userOTP}, Current OTP: {currentOTP}");

                if (userOTP == currentOTP)
                {
                    Console.WriteLine("OTP is correct.");
                    return Status.STATUS_OTP_OK;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General exception during OTP check: {ex.Message}");
                return Status.STATUS_OTP_FAIL;
            }
            finally
            {
                attempts++;
            }
        }

        Console.WriteLine("Failed after all attempts or expired.");
        return DateTime.Now >= otpExpiryTime ? Status.STATUS_OTP_TIMEOUT : Status.STATUS_OTP_FAIL;
    }




    private bool IsValidEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private string GenerateOTP()
    {
        Random random = new Random();
        string otp = random.Next(100000, 999999).ToString();
        Console.WriteLine($"Generated OTP: {otp}"); // Print the generated OTP
        return otp;
    }

    private void send_email(string email_address, string email_body)
    {
        // some calls to the SMTP module to send out emails
    }

    public string GetCurrentOTP()
    {
        return currentOTP;
    }
}

// Example implementation of iostream class
public class iostream
{
    public virtual string readOTP()
    {
        Console.WriteLine("Please enter your 6-digit OTP:");
        return Console.ReadLine();
    }
}
