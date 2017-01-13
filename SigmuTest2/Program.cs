using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Security.Cryptography;
using System.Configuration;
namespace SyncSeomcMember
{
    class Program
    {
        private static object syncHandle = new object();
        static void Main(string[] args)
        {
            var db = new DW_CRMEntities();
            var data = db.F_CUST_ACCOUNT_CRM.ToList();
            var companyData = db.F_CUST_ACCOUNT_CRM_BM.ToList();

            List<int> termsList = new List<int>();
            var transTime = db.F_SP_PROCESS.Where(x => x.SP_NAME == "F_CUST_ACCOUNT_CRM").FirstOrDefault();
            termsList.Add(0900000000);
            termsList.Add(0939394191);
            termsList.Add(0910316240);
            termsList.Add(0912575180);
            termsList.Add(0939053712);
            termsList.Add(0920638686);
            termsList.Add(0963671535);
            termsList.Add(0989223251);
            termsList.Add(0928256036);
            termsList.Add(0972028957);
            termsList.Add(0938021033);
            termsList.Add(0932304630);
            termsList.Add(0912547880);
            termsList.Add(0952048047);
            termsList.Add(0972184398);
            termsList.Add(0922667444);
            termsList.Add(0968723628);
            termsList.Add(0955200306);
            termsList.Add(0936066659);
            termsList.Add(0988336799);
            termsList.Add(0928552082);
            termsList.Add(0978026662);
            termsList.Add(0921950182);
            termsList.Add(0920528325);
            termsList.Add(0932559875);
            termsList.Add(0932053223);
            termsList.Add(0932039869);
            termsList.Add(0979555850);
            termsList.Add(0922448448);
            termsList.Add(0939851518);
            termsList.Add(0958803776);
            termsList.Add(0935698301);
            termsList.Add(0910911285);
            termsList.Add(0917525268);
            termsList.Add(0989268323);
            termsList.Add(0932206838);
            termsList.Add(0955594696);
            termsList.Add(0913519991);
            termsList.Add(0932393929);
            termsList.Add(0956199517);
            termsList.Add(0933764909);
            termsList.Add(0915888322);
            termsList.Add(0937982061);
            termsList.Add(0987425116);
            termsList.Add(0935221381);
            termsList.Add(0937533023);
            termsList.Add(0922628628);
            termsList.Add(0986053022);
            termsList.Add(0910021520);
            termsList.Add(0919268622);
            termsList.Add(0955887786);
            termsList.Add(0933875111);
            termsList.Add(0960702511);
            termsList.Add(0932217927);
            termsList.Add(0932123184);
            termsList.Add(0935186635);
            termsList.Add(0987955075);
            termsList.Add(0972168538);
            termsList.Add(0966909538);
            termsList.Add(0920680602);
            termsList.Add(0920253655);
            termsList.Add(0932038432);
            termsList.Add(0910550006);
            termsList.Add(0958233196);
            termsList.Add(0931329375);
            termsList.Add(0919530790);
            termsList.Add(0922735660);
            termsList.Add(0982820114);
            termsList.Add(0925368507);
            termsList.Add(0917668340);
            termsList.Add(0955738758);
            termsList.Add(0987155262);
            termsList.Add(0939253150);
            termsList.Add(0970177530);
            termsList.Add(0938915607);
            termsList.Add(0958116187);
            termsList.Add(0937741548);
            termsList.Add(0930828387);



            var tempData = data.Where(c => termsList.Any(x => c.MOBILE_NO.Contains(x.ToString())));













            var CreateMemberUrl = ConfigurationSettings.AppSettings[@"CreateMemberAPI"];
            var first = ConfigurationSettings.AppSettings[@"first"];


            Console.WriteLine(CreateMemberUrl);

            var lastTransTime = Convert.ToDateTime(transTime.LAST_TRANS_VALUE);
            // 比較時間
            var g = data.Where(x => x.TRANS_DATE >= lastTransTime);
            var total = g.Count();
            var finalTotal = total % 50;
            var creatememberInfo = new List<ContactModel.CreateMemberInfo>();
            var creatComapnyList = new List<CreateAccountPost>();
            var log = new LogWriter("Test");
            var alreadyCount = 0;
            var alreadyCompanyCount = 0;
            Console.WriteLine("本次作業共有" + total + "筆");

            foreach (var c in tempData)
            {
                //var omemberRegServicePost = new ContactModel.memberRegServicePost();
                //var omemberRegServiceResponse = new ContactModel.memberRegServiceResponse();
                int zip_code, service_type;
                Int32.TryParse(c.ZIP_ID, out zip_code);
                if (zip_code == 0)
                    zip_code = 999;
                Int32.TryParse(c.SERVICE_TYPE, out service_type);
                var ADDR = c.CUST_ACC_ADDR;


                if (c.COUNTY_NAME != null)
                {

                    ADDR = ADDR.Replace(c.COUNTY_NAME, "");
                    if (c.COUNTY_NAME.Contains("臺"))
                    {
                        var tempCounty = c.COUNTY_NAME.Replace("臺", "台");
                        ADDR = ADDR.Replace(tempCounty, "");
                    }
                    if (c.COUNTY_NAME.Contains("台"))
                    {
                        var tempCounty = c.COUNTY_NAME.Replace("台", "臺");
                        ADDR = ADDR.Replace(tempCounty, "");
                    }

                }
                if (c.TOWN_NAME != null)
                    ADDR = ADDR.Replace(c.TOWN_NAME, "");
                ;


                var isCellPhone = System.Text.RegularExpressions.Regex.IsMatch(c.MOBILE_NO, @"(09)[0-9]{8}");

                var util = new EmailValidation();

                var newUser = new ContactModel.CreateMemberInfo
                {

                    regFrom = "aion",

                    // db為第一次同步 change 為之後同步之處理字串
                    regType = "db",
                    userName = c.CUST_NAME,

                    mainResidentialStateorprovince = c.COUNTY_NAME,
                    mainResidentialPostalCode = zip_code,
                    mainResidentialCity = c.TOWN_NAME,

                    mainResidentialLine1 = ADDR,
                    serviceType = service_type,


                };
                if (util.IsValidEmail(c.EMAIL))
                    newUser.email = c.EMAIL;

                if (first == "true")
                    newUser.fisrtSend = true;
                else
                    newUser.fisrtSend = false;

                if (isCellPhone)
                {
                    newUser.mobilePhone = c.MOBILE_NO;
                    //取得將原始字串加鹽後轉成UTF8 Byte[]輸出
                    var buffer = Encoding.UTF8.GetBytes(String.Concat("CreateMember", c.MOBILE_NO));
                    //使用SHA1加密
                    var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
                    //將Hash後的結果移除"-"輸出
                    var genCheckCode = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
                    newUser.checkCode = genCheckCode;
                }
                else
                {
                    newUser.telNumber = c.MOBILE_NO;
                    //取得將原始字串加鹽後轉成UTF8 Byte[]輸出
                    var buffer = Encoding.UTF8.GetBytes(String.Concat("CreateMember", ""));
                    //使用SHA1加密
                    var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
                    //將Hash後的結果移除"-"輸出
                    var genCheckCode = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
                    newUser.checkCode = genCheckCode;


                }
                creatememberInfo.Add(newUser);
                total = total - 1;
                ServicePointManager.DefaultConnectionLimit = 50000;
                if (first == "true")
                {
                    if (creatememberInfo.Count() == 50)
                    {
                        var json = JsonConvert.SerializeObject(creatememberInfo);


                        retry:
                        try
                        {
                            HttpWebRequest request = HttpWebRequest.Create(CreateMemberUrl) as HttpWebRequest;
                            string result = null;
                            request.Method = "POST";    // 方法
                            request.KeepAlive = true; //是否保持連線
                            request.ContentType = "text/json";


                            byte[] bs = Encoding.UTF8.GetBytes(json);

                            using (Stream reqStream = request.GetRequestStream())
                            {
                                reqStream.Write(bs, 0, bs.Length);
                            }

                            using (WebResponse response = request.GetResponse())
                            {
                                request.Timeout = int.MaxValue;
                                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                                {
                                    result = sr.ReadToEnd();
                                    sr.Close();
                                }
                                response.Close();
                            }
                            Console.WriteLine(result);
                            alreadyCount += 50;
                            Console.WriteLine(result + alreadyCount);
                            log.LogWrite(result + alreadyCount);
                            creatememberInfo.Clear();
                            Thread.Sleep(1000);

                        }
                        catch (WebException e)
                        {

                            goto retry;

                        }
                    }

                }
                else
                {
                    var json = JsonConvert.SerializeObject(creatememberInfo);
                    try
                    {
                        HttpWebRequest request = HttpWebRequest.Create(CreateMemberUrl) as HttpWebRequest;
                        string result = null;
                        request.Method = "POST";    // 方法
                        request.KeepAlive = true; //是否保持連線
                        request.ContentType = "text/json";


                        byte[] bs = Encoding.UTF8.GetBytes(json);

                        using (Stream reqStream = request.GetRequestStream())
                        {
                            reqStream.Write(bs, 0, bs.Length);
                        }

                        using (WebResponse response = request.GetResponse())
                        {
                            request.Timeout = int.MaxValue;
                            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                            {
                                result = sr.ReadToEnd();
                                sr.Close();
                            }
                            response.Close();
                        }

                        Console.WriteLine(result + total);
                        log.LogWrite(result + alreadyCount);
                        creatememberInfo.Clear();
                        Thread.Sleep(1000);

                    }
                    catch (WebException e)
                    {

                        Console.WriteLine(e.ToString());

                    }
                }


            }


            //foreach (var company in companyData)
            //{

            //    var AccountAPIUrl = ConfigurationSettings.AppSettings[@"CreateAccountAPI"];


            //    var newCompany = new CreateAccountPost
            //    {
            //        billingAmount = Convert.ToInt32(company.BILLING_AMT.Value),
            //        companyName = company.CUST_ACC_NAME,
            //        companyId = company.CUST_ACC_IDNO,
            //        mainResidentialLine1 = company.CUST_ACC_ADDR
            //    };




            //    //取得將原始字串加密後轉成UTF8 Byte[]輸出
            //    var buffer = Encoding.UTF8.GetBytes(String.Concat("CreateAccountPost", company.CUST_ACC_IDNO));
            //    //使用SHA1加密
            //    var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            //    //將Hash後的結果移除"-"輸出
            //    var genCheckCode = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
            //    newCompany.checkCode = genCheckCode;


            //    creatComapnyList.Add(newCompany);
            //    //total = total - 1;
            //    ServicePointManager.DefaultConnectionLimit = 50000;

            //    if (creatComapnyList.Count() == 50)
            //    {
            //        var json = JsonConvert.SerializeObject(creatComapnyList);


            //        retry:
            //        try
            //        {
            //            HttpWebRequest request = HttpWebRequest.Create(AccountAPIUrl) as HttpWebRequest;
            //            string result = null;
            //            request.Method = "POST";    // 方法
            //            request.KeepAlive = true; //是否保持連線
            //            request.ContentType = "text/json";


            //            byte[] bs = Encoding.UTF8.GetBytes(json);

            //            using (Stream reqStream = request.GetRequestStream())
            //            {
            //                reqStream.Write(bs, 0, bs.Length);
            //            }

            //            using (WebResponse response = request.GetResponse())
            //            {
            //                request.Timeout = int.MaxValue;
            //                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            //                {
            //                    result = sr.ReadToEnd();
            //                    sr.Close();
            //                }
            //                response.Close();
            //            }
            //            Console.WriteLine(result);
            //            alreadyCompanyCount += 50;
            //            Console.WriteLine(result + alreadyCompanyCount);
            //            log.LogWrite(result + alreadyCompanyCount);
            //            creatComapnyList.Clear();
            //            Thread.Sleep(1000);

            //        }
            //        catch (WebException e)
            //        {

            //            goto retry;

            //        }
            //    }



                //var json = JsonConvert.SerializeObject(creatememberInfo);
                //try
                //{
                //    HttpWebRequest request = HttpWebRequest.Create(Url) as HttpWebRequest;
                //    string result = null;
                //    request.Method = "POST";    // 方法
                //    request.KeepAlive = true; //是否保持連線
                //    request.ContentType = "text/json";


                //    byte[] bs = Encoding.UTF8.GetBytes(json);

                //    using (Stream reqStream = request.GetRequestStream())
                //    {
                //        reqStream.Write(bs, 0, bs.Length);
                //    }

                //    using (WebResponse response = request.GetResponse())
                //    {
                //        request.Timeout = int.MaxValue;
                //        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                //        {
                //            result = sr.ReadToEnd();
                //            sr.Close();
                //        }
                //        response.Close();
                //    }

                //    //Console.WriteLine(result + total);
                //    log.LogWrite(result + alreadyCount);
                //    creatememberInfo.Clear();
                //    Thread.Sleep(1000);

                //}
                //catch (WebException e)
                //{

                //    Console.WriteLine(e.ToString());

                //}



            //}








            #region 更新時間
            var todayTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            db.Database.ExecuteSqlCommand(@"UPDATE [F_SP_PROCESS] set [LAST_TRANS_VALUE]='" + todayTime + "' where [SP_NAME]='F_CUST_ACCOUNT_CRM'");
            db.SaveChanges();
            #endregion

        }

    }

}

public class LogWriter
{
    private string m_exePath = string.Empty;

    public LogWriter(string logMessage)
    {
        LogWrite(logMessage);
    }
    public void LogWrite(string logMessage)
    {
        var logLocation = ConfigurationSettings.AppSettings[@"logLocation"];
        m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        try
        {
            using (StreamWriter w = File.AppendText(logLocation + "secomMemberLog.txt"))
            {
                Log(logMessage, w);
            }
        }
        catch (Exception ex)
        {
        }
    }

    public void Log(string logMessage, TextWriter txtWriter)
    {
        try
        {
            txtWriter.Write("\r\nLog Entry : ");
            txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            txtWriter.WriteLine("  :");
            txtWriter.WriteLine("  :{0}", logMessage);
            txtWriter.WriteLine("-------------------------------");
        }
        catch (Exception ex)
        {
        }
    }
}


