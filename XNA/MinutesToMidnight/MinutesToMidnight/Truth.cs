using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace MinutesToMidnight
{
    [DataContract]
    public class Truth
    {
        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; set; }

        [DataMember(Name = "responses", IsRequired = true)]
        public List<Response> Responses { get; set; }

        public Truth()
        {
            Name = "sample";
            Responses = new List<Response>();

            for (int i = 0; i < 5; i++)
            {
                Responses.Add(new Response());
            }
        }
    }
}
