using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ExpressiveAnnotations.Attributes;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace SigmuTest2
{
    class ContactModel
    {
        public class CreateMemberInfo
        {


            [Required(ErrorMessage = "Please enter the regFrom.")]
            [StringLength(10, ErrorMessage = "regFrom最長10碼")]
            public string regFrom { get; set; }
            /// <summary>
            /// 來源通路
            /// </summary>
            /// <value>
            /// Plase enter 'fb' or 'mobilePhone'
            /// </value>
            [RequiredIf(@"regFrom=='ec'||regFrom=='aion'")]
            [Required]
            [StringLength(50, ErrorMessage = "regType最長50碼")]
            public string regType { get; set; }

            /// <summary>
            /// 手機號碼
            /// </summary>
            /// <value>
            /// The mobile phone.共十碼
            /// </value>
            [RequiredIf(@"regFrom =='ec'", ErrorMessage = "請輸入行動電話號碼")]
            public string mobilePhone { get; set; }

            /// <summary>
            /// 使用者姓名
            /// </summary>
            /// <value>
            /// The full name.
            /// </value>
            [Required(ErrorMessage = "Please enter the userName.")]
            [StringLength(50, ErrorMessage = "fullName最長50碼")]
            public string userName { get; set; }
            /// <summary>
            ///  email.
            /// </summary>
            /// <value>
            /// The email.
            /// </value>
            [RequiredIf(@"regFrom=='ec'", ErrorMessage = "請輸入電子信箱")]
            [StringLength(128, ErrorMessage = "請勿輸入超過 128 個字")]
            [EmailAddress(ErrorMessage = "請輸入正確電子郵件信箱")]
            public string email { get; set; }

            public string telNumber { get; set; }


            /// <summary>
            /// 居住地址: 郵遞區號
            /// </summary>
            /// <value>
            /// The main residential postal code. ex:114
            /// </value>
            [Range(100, 999, ErrorMessage = "PostalCode must be between 100 and 999")]
            public int? mainResidentialPostalCode { get; set; }
            /// <summary>
            /// 居住地址: 縣/市
            /// </summary>
            /// <value>
            /// The main residential stateorprovince.
            /// </value>
            [StringLength(4, ErrorMessage = "mainResidentialPostalCode最長4碼")]
            public string mainResidentialStateorprovince { get; set; }
            /// <summary>
            /// 居住地址:市/鎮
            /// </summary>
            /// <value>
            /// The main residential city.
            /// </value>
            [StringLength(4, ErrorMessage = "mainResidentialCity最長4碼")]
            public string mainResidentialCity { get; set; }
            /// <summary>
            /// 居住地址:街道 1
            /// </summary>
            /// <value>
            /// The main residential line1.
            /// </value>
            [StringLength(50, ErrorMessage = "mainResidentialLine1最長50碼")]
            public string mainResidentialLine1 { get; set; }
            /// <summary>
            ///  會員等級
            /// </summary>
            /// <value>
            ///  0-->會員未註冊服務,1-->會員已註冊服務
            /// </value>
            //[RequiredIf(@"regFrom=='aion'")]
            [Range(0, 1)]
            public int serviceType { get; set; }
            /// <summary>
            /// 檢核碼
            /// </summary>
            public string checkCode { get; set; }

        }


        /// <summary>
        /// 科研會員PostModel
        /// </summary>
        public class memberRegServicePost
        {
            [Required(ErrorMessage = "Please enter the userName.")]
            public string userName { get; set; }
            [Required]
            public string regIdType { get; set; }
            [RequiredIf(@"regIdType =='cellPhone'")]
            public string regId { get; set; }
            [DefaultValue("")]
            [JsonProperty(PropertyName = "sex", DefaultValueHandling = DefaultValueHandling.Populate)]

            public int sex { get; set; }
            public string regFrom { get; set; }
            [AssertThat("birthDay >= Today()")]
            public DateTime birthDay { get; set; }
            public string modelSel { get; set; }
            public string regService { get; set; }


        }
        /// <summary>
        /// 科研會員ResponseModel
        /// </summary>
        public class memberRegServiceResponse
        {
            public string rs { get; set; }
            public string iAccount { get; set; }
            public string regService { get; set; }
            public string dataType { get; set; }

        }

        public class ReturnMessage
        {
            /// <summary>
            /// 回傳處理項目的組建資訊。
            /// </summary>
            public string itemKey { get; set; }
            /// <summary>
            /// 傳回代碼[char(2)]
            /// </summary>
            public string returnCode { get; set; }

            /// <summary>
            /// 傳回訊息[nvarchar(256)]
            /// </summary>
            public string returnMsg { get; set; }
        }
        [DataContract]
        public class ReturnMessageList
        {
            public ReturnMessage[] contactList { get; set; }
        }
    }
}
