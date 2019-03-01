using System.Runtime.Serialization;

namespace CalibreSetMetaData.Entity
{

    [DataContract]
    public class CalibreList
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "originaltitle")]
        public string OriginalTitle { get; set; }
    }

}
