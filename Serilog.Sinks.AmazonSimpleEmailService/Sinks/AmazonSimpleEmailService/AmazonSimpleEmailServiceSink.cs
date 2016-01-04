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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.AmazonSimpleEmailService
{
    public class AmazonSimpleEmailServiceSink : PeriodicBatchingSink
    {
        private readonly AmazonSimpleEmailServiceClient _client;

        private readonly ITextFormatter _textFormatter;

        /// <summary>
        /// A reasonable default for the number of events posted in
        /// each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 100;

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(30);

        private readonly AmazonSimpleEmailServiceConfig _config;

        /// <summary>
        /// Construct a sink emailing via Amazon SES with the specified details.
        /// </summary>
        /// <param name="config">Configuration used to construct the Amazon SES client and mail messages.</param>
        /// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="textFormatter">Supplies culture-specific formatting information, or null.</param>
        public AmazonSimpleEmailServiceSink(AmazonSimpleEmailServiceConfig config, int batchSizeLimit, TimeSpan period, ITextFormatter textFormatter)
            : base(batchSizeLimit, period)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config;
            _textFormatter = textFormatter;
            _client = CreateClient();
            _client.AfterResponseEvent += ClientOnAfterResponseEvent;
            _client.ExceptionEvent += ClientOnExceptionEvent;
        }

        private static void ClientOnExceptionEvent(object sender, ExceptionEventArgs e)
        {
            SelfLog.WriteLine("Received failed result {0}: {1}", "Error", e);
        }

        private static void ClientOnAfterResponseEvent(object sender, ResponseEventArgs e)
        {
            SelfLog.WriteLine("Received result {0}", e);
        }

        /// <summary>
        /// Free resources held by the sink.
        /// </summary>
        /// <param name="disposing">If true, called because the object is being disposed; if false,
        /// the object is being disposed from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            // First flush the buffer
            base.Dispose(disposing);

            if (disposing)
                _client.Dispose();
        }

        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var payload = new StringWriter();

            foreach (var logEvent in events)
            {
                _textFormatter.Format(logEvent, payload);
            }

            var request = new SendEmailRequest
            {
                Destination = new Destination
                {
                    ToAddresses = new List<string>(_config.ToEmail.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                },
                Message = new Message(new Content(_config.EmailSubject), new Body
                {
                    Text = !_config.IsBodyHtml ? new Content(payload.ToString()) : null,
                    Html = _config.IsBodyHtml ? new Content(payload.ToString()) : null
                }),
                Source = _config.FromEmail
            };

            _client.SendEmail(request);
        }

#if NET45
        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        /// <remarks>Override either <see cref="PeriodicBatchingSink.EmitBatch"/> or <see cref="PeriodicBatchingSink.EmitBatchAsync"/>,
        /// not both.</remarks>
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {

            if (events == null)
                throw new ArgumentNullException(nameof(events));
            var payload = new StringWriter();

            foreach (var logEvent in events)
            {
                _textFormatter.Format(logEvent, payload);
            }

            var request = new SendEmailRequest
            {
                Destination = new Destination
                {
                    ToAddresses = new List<string>(_config.ToEmail.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                },
                Message = new Message(new Content(_config.EmailSubject), new Body
                {
                    Text = !_config.IsBodyHtml ? new Content(payload.ToString()) : null,
                    Html = _config.IsBodyHtml ? new Content(payload.ToString()) : null
                }),
                Source = _config.FromEmail
            };

            await _client.SendEmailAsync(request);
        }
#endif

        private AmazonSimpleEmailServiceClient CreateClient()
        {
            var sesClient = new AmazonSimpleEmailServiceClient(new BasicAWSCredentials(_config.AwsAccessKeyId, _config.AwsSecretKey), new Amazon.SimpleEmail.AmazonSimpleEmailServiceConfig
            {
                RegionEndpoint = _config.RegionEndpoint,
                SignatureMethod = _config.SignatureMethod
            });
            return sesClient;
        }
    }
}
