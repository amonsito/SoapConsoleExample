namespace ConsoleApp1
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;

    /// <summary>
    /// Generic Proxy for SOAP service consumption
    /// </summary>
    /// <typeparam name="TContract">The type of the contract.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public class GenericProxy<TContract> : IDisposable where TContract : class
    {

        /// <summary>
        /// The SOAP inspectorbehavior
        /// </summary>
        private SoapInspectorBehavior _soapInspectorbehavior;

        /// <summary>
        /// The channel factory
        /// </summary>
        private readonly ChannelFactory<TContract> _channelFactory;

        /// <summary>
        /// The channel
        /// </summary>
        private TContract _channel;

        /// <summary>
        /// The logger service.
        /// </summary>
        //readonly ILoggerService _loggerService;

        /// <summary>
        /// The HTTPS protocol
        /// </summary>
        const string _httpsProtocol = "https";

        /// <summary>
        /// The HTTP protocol
        /// </summary>
        const string _httpProtocol = "http";

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericProxy{TContract}"/> class.
        /// </summary>
        /// <param name="url">The service options.</param>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="loggerService">The logger service.</param>
        public GenericProxy(string url,int timeOut)
        {
            if (url == null)
            {
                return;
            }

            //_loggerService = loggerService;

            HttpBindingBase binding = url.Contains(_httpsProtocol) ? GetSpecificBinding(_httpsProtocol) : GetSpecificBinding(_httpProtocol);
            binding.MaxReceivedMessageSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.ReceiveTimeout = new TimeSpan(0, 0, timeOut);
            binding.CloseTimeout = new TimeSpan(0, 0, timeOut);
            binding.OpenTimeout = new TimeSpan(0, 0, timeOut);
            binding.SendTimeout = new TimeSpan(0, 0, timeOut);

            EndpointAddress Endpoint = new EndpointAddress(url);

            _soapInspectorbehavior = new SoapInspectorBehavior();
            _channelFactory = new ChannelFactory<TContract>(binding, Endpoint);
            _channelFactory.Endpoint.EndpointBehaviors.Add(_soapInspectorbehavior);
        }

        /// <summary>
        /// Executes the specified function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        public TResult Execute<TResult>(Func<TContract, TResult> function)
        {
            var result = default(TResult);
            var elapsed = string.Empty;

            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                result = function.Invoke(Channel);
                stopWatch.Stop();
                elapsed = stopWatch.Elapsed.ToString();
                //Log(_soapInspectorbehavior.MessageInspector.RequestMessage, _soapInspectorbehavior.MessageInspector.ResponseMessage, elapsed);
            }
            catch (Exception ex)
            {
                //LogError(ex, _soapInspectorbehavior.MessageInspector.RequestMessage, _soapInspectorbehavior.MessageInspector.ResponseMessage, elapsed);
            }
            return result;
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        private TContract Channel
        {
            get
            {
                if (_channel == null)
                {
                    _channel = _channelFactory.CreateChannel();
                }

                return _channel;
            }
        }

        /// <summary>
        /// Gets the specific binding.
        /// </summary>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns></returns>
        private HttpBindingBase GetSpecificBinding(string bindingType)
        {
            if (bindingType.Equals("https"))
            {
                return new BasicHttpsBinding();
            }
            return new BasicHttpBinding();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_channel != null)
                {
                    var currentChannel = _channel as IClientChannel;
                    if (currentChannel != null && currentChannel.State == CommunicationState.Faulted)
                    {
                        currentChannel.Abort();
                    }
                    else
                    {
                        currentChannel?.Close();
                    }
                }

                _soapInspectorbehavior = null;
            }
            finally
            {
                _channel = null;
                GC.SuppressFinalize(this);
            }
        }

        //void Log(string input, string output, string elapsed)
        //{
        //    if (_loggerService != null)
        //    {
        //        Task.Run(async () =>
        //        {
        //            await _loggerService.LogAsync(LoggerType.SOAP_ExternalServiceTraceability, input, output, elapsed, _channelFactory.Endpoint.Address.Uri.AbsoluteUri).ConfigureAwait(false);
        //        });
        //    }
        //}

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="input">The input.</param>
        //void LogError(Exception exception, string input, string output, string time)
        //{
        //    if (_loggerService != null)
        //    {
        //        Task.Run(async () =>
        //        {
        //            await _loggerService.LogAsync(LoggerType.SOAP_ErrorExternalService, exception.ToString(), input, output, time, _channelFactory.Endpoint.Address.Uri.AbsoluteUri).ConfigureAwait(false);
        //        });
        //    }
        //}
    }
}

