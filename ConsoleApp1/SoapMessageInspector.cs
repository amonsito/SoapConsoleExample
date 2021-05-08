namespace ConsoleApp1
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    /// <summary>
    /// Soap Service Messages
    /// </summary>
    internal struct SoapServiceMessages
    {
        /// <summary>
        /// The request message
        /// </summary>
        public string Request;

        /// <summary>
        /// The response message
        /// </summary>
        public string Response;
    }

    /// <summary>
    /// Soap Message Inspector
    /// </summary>
    /// <seealso cref="System.ServiceModel.Dispatcher.IClientMessageInspector" />
    public class SoapMessageInspector : IClientMessageInspector
    {
        /// <summary>
        /// The messages
        /// </summary>
        private SoapServiceMessages _messages;

        /// <summary>
        /// Gets the request message.
        /// </summary>
        /// <value>
        /// The request message.
        /// </value>
        public string RequestMessage
        {
            get { return _messages.Request; }
            set { _messages.Request = value; }
        }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        /// <value>
        /// The response message.
        /// </value>
        public string ResponseMessage
        {
            get { return _messages.Response; }
            set { _messages.Response = value; }
        }

        /// <summary>
        /// Afters the receive reply.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <param name="correlationState">State of the correlation.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            _messages.Response = reply.ToString();
        }

        /// <summary>
        /// Befores the send request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channel">The channel.</param>
        /// <returns></returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            _messages.Request = request.ToString();
            return _messages.Request;
        }
    }

    /// <summary>
    /// Soap Inspector Behavior
    /// </summary>
    /// <seealso cref="System.ServiceModel.Description.IEndpointBehavior" />
    public class SoapInspectorBehavior : IEndpointBehavior
    {
        /// <summary>
        /// The message inspector
        /// </summary>
        SoapMessageInspector _messageInspector = new SoapMessageInspector();

        /// <summary>
        /// Gets the message inspector.
        /// </summary>
        /// <value>
        /// The message inspector.
        /// </value>
        public SoapMessageInspector MessageInspector
        {
            get { return _messageInspector; }
            set { _messageInspector = value; }
        }

        /// <summary>
        /// Adds the binding parameters.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="bindingParameters">The binding parameters.</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // Not required
        }

        /// <summary>
        /// Applies the client behavior.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="clientRuntime">The client runtime.</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(_messageInspector);
        }

        /// <summary>
        /// Applies the dispatch behavior.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher.</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            // Not required
        }

        /// <summary>
        /// Validates the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public void Validate(ServiceEndpoint endpoint)
        {
            // Not required
        }
    }
}
