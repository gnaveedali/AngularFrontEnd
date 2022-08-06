
using Data_Access_Layer;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Standard_Trading_API.Model
{
    public class UserDetailsDataType
    {
        [DataContract]
        public class UserDetails : DefaultRequest
        {
            [DataMember]
            public int? Id { get; set; }
            [DataMember]
            public string? fname { get; set; }
            [DataMember]
            public string? lname { get; set; }
            [DataMember]
            public string? position { get; set; }
            [DataMember]
            public int? groupId { get; set; }
            [DataMember]
            public string? addres { get; set; }
            [DataMember]
            public string? contact { get; set; }
            [DataMember]
            public int? city { get; set; }
            [DataMember]
            public int? country { get; set; }
            [DataMember]
            public string? email { get; set; }
            [DataMember]
            public string? password { get; set; }
            [DataMember]
            public string? Photo { get; set; }

            [DataMember]
            public bool? canAccessDashboard { get; set; }

            [DataMember]
            public bool? canAccessUserProfile { get; set; }
            
           
        }
        [DataContract]
        public class UserClaims
        {
            [DataMember]
            public bool? canAccessDashboard { get; set; }

            [DataMember]
            public bool? canAccessUserProfile { get; set; }

            [DataMember]
            public int? ClaimId { get; set; }
            [DataMember]
            public int? UserId { get; set; }
        }
       

        [DataContract]
        public class UserAuthenticationObject
        {
            [DataMember]
            public string userName { get; set; }
            [DataMember]

            public string role { get; set; }
            [DataMember]

            public string email { get; set; }
            [DataMember]
            public bool isAuthenticated { get; set; }
            [DataMember]
            public string bearerToken { get; set; }
            [DataMember]
            public int groupID { get; set; }

            [DataMember]
            public bool canAccessDashboard { get; set; }

            [DataMember]
            public bool canAccessUserProfile { get; set; }
        }

    }
}
