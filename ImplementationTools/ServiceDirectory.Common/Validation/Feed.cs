﻿using System;

namespace ServiceDirectory.Common.Validation
{
    public class Feed
    {
        public string Url { get; set; }
        public string Label { get; set; }
        public string OrganisationLabel { get; set; }
        public string OrganisationUrl { get; set; }
        public string DeveloperLabel { get; set; }
        public string DeveloperUrl { get; set; }
        public string ServicePathOverride { get; set; }

        public DateTime LastCheck { get; set; }
        public bool CheckIsRunning { get; set; }
        public long TimeTaken { get; set; }

        public bool IsUp { get; set; }

        public bool IsServicesValid { get; set; }
        public string ServicesMessage { get; set; }

        public bool IsServiceExampleValid { get; set; }
        public string ServiceExampleIdentifier { get; set; }
        public string ServiceExampleMessage { get; set; }

        public bool IsSearchEnabled { get; set; }
        public string SearchEnabledMessage { get; set; }

        public static Feed CreateRunning(Feed feed)
        {
            return new Feed
            {
                Url = feed.Url,
                Label = feed.Label,
                OrganisationLabel = feed.OrganisationLabel,
                OrganisationUrl = feed.OrganisationUrl,
                DeveloperLabel = feed.DeveloperLabel,
                DeveloperUrl = feed.DeveloperUrl,
                ServicePathOverride = feed.ServicePathOverride,

                LastCheck = DateTime.UtcNow,
                CheckIsRunning = true,
                TimeTaken = -1,

                IsUp = false,

                IsServicesValid = false,
                ServicesMessage = null,

                IsServiceExampleValid = false,
                ServiceExampleIdentifier = null,
                ServiceExampleMessage = null,

                IsSearchEnabled = false,
                SearchEnabledMessage = null,
            };
        }
    }
}
