using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace ITJakub.MobileApps.Client.Core.Manager.Communication
{
    /// <summary>
    /// Represents a run-time behavior extension for a client endpoint.
    /// </summary>
    public class CustomEndpointBehavior : IEndpointBehavior
    {
        private readonly ClientMessageInspector m_clientMessageInspector;

        public CustomEndpointBehavior(ClientMessageInspector clientMessageInspector)
        {
            m_clientMessageInspector = clientMessageInspector;
        }

        /// <summary>
        /// Implements a modification or extension of the client across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that is to be customized.</param>
        /// <param name="clientRuntime">The client runtime to be customized.</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(m_clientMessageInspector);
        }

        /// <summary>
        /// Implement to pass data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <param name="endpoint">The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // Nothing special here
        }

        /// <summary>
        /// Implements a modification or extension of the service across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            // Nothing special here
        }

        /// <summary>
        /// Implement to confirm that the endpoint meets some intended criteria.
        /// </summary>
        /// <param name="endpoint">The endpoint to validate.</param>
        public void Validate(ServiceEndpoint endpoint)
        {
            // Nothing special here
        }
    }
}