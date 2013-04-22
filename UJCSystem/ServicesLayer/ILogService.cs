using System.ServiceModel;

namespace ServicesLayer
{
    [ServiceContract]
    public interface ILogService
    {
         /// <summary>
        /// Debug log method
        /// </summary>
        /// <param name="msg">Message to log</param>
        [OperationContract]
        void LogDebug(string msg);

        /// <summary>
        /// Error log method
        /// </summary>
        /// <param name="msg">Message to log</param>
        [OperationContract]
        void LogError(string msg);

        /// <summary>
        /// Info log method
        /// </summary>
        /// <param name="msg">Message to log</param>
        [OperationContract]
        void LogInfo(string msg);
        
     /// <summary>
        /// Warn log method
        /// </summary>
        /// <param name="msg">Message to log</param>
        [OperationContract]
        void LogWarn(string msg);
    }
}
