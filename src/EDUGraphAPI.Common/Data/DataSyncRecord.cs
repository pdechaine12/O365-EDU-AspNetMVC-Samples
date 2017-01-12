using System;

namespace EDUGraphAPI.Data
{
    public class DataSyncRecord
    {
        public int Id { get; set; }

        public string TenantId { get; set; }

        public string Query { get; set; }

        public string DeltaLink { get; set; }

        public DateTime Updated { get; set; }
    }
}