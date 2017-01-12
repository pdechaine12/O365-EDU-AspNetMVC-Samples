using Newtonsoft.Json;
using System.Collections.Generic;

namespace EDUGraphAPI.DifferentialQuery
{
    public interface IDeltaEntity
    {
        [JsonProperty("aad.isDeleted")]
        bool IsDeleted { get; set; }

        HashSet<string> ModifiedPropertyNames { get; }
    }
}