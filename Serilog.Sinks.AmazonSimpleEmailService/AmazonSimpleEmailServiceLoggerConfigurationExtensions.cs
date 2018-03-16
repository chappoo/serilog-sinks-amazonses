// Copyright 2016 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Amazon.SimpleEmail;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.AmazonSimpleEmailService;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.AmazonSimpleEmailService() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class AmazonSimpleEmailServiceLoggerConfigurationExtensions
    {
        private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";
        private const string DefaultEmailSubject = "Log Email";

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="amazonSimpleEmailServiceClient"></param>
        /// <param name="emailFrom"></param>
        /// <param name="emailTo"></param>
        /// <param name="emailSubject"></param>
        /// <param name="isBodyHtml"></param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration AmazonSimpleEmailService(
            this LoggerSinkConfiguration loggerConfiguration,
            AmazonSimpleEmailServiceClient amazonSimpleEmailServiceClient,
            string emailFrom,
            string emailTo,
            string emailSubject = DefaultEmailSubject,
            bool isBodyHtml = false,
            string outputTemplate = DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = AmazonSimpleEmailServiceSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null)
        {
            if (amazonSimpleEmailServiceClient == null) throw new ArgumentNullException(nameof(amazonSimpleEmailServiceClient));

            var defaultedPeriod = period ?? AmazonSimpleEmailServiceSink.DefaultPeriod;
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);

            return loggerConfiguration.Sink(
                new AmazonSimpleEmailServiceSink(amazonSimpleEmailServiceClient, emailFrom, emailTo, emailSubject, isBodyHtml, batchPostingLimit, defaultedPeriod, formatter),
                restrictedToMinimumLevel);
        }

    }
}