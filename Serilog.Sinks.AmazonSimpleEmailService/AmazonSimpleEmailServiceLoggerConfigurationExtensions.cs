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
using Amazon;
using Amazon.Runtime;
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
        private const string DefaultAwsRegionEndpoint = "us-east-1";

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// Cannot be used by the AppSettings reader.
        /// Allows for complete customisation of the AmazonSimpleEmailServiceClient (passed in).
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="amazonSimpleEmailServiceClient"></param>
        /// <param name="emailFrom">The 'From' email address</param>
        /// <param name="emailTo">The 'To' email address</param>
        /// <param name="emailSubject">The email subject</param>
        /// <param name="isBodyHtml">True if body is HTML.  Defaults to false.</param>
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

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// Can be used by the AppSettings reader.
        /// Allows for explicit specification of the AWS credentials.
        /// It's recommended to use one of the other overloads that uses the default AWS SDK credential discovery implementation.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="accessKey">AWS Access Key.</param>
        /// <param name="secretKey">AWS Secret Key.</param>
        /// <param name="emailFrom">The 'From' email address</param>
        /// <param name="emailTo">The 'To' email address</param>
        /// <param name="awsRegionEndpoint">The AWS region endpoint for SES; must match one of the valid AWS region system names. Defaults to 'us-east-1'.</param>
        /// <param name="emailSubject">The email subject</param>
        /// <param name="isBodyHtml">True if body is HTML.  Defaults to false.</param>
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
            string accessKey,
            string secretKey,
            string emailFrom,
            string emailTo,
            string awsRegionEndpoint = DefaultAwsRegionEndpoint,
            string emailSubject = DefaultEmailSubject,
            bool isBodyHtml = false,
            string outputTemplate = DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = AmazonSimpleEmailServiceSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.AmazonSimpleEmailService(
                new AmazonSimpleEmailServiceClient(new BasicAWSCredentials(accessKey, secretKey), RegionEndpoint.GetBySystemName(awsRegionEndpoint)),
                emailFrom,
                emailTo,
                emailSubject,
                isBodyHtml,
                outputTemplate,
                restrictedToMinimumLevel,
                batchPostingLimit,
                period,
                formatProvider);
        }

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// Can be used by the AppSettings reader.
        /// Uses the default AWS SDK credential discovery implementation.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="emailFrom">The 'From' email address</param>
        /// <param name="emailTo">The 'To' email address</param>
        /// <param name="regionEndpoint">The AWS region endpoint for SES; must match one of the valid AWS region system names. Defaults to 'us-east-1'.</param>
        /// <param name="emailSubject">The email subject</param>
        /// <param name="isBodyHtml">True if body is HTML.  Defaults to false.</param>
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
            string emailFrom,
            string emailTo,
            string regionEndpoint = DefaultAwsRegionEndpoint,
            string emailSubject = DefaultEmailSubject,
            bool isBodyHtml = false,
            string outputTemplate = DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = AmazonSimpleEmailServiceSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.AmazonSimpleEmailService(
                new AmazonSimpleEmailServiceClient(RegionEndpoint.GetBySystemName(regionEndpoint)),
                emailFrom,
                emailTo,
                emailSubject,
                isBodyHtml,
                outputTemplate,
                restrictedToMinimumLevel,
                batchPostingLimit,
                period,
                formatProvider);
        }
    }
}