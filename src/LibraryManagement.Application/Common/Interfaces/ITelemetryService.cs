using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Common.Interfaces
{
    public interface ITelemetryService
    {
        void TrackEvent(string eventName, IDictionary<string, string>? properties = null);
        void TrackMetric(string metricName, double value);
        void TrackException(Exception exception, IDictionary<string, string>? properties = null);
    }
}
