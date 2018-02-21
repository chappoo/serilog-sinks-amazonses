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

using System.ComponentModel;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;

namespace Serilog.Sinks.AmazonSimpleEmailService
{
    public class AmazonSimpleEmailServiceConfig
    {
        public AmazonSimpleEmailServiceConfig()
        {
            EmailSubject = DefaultEmailSubject;
            IsBodyHtml = false;
        }
        /// <summary>
        /// The default subject used for email messages.
        /// </summary>
        public const string DefaultEmailSubject = "Log Email";

        /// <summary>
        /// The email address emails will be sent from.
        /// </summary>
        public string EmailFrom { get; set; }

        /// <summary>
        /// The email address(es) emails will be sent to. Accepts multiple email addresses separated by comma or semicolon.
        /// </summary>
        public string EmailTo { get; set; }

        /// <summary>
        /// The subject to use for the email.
        /// </summary>
        [DefaultValue(DefaultEmailSubject)]
        public string EmailSubject { get; set; }

        /// <summary>
        /// Sets whether the body contents of the email is HTML. Defaults to false.
        /// </summary>
        public bool IsBodyHtml { get; set; }

        /// <summary>
        /// The Amazon SES Client
        /// </summary>
        public AmazonSimpleEmailServiceClient AmazonSimpleEmailServiceClient { get; set; }
    }
}