using ReportService.Core;
using ReportService.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using serwersms;
using Cipher;

namespace SmsReportService
{
    public partial class SmsReportService : ServiceBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private Timer _timer = new Timer(Convert.ToInt32(ConfigurationManager.AppSettings["IntervalInMinutes"]) * 60000);
        private int _intervalInMinutes;
        private StringCipher _stringCipher = new StringCipher("5F73524F-C324-453D-9FDB-CD52EC448F35");
        private ErrorRepository _errorRepository = new ErrorRepository();
        private GenerateSmsMessage _smsMessage = new GenerateSmsMessage();
        private const string _notEncryptedPasswordSettings = "encrypt:";
        private string _phone;
        private string _username;
        private string _password;
        public SmsReportService()
        {
            InitializeComponent();

            try
            {
                _intervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalInMinutes"]);
                _phone = DecryptPhoneNumber();
                _username = ConfigurationManager.AppSettings["Username"];
                _password = DecryptPassword();
            }
            catch (Exception ex)
            {

                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }


        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += DoWork;
            _timer.Start();
            Logger.Info("Service started...");
        }

        private void DoWork(object sender, ElapsedEventArgs e)
        {
            try
            {
                SendError();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private void SendError()
        {
            var errors = _errorRepository.GetLastErrors(_intervalInMinutes);

            if (errors == null || !errors.Any())
                return;

            var serwerssms = new SerwerSMS(_username, _password);
            var data = new Dictionary<string, string>();
            String text = _smsMessage.GenerateErrors(errors,_intervalInMinutes);
            String sender = "INFORMACJA";
            data.Add("details", "1");
            serwerssms.messages.sendSms(_phone, text, sender, data);

            Logger.Info("Error sent.");
        }
        private string DecryptPassword()
        {
            var encryptedPassword = ConfigurationManager.AppSettings["Password"];

            if (encryptedPassword.StartsWith(_notEncryptedPasswordSettings))
            {
                encryptedPassword = _stringCipher.Encrypt(encryptedPassword.Replace(_notEncryptedPasswordSettings, string.Empty));

                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configFile.AppSettings.Settings["Password"].Value = encryptedPassword;

                configFile.Save();
            }
            return _stringCipher.Decrypt(encryptedPassword);
        }
        private string DecryptPhoneNumber()
        {
            var encryptedPhoneNumber = ConfigurationManager.AppSettings["PhoneNumber"];

            if (encryptedPhoneNumber.StartsWith(_notEncryptedPasswordSettings))
            {
                encryptedPhoneNumber = _stringCipher.Encrypt(encryptedPhoneNumber.Replace(_notEncryptedPasswordSettings, string.Empty));

                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configFile.AppSettings.Settings["PhoneNumber"].Value = encryptedPhoneNumber;

                configFile.Save();
            }
            return _stringCipher.Decrypt(encryptedPhoneNumber);
        }
        protected override void OnStop()
        {
            Logger.Info("Service stopped...");
        }
    }
}
