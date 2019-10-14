﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace ServiceLoader.JsonMappingObjects
{
    public class Result
    {
        [JsonProperty("assetId")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parentOrganisation")]
        public string Organisation { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("keywords")]
        public string[] Keywords { get; set; }

        [JsonProperty("accessibility")]
        public string[] Accessibilities { get; set; }

        [JsonProperty("ageGroups")]
        public string[] AgeGroups { get; set; }

        [JsonProperty("suitability")]
        public string[] Suitabilities { get; set; }

        [JsonProperty("venue")]
        public string Venue { get; set; }

        [JsonProperty("area")]
        public string Area { get; set; }

        [JsonProperty("postcode")]
        public string PostCode { get; set; }

        [JsonProperty("geo")]
        public Geo Geo { get; set; }

        [JsonProperty("frequency")]
        public string Frequency { get; set; }

        [JsonProperty("days")]
        public string[] Days { get; set; }

        [JsonProperty("contactName")]
        public string ContactName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("lastUpdated")]
        public DateTime? LastUpdated { get; set; }

        [JsonProperty("confidDataProtect")]
        public string ConfidentialData { get; set; }

        [NotMapped]
        public bool HasConfidentialData => !string.IsNullOrEmpty(ConfidentialData) &&
                                    (ConfidentialData.StartsWith("true", StringComparison.OrdinalIgnoreCase) ||
                                     ConfidentialData.StartsWith("yes", StringComparison.OrdinalIgnoreCase));

        [NotMapped]
        public string OrganisationId => Organisation ?? ServiceId;

        [NotMapped]
        public string OrganisationName => Organisation ?? string.Empty;

        [NotMapped]
        public string OrganisationDescription => Organisation ?? string.Empty;

        [NotMapped]
        public string OrganisationUrl => Url;

        [NotMapped]
        public string ServiceId => Id.ToString(CultureInfo.InvariantCulture);

        [NotMapped]
        public string ServiceName => Name ?? string.Empty;

        [NotMapped]
        public string ServiceDescription => Description ?? string.Empty;

        [NotMapped]
        public string ServiceAreaId => Area?.ToLowerInvariant();

        [NotMapped]
        public string LocationId => HasConfidentialData ? string.Empty : Venue?.ToLowerInvariant();

        [NotMapped]
        public string LocationName => HasConfidentialData ? string.Empty : Venue?.ToLowerInvariant();

        [NotMapped]
        public string LocationDescription => HasConfidentialData ? string.Empty : Venue?.ToLowerInvariant();

        [NotMapped]
        public double? LocationLatitude => HasConfidentialData ? null : Geo?.Latitude;

        [NotMapped]
        public double? LocationLongitude => HasConfidentialData ? null : Geo?.Longitude;

        [NotMapped]
        public string ServiceAtLocationId => !HasConfidentialData && !string.IsNullOrEmpty(LocationId) ? $"{ServiceId}:{LocationId}" : null;

        [NotMapped]
        public IEnumerable<Taxonomy> Taxonomies
        {
            get
            {
                if (!string.IsNullOrEmpty(Category)) yield return new Taxonomy($"category:{Category}", Category, "Bucks:category");
                foreach (var keyword in Keywords)
                {
                    yield return new Taxonomy($"keyword:{keyword}", keyword, "Bucks:keyword");
                }

                foreach (var accessibility in Accessibilities)
                {
                    yield return new Taxonomy($"accessibility:{accessibility}", accessibility, "Bucks:accessibility");
                }

                foreach (var ageGroup in AgeGroups)
                {
                    yield return new Taxonomy($"age-group:{ageGroup}", ageGroup, "Bucks:age-group");
                    switch (ageGroup.ToLowerInvariant())
                    {
                        case "young people":
                            yield return new Taxonomy("OpenEligibility:20005:children", "Children", "OpenEligibility:age-group");
                            break;
                        case "young adults":
                            yield return new Taxonomy("OpenEligibility:20004:teens", "Teens", "OpenEligibility:age-group");
                            break;
                        case "older adults":
                            yield return new Taxonomy("OpenEligibility:20003:adults", "Adults", "OpenEligibility:age-group");
                            yield return new Taxonomy("OpenEligibility:20006:seniors", "Seniors", "OpenEligibility:age-group");
                            break;
                    }
                }

                foreach (var suitability in Suitabilities)
                {
                    yield return new Taxonomy($"suitability:{suitability}", suitability, "Bucks:suitability");
                    switch (suitability.ToLowerInvariant())
                    {
                        case "autism":
                            yield return new Taxonomy("OpenEligibility:20019:developmental-disability", "Developmental Disability", "OpenEligibility:disability");
                            break;
                        case "dementia":
                        case "mental health":
                        case "mental health/acquired brain injury":
                            yield return new Taxonomy("OpenEligibility:20025:mental-illness", "Mental Illness", "OpenEligibility:disability");
                            break;
                        case "hearing or visual impairment":
                        case "visual and/or hearing impediment":
                            yield return new Taxonomy("OpenEligibility:20023:hearing-impairment", "Hearing Impairment", "OpenEligibility:disability");
                            yield return new Taxonomy("OpenEligibility:20024:visual-impairment", "Visual Impairment", "OpenEligibility:disability");
                            break;
                        case "learning difficulties":
                        case "learning disability":
                            yield return new Taxonomy("OpenEligibility:20018:learning-disability", "Learning Disability", "OpenEligibility:disability");
                            break;
                        case "physical difficulty":
                            yield return new Taxonomy("OpenEligibility:20022:limited-mobility", "Limited Mobility", "OpenEligibility:disability");
                            break;
                        case "physical disability":
                            yield return new Taxonomy("OpenEligibility:20020:physical-disability", "Physical Disability", "OpenEligibility:disability");
                            break;
                    }
                }
            }
        }

        [NotMapped]
        public string ContactId => !string.IsNullOrEmpty(ContactName) ? $"{ServiceId}:{ContactName}" : ServiceId;

        [NotMapped]
        public string AddressId => HasConfidentialData ? string.Empty : $"{LocationId}:{PostCode}:{LocationLongitude}:{LocationLatitude}";

        [NotMapped]
        public string AddressLine1 => HasConfidentialData ? string.Empty : Venue;

        [NotMapped]
        public string AddressCity => HasConfidentialData ? string.Empty : Area;

        [NotMapped]
        public string AddressStateProvince => string.Empty;

        [NotMapped]
        public string AddressCountry => HasConfidentialData ? string.Empty : "England";

        [NotMapped]
        public string AddressPostCode => HasConfidentialData ? string.Empty : PostCode ?? string.Empty;

        [NotMapped]
        public IEnumerable<Schedule> Schedules
        {
            get
            {
                var validDays = new string[] { "SU", "MO", "TU", "WE", "TH", "FR", "SA" };
                foreach (var day in Days.Distinct())
                {
                    if (day.Length < 2) throw new Exception($"Invalid day {day} for service {ServiceId}");
                    var dayAbbrv = day.Substring(0, 2).ToUpperInvariant();
                    if (!validDays.Contains(dayAbbrv, StringComparer.Ordinal)) throw new Exception($"Invalid day {day} for service {ServiceId}");

                    yield return new Schedule($"{ServiceId}:{day}", dayAbbrv, Frequency);
                }
            }
        }

        [NotMapped]
        public string CostOptionId => ServiceId;
    }
}