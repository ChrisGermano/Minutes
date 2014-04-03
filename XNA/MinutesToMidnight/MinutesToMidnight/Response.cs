
using System;
using System.Runtime.Serialization;

namespace MinutesToMidnight
{
    [DataContract]
    public class Response
    {
        [DataMember(Name = "role", IsRequired = true)]
        private ROLE role = ROLE.civilian;

        [DataMember(Name = "importance", IsRequired = true)]
        private IMPORTANCE importance = IMPORTANCE.low;

        [DataMember(Name = "verity", IsRequired = true)]
        public VERITY verity = VERITY.false_opinion;

        [DataMember(Name = "prompt", IsRequired = true)]
        public System.String prompt = "Let me tell you...";

        [DataMember(Name = "dialog", IsRequired = true)]
        public System.String dialog = "It's all a conspiracy!";

        [DataMember(Name = "locked", IsRequired = true)]
        private bool locked = false;

        [DataMember(Name = "discovered", IsRequired = true)]
        private bool discovered = false;

        [DataMember(Name = "marked", IsRequired = true)]
        private MARKEDAS marked = MARKEDAS.huh;


        [DataMember(Name = "responsePrompt", IsRequired = true)]
        public string responsePrompt = "Ask";


        [DataMember(Name = "replace", IsRequired = true)]
        private bool replace = false;

        /// <summary>
        /// ensures that a specific response can be applied to the same
        /// person as the previous response
        /// </summary>
        [DataMember(Name = "tietoprevious", IsRequired = true)]
        public bool tietoprevious = false;

        public Response()
        {

        }

        //Create default Response for testing
        public Response(String text)
        {
            role = ROLE.civilian;
            importance = IMPORTANCE.medium;
            verity = VERITY.true_opinion;
            prompt = "let me tell you..";
            dialog = text;
            discovered = false;
            marked = MARKEDAS.huh;
        }



        internal ROLE getRole()
        {
            return this.role;
        }
    }
}