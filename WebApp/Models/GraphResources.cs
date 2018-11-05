using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft_Graph_REST_ASPNET_Connect.Models
{
    public class OfficeHoursUserInfo
    {
        public string Address { get; set; }
    }

    public class OfficeHoursFileInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SharingLink { get; set; }
    }

    public class OfficeHoursMessage
    {
        public string Subject { get; set; }
        public OfficeHoursItemBody Body { get; set; }
        public List<OfficeHoursRecipient> ToRecipients { get; set; }
        public List<OfficeHoursAttachment> Attachments { get; set; }
    }

    public class OfficeHoursRecipient
    {
        public OfficeHoursUserInfo EmailAddress { get; set; }
    }

    public class OfficeHoursItemBody
    {
        public string ContentType { get; set; }
        public string Content { get; set; }
    }

    public class OfficeHoursMessageRequest
    {
        public OfficeHoursMessage Message { get; set; }
        public bool SaveToSentItems { get; set; }
    }

    public class OfficeHoursAttachment
    {
        [JsonProperty(PropertyName = "@odata.type")]
        public string ODataType { get; set; }
        public byte[] ContentBytes { get; set; }
        public string Name { get; set; }
    }

    public class OfficeHoursPermissionInfo
    {
        public OfficeHoursSharingLinkInfo Link { get; set; }
    }

    public class OfficeHoursSharingLinkInfo
    {
        public OfficeHoursSharingLinkInfo(string type)
        {
            Type = type;
        }

        public string Type { get; set; }
        public string WebUrl { get; set; }
    }
}