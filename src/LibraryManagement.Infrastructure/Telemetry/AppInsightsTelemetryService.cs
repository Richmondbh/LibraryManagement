using LibraryManagement.Application.Common.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Telemetry
{
    public class AppInsightsTelemetryService: ITelemetryService
    {
        private readonly TelemetryClient _telemetryClient;

        public AppInsightsTelemetryService(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public void TrackEvent(string eventName, IDictionary<string, string>? properties = null)
        {
            _telemetryClient.TrackEvent(eventName, properties);
        }

        public void TrackMetric(string metricName, double value)
        {
            _telemetryClient.TrackMetric(metricName, value);
        }

        public void TrackException(Exception exception, IDictionary<string, string>? properties = null)
        {
            _telemetryClient.TrackException(exception, properties);
        }
    }
}
