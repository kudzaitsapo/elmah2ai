using ElmahCore;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elmah2AI.AppInsights
{
    /// <summary>
    ///     An <see cref="ErrorLog" /> implementation that uses Azure Application Insights
    ///     as its backing store.
    /// </summary>
    public class AIErrorLog : ErrorLog
    {
        /// <summary>
        ///  Telemetry client declaration. 
        /// </summary>

        private readonly TelemetryClient _telemetryClient;


        /// <summary>
        ///  Initializes the Application insights from Elmah Connection string. 
        /// </summary>
        /// <param name="option"></param>
        public AIErrorLog(IOptions<ElmahOptions> option) : this(option.Value.ConnectionString)
        {

        }

        /// <summary>
        /// TODO: Replace instrument key initialization with connection string.
        /// </summary>
        /// <param name="instrumentationKey"></param>
        public AIErrorLog(string instrumentationKey)
        {
            if (string.IsNullOrEmpty(instrumentationKey))
                throw new ArgumentNullException(nameof(instrumentationKey));

            // Telemetry client initialization
            IServiceCollection services = new ServiceCollection();
            services.AddApplicationInsightsTelemetryWorkerService(instrumentationKey);

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            _telemetryClient = serviceProvider.GetRequiredService<TelemetryClient>();
        }

        /// <summary>
        ///     Gets the name of this error log implementation.
        /// </summary>
        public override string Name => "Application Insights";

        /// <summary>
        ///     Gets the connection string used by the log to connect to Application Insights.
        ///     In this case, it will be the instrumentation key.
        /// </summary>
        public virtual string ConnectionString { get; }
        

        public override ErrorLogEntry GetError(string id)
        {
            throw new NotImplementedException();
        }

        public override int GetErrors(int errorIndex, int pageSize, ICollection<ErrorLogEntry> errorEntryList)
        {
            throw new NotImplementedException();
        }

        public override string Log(Error error)
        {
            var id = Guid.NewGuid();

            Log(id, error);

            return id.ToString();
        }

        public override void Log(Guid id, Error error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            var errorInfo = new ErrorDto
            {
                Id = id,
                AppName = ApplicationName,
                HostName = error.HostName,
                TypeName = error.Type,
                Source = error.Source,
                Message = error.Message,
                User = error.User,
                StatusCode = error.StatusCode,
                Time = error.Time,
                Xml = ErrorXml.EncodeString(error)
            };

            var errorProperties = CreateErrorProperties<string>(errorInfo);
            var exception = new Exception(error.Message);
            
            using(_telemetryClient.StartOperation<RequestTelemetry>($"{ApplicationName} Logs"))
            {
                _telemetryClient.TrackException(exception, errorProperties);
                _telemetryClient.Flush();
            }


        }

        private static Dictionary<string, TValue> CreateErrorProperties<TValue>(ErrorDto errorInfo)
        {
            var json = JsonConvert.SerializeObject(errorInfo);
            return JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
        }
    }
}
