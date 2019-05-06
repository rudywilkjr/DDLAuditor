using System;

namespace Logging
{
    public class Logger
    {
        private log4net.ILog _log;

        public Logger()
        {
            log4net.Config.XmlConfigurator.Configure();
            _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void Info(string str)
        {
            _log.Info(str);
        }

        public void Debug(string str)
        {
            _log.Debug(str);
        }

        public void Error(string str)
        {
            _log.Error(str);
        }

        public void Exception(Exception ex)
        {
            _log.ErrorFormat("Exception thrown: message {0}\nStack Trace:{1}", ex.Message, ex.StackTrace);
            if(ex.InnerException != null)
            {
                _log.Error("Inner Exception found too...");
                Exception(ex.InnerException);
            }
        }
    }
}
