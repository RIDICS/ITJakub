using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using System.Text;

namespace Ujc.Naki.RestLayer
{
    /// <summary>
    ///     Utilities used by REST layer.
    /// </summary>
    public static class RestUtils
    {
        /// <summary>
        ///     Converts specified string to the memory stream.
        /// </summary>
        /// <param name="str">string to be converted</param>
        /// <returns>string as memory stream</returns>
        public static Stream StringToStream(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return new MemoryStream(bytes);
        }

        /// <summary>
        ///     Gets optional integer query parameter.
        /// </summary>
        /// <param name="parmName">name of the parameter</param>
        /// <returns></returns>
        public static int GetIntQp(string parmName)
        {
            return GetIntQp(parmName, -1, false);
        }

        /// <summary>
        ///     Gets optional integer query paramter.
        /// </summary>
        /// <param name="parmName">name of the parameter</param>
        /// <param name="defaultValue">default value if the parameter is not set</param>
        /// <returns>value of the parameter</returns>
        public static int GetIntQp(string parmName, int defaultValue)
        {
            return GetIntQp(parmName, defaultValue, false);
        }

        /// <summary>
        ///     Gets required integer query parameter.
        /// </summary>
        /// <param name="parmName">name of the parameter</param>
        /// <returns>value of the parameter</returns>
        public static int GetRequiredIntQp(string parmName)
        {
            return GetIntQp(parmName, -1, true);
        }

        /// <summary>
        ///     Gets integer query parameter.
        /// </summary>
        /// <param name="parmName">name of the parameter</param>
        /// <param name="defaultValue">default value if the parameter is not set</param>
        /// <param name="isRequired">flag if parameter is required</param>
        /// <returns>value of the parameter</returns>
        private static int GetIntQp(string parmName, int defaultValue, bool isRequired)
        {
            string value = GetStringQp(parmName, defaultValue.ToString(CultureInfo.InvariantCulture), isRequired);
            int returnValue = defaultValue;

            if (!string.IsNullOrEmpty(value))
            {
                if (!Int32.TryParse(value, out returnValue))
                    SetBadRequest(string.Format("Invalid query parameter \"{0}\", value \"{1}\"", parmName, value));
            }

            return returnValue;
        }


        /// <summary>
        ///     Gets optional string query parameter.
        /// </summary>
        /// <param name="parmName">name of the parameter</param>
        /// <returns>value of the parameter</returns>
        public static string GetStringQp(string parmName)
        {
            return GetStringQp(parmName, string.Empty, false);
        }

        /// <summary>
        ///     Gets optional string query parameter.
        /// </summary>
        /// <param name="parmName">name of the parameter</param>
        /// <param name="defaultValue">default value if parameter is not set</param>
        /// <returns>value of the parameter</returns>
        public static string GetStringQp(string parmName, string defaultValue)
        {
            return GetStringQp(parmName, defaultValue, false);
        }

        /// <summary>
        ///     Gets required string query parameter.
        /// </summary>
        /// <param name="parmName">name of the parameter</param>
        /// <returns>value of the parameter</returns>
        public static string GetRequiredStringQp(string parmName)
        {
            return GetStringQp(parmName, string.Empty, true);
        }

        /// <summary>
        ///     Gets string query parameter.
        /// </summary>
        /// <param name="parmName">name of the parameter</param>
        /// <param name="defaultValue">default value if parameter is not set</param>
        /// <param name="isRequired">flag if parameter is required</param>
        /// <returns>value of the parameter</returns>
        private static string GetStringQp(string parmName, string defaultValue, bool isRequired)
        {
            if (WebOperationContext.Current == null)
                throw new InvalidOperationException("WebOperationContext is null");

            string returnValue = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters[parmName];

            // if the parameter is required then this is a bad request - this will throw ArgumentException
            if (isRequired && string.IsNullOrEmpty(returnValue))
            {
                SetBadRequest(string.Format("Missing required query parameter \"{0}\"", parmName));
            }
            else if (returnValue == null)
            {
                // If null (not found) use default value
                returnValue = defaultValue;
            }

            return returnValue;
        }

        /// <summary>
        ///     Sets request as bad and returns appropriate response by throwing argument exception.
        /// </summary>
        /// <param name="description">description of error</param>
        public static void SetBadRequest(string description)
        {
            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.StatusDescription = description;
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                WebOperationContext.Current.OutgoingResponse.SuppressEntityBody = true;
            }
            throw new ArgumentException(description);
        }
    }
}