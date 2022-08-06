using Data_Access_Layer;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Standard_Trading_API.Model
{

    
    public class MenusDataType 
    {
        [DataContract]
        public class Menus : DefaultRequest
        {
            [DataMember]
            public int? menuId { get; set; }
            [DataMember]
            public string? menuName { get; set; }

            [DataMember]
            public int? levels { get; set; }
            [DataMember]
            public int? parentId { get; set; }
            [DataMember]
            public string? icon { get; set; }
            [DataMember]
            public bool? child { get; set; }
            [DataMember]
            public bool? enable { get; set; }
            [DataMember]
            public int? userGroupId { get; set; }
            [DataMember]
            public string? UserGroupName { get; set; }

        }
    }
}
