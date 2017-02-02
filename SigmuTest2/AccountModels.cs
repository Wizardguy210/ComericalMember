using System.ComponentModel.DataAnnotations;

namespace SyncSecomMember
{
    public class CreateAccountPost
    {
        /// <summary>
        /// 公司名稱
        /// </summary>
        /// <value>
        /// The name of the company.
        /// </value>
        public string companyName { get; set; }
        /// <summary>
        /// 公司統一編號
        /// </summary>
        /// <value>
        /// The company identifier.
        /// </value>
        public string companyId { get; set; }

        /// <summary>
        /// 公司等級
        /// </summary>
        /// <value>
        /// The company level.
        /// </value>
        public int billingAmount { get; set; }


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
        /// 檢核碼
        /// </summary>
        public string checkCode { get; set; }
    }
}
