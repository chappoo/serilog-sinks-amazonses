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
        /// <summary>
        ///     A reasonable default for the number of events posted in
        ///     each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 100;

        /// <summary>
        ///     A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(30);

        private readonly AmazonSimpleEmailServiceClient _client;
        private readonly string _emailFrom;
        private readonly string _emailSubject;
        private readonly string _emailTo;
        private readonly bool _isBodyHtml;

        private readonly ITextFormatter _textFormatter;

        /// <inheritdoc />
        /// <summary>
        ///     Construct a sink emailing via Amazon SES with the specified details.
        /// </summary>
        /// <param name="isBodyHtml"></param>
        /// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="textFormatter">Supplies culture-specific formatting information, or null.</param>
        /// <param name="client"></param>
        /// <param name="emailFrom"></param>
        /// <param name="emailTo"></param>
        /// <param name="emailSubject"></param>
        public AmazonSimpleEmailServiceSink(AmazonSimpleEmailServiceClient client, 
            string emailFrom,
            string emailTo,
            string emailSubject,
            bool isBodyHtml,
            int batchSizeLimit,
            TimeSpan period,
            ITextFormatter textFormatter)
            : base(batchSizeLimit, period)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _client.AfterResponseEvent += ClientOnAfterResponseEvent;
            _client.ExceptionEvent += ClientOnExceptionEvent;
            _emailFrom = emailFrom;
            _emailTo = emailTo;
            _emailSubject = emailSubject;
            _isBodyHtml = isBodyHtml;
            _textFormatter = textFormatter;
        }

        private static void ClientOnExceptionEvent(object sender, ExceptionEventArgs e)
        {
            SelfLog.WriteLine("Received failed result {0}: {1}", "Error", e);
        }

        private static void ClientOnAfterResponseEvent(object sender, ResponseEventArgs e)
        {
            SelfLog.WriteLine("Received result {0}", e);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Free resources held by the sink.
        /// </summary>
        /// <param name="disposing">
        ///     If true, called because the object is being disposed; if false,
        ///     the object is being disposed from the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // First flush the buffer
            base.Dispose(disposing);

            if (disposing)
                _client.Dispose();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var payload = new StringWriter();

            foreach (var logEvent in events)
                _textFormatter.Format(logEvent, payload);

            var request = new SendEmailRequest
            {
                Destination = new Destination
                {
                    ToAddresses =
                        new List<string>(_emailTo.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                },
                Message = new Message(new Content(_emailSubject), new Body
                {
                    Text = !_isBodyHtml ? new Content(payload.ToString()) : null,
                    Html = _isBodyHtml ? new Content(payload.ToString()) : null
                }),
                Source = _emailFrom
            };

            await _client.SendEmailAsync(request);

            // could do some further validation processing with the await response
        }
    }
}