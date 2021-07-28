using ReportService.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Core
{
    public class GenerateSmsMessage
    {
        public string GenerateErrors(List<Error> errors, int interval)
        {
            if (errors == null)
                throw new ArgumentNullException(nameof(errors));

            if (!errors.Any())
                return string.Empty;

            var msg = $"Błędy z ostatnich {interval} minut.\n";

            foreach (var error in errors)
            {
                msg +=
                    $@"Wiadomość błędu : {error.Message} Data wystąpienia: {error.Date.ToString("dd-MM-yyyy HH:mm")}" +"\n";
            }

            msg += @"Automatyczna wiadomość wysłana z aplikacji SmsReportService.";

            return msg;
        }
        
    }
}
