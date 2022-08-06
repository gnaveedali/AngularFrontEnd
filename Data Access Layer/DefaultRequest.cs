using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Data_Access_Layer
{
    [DataContract]
    public class DefaultRequest
    {
        [DataMember]
        public int AppTypeID { get; set; }

        [DataMember]
        public string AppVersion { get; set; }

        [DataMember]
        public int LanguageID { get; set; }


        [DataMember]
        public bool HasError { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public bool IsSync { get; set; }



        public DefaultRequest()
        {
            AppTypeID = 1;
            AppVersion = "1.0";
            LanguageID = 1;
            HasError = false;
            ErrorMessage = "";
            IsSync = false;
        }
    }
}
