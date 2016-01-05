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

namespace Serilog.Sinks.AmazonSimpleEmailService
{
    public class AmazonSimpleEmailServiceConfig
    {
        public AmazonSimpleEmailServiceConfig()
        {
            EmailSubject = DefaultEmailSubject;
            IsBodyHtml = false;
            RegionEndpoint = DefaultRegionEndpoint;
            SignatureMethod = DefaultSignatureMethod;
        }
        /// <summary>
        /// The default subject used for email messages.
        /// </summary>
        public const string DefaultEmailSubject = "Log Email";

        /// <summary>
        /// The default region endpoint
        /// </summary>
        public static readonly RegionEndpoint DefaultRegionEndpoint = RegionEndpoint.USEast1;

        /// <summary>
        /// The default signature method
        /// </summary>
        public static readonly SigningAlgorithm DefaultSignatureMethod = SigningAlgorithm.HmacSHA256;

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
        /// The AWS access key id
        /// </summary>
        public string AwsAccessKeyId { get; set; }

        /// <summary>
        /// The AWS secret key
        /// </summary>
        public string AwsSecretKey { get; set; }

        /// <summary>
        /// The region endpoint to use for SES
        /// </summary>
        public RegionEndpoint RegionEndpoint { get; set; }

        /// <summary>
        /// The signing algorithm to use for SES
        /// </summary>
        public SigningAlgorithm SignatureMethod { get; set; }
    }
}