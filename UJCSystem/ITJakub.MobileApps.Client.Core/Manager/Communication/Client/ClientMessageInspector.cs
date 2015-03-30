using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace ITJakub.MobileApps.Client.Core.Manager.Communication.Client
{
    /// <summary>
    /// Represents a message inspector object that can be added to the <c>MessageInspectors</c> collection to view or modify messages.
    /// </summary>
    public class ClientMessageInspector : IClientMessageInspector
    {
        public string CommunicationToken { get; set; }

        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The WCF client object channel.</param>
        /// <returns>
        /// The object that is returned as the <paramref name="correlationState " /> argument of
        /// the <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)" /> method.
        /// This is null if no correlation state is used.The best practice is to make this a <see cref="T:System.Guid" /> to ensure that no two
        /// <paramref name="correlationState" /> objects are the same.
        /// </returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty property = new HttpRequestMessageProperty();

            property.Headers["Communication_Token"] = CommunicationToken;
            request.Properties.Add(HttpRequestMessageProperty.Name, property);

            return null;
        }

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            // Nothing special here
        }
    }
}