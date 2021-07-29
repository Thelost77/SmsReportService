using ReportService.Core;
using ReportService.Core.Repositories;
using serwersms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        private static int _intervalInMinutes = 1;
        static void Main(string[] args)
        {
            try
            {
                // This logic lets you try how your sms will look.

                ErrorRepository _errorRepository = new ErrorRepository();
                GenerateSmsMessage _smsMessage = new GenerateSmsMessage();

                var errors = _errorRepository.GetLastErrors(_intervalInMinutes);
                var serwerssms = new SerwerSMS("enter_username", "enter_password");
                var data = new Dictionary<string, string>();
                serwerssms.format = "json";


                String phone = "+48_enterPhoneNumber";
                String text = _smsMessage.GenerateErrors(errors, _intervalInMinutes);
                String sender = "INFORMACJA";
                data.Add("details", "1");
                var result = serwerssms.messages.SendSms(phone, text, sender, data).ToString();
                Console.WriteLine(result);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        
    }
}
