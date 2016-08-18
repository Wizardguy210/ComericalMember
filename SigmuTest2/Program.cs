using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Reflection;


namespace SigmuTest2
{
    class Program
    {
        private static object syncHandle = new object();
        static void Main(string[] args)
        {
            var db = new DW_CRMEntities();
            var data = db.F_CUST_ACCOUNT_CRM.ToList();
            var ct = new F_CUST_ACCOUNT_CRM();
            var transTime = db.F_SP_PROCESS.Where(x => x.SP_NAME == "F_CUST_ACCOUNT_CRM").Take(1);
            foreach (var t in transTime)
            {
                var c = Convert.ToDateTime(t.LAST_TRANS_VALUE);

                var g = data.Where(x => x.TRANS_DATE.Date >= c.Date);
                var creatememberInfo = new List<ContactModel.CreateMemberInfo>();
                LogWriter ct = new LogWriter("Test");
                var alreadyCount = 0;

                foreach (var c in data)
                {
                    var omemberRegServicePost = new ContactModel.memberRegServicePost();
                    var omemberRegServiceResponse = new ContactModel.memberRegServiceResponse();
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

                    var newUser = new ContactModel.CreateMemberInfo
                    {

                        regFrom = "aion",
                        regType = "db",
                        userName = c.CUST_NAME,

                        mainResidentialStateorprovince = c.COUNTY_NAME,
                        mainResidentialPostalCode = zip_code,
                        mainResidentialCity = c.TOWN_NAME,
                        mainResidentialLine1 = ADDR,
                        serviceType = service_type
                    };

                    if (isCellPhone)
                        newUser.mobilePhone = c.MOBILE_NO;
                    else
                        newUser.telNumber = c.MOBILE_NO;

                    creatememberInfo.Add(newUser);
                    ServicePointManager.DefaultConnectionLimit = 50000;

                    if (creatememberInfo.Count() == 50)
                    {
                        var json = JsonConvert.SerializeObject(creatememberInfo);
                        var Url = "http://sigmuecapi4dev.azurewebsites.net/api/CreateMember/";

                        retry:
                        try
                        {
                            HttpWebRequest request = HttpWebRequest.Create(Url) as HttpWebRequest;
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
                                request.Timeout = 20 * 60 * 1000000;
                                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                                {
                                    result = sr.ReadToEnd();
                                    sr.Close();
                                }
                                response.Close();
                            }

                            //List<ContactModel.ReturnMessage> Msg = null;
                            //result = result.Replace(@"\\\", @"\");
                            //Msg = JsonConvert.DeserializeObject < List<ContactModel.ReturnMessage>>(result);

                            Console.WriteLine(result);
                            alreadyCount += 50;
                            Console.WriteLine(result + alreadyCount);
                            ct.LogWrite(result + alreadyCount);
                            creatememberInfo.Clear();
                            Thread.Sleep(1000);
                        }
                        catch (WebException e)
                        {

                            goto retry;

                        }

                    }

                }
                //int total = data.Count();

                ////設定csv檔案位置
                //string strPath = "C:\\New\\Test3.csv";

                ////修改檔案為非唯讀屬性(Normal)
                //System.IO.FileInfo FileAttribute = new FileInfo(strPath);
                //FileAttribute.Attributes = FileAttributes.Normal;

                ////開啟CSV檔案
                //FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                //int sum = 0;


                ////var datas = db.F_CUST_ACCOUNT_CRM.Skip(sum).Take(50).ToList();

                //sw.WriteLine("姓氏,行動電話,地址 1: 縣/市,地址 1: 郵遞區號, 地址 1: 市/鎮, 地址 1: 郵遞區號,地址 1: 街道 1,會員編號");




                //#region 連接至科研新增會員API
                //try
                //{

                //    if (isCellPhone == true)
                //    {


                //        var responseData = "";

                //        omemberRegServicePost.userName = newUser.userName;

                //        omemberRegServicePost.regIdType = "telNo";
                //        omemberRegServicePost.regId = newUser.mobilePhone;



                //        omemberRegServicePost.regFrom = "microsoft";
                //        omemberRegServicePost.regService = "member";
                //        omemberRegServicePost.modelSel = "TRIAL";


                //        var memeberData = JsonConvert.SerializeObject(omemberRegServicePost);


                //        var httpWebRequest = HttpWebRequest.Create(" http://member.myvita.com.tw:8096/memberRegService.aspx");
                //        httpWebRequest.ContentType = "application/json";
                //        httpWebRequest.Method = "POST";
                //        httpWebRequest.Headers.Set("token", "B16E55A1A203236EED26EBD1E5AF8A0935EAC580AE583159B83232A6FB00C6AD0F1941F7F3562CFB5CEEE214F76EBA1540E879D988B2EA80B60DC945E221C18D14DB485E4C0D5557F3EEF7356D049D781E413F32EC90A747197B9054FF0E502E57E383FD8AD6C75BA1D845285F32416E7FAD470A5E220BF3");

                //        httpWebRequest.Proxy = null;
                //        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                //        {
                //            streamWriter.Write(memeberData);
                //            streamWriter.Flush();
                //        }
                //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //        {
                //            responseData = streamReader.ReadToEnd();
                //            omemberRegServiceResponse = JsonConvert.DeserializeObject<ContactModel.memberRegServiceResponse>(responseData);
                //            if (omemberRegServiceResponse.rs != "OK")
                //            {
                //                //throw new System.ArgumentException("科研API錯誤", "regId");

                //            }
                //        }
                //    }
                //    if (omemberRegServiceResponse.iAccount == null)
                //        omemberRegServiceResponse.iAccount = "";
                //    sw.WriteLine(newUser.userName + "," + newUser.mobilePhone + "," + newUser.mainResidentialStateorprovince + "," + newUser.mainResidentialPostalCode + "," + newUser.mainResidentialCity+","+newUser.mainResidentialLine1+","+ omemberRegServiceResponse.iAccount);
                //    Console.WriteLine(newUser.userName + "," + omemberRegServiceResponse.iAccount);
                //}
                //catch (Exception e)
                //{

                //    throw new System.ArgumentException("Please enter Data", @"regType");

                //}
                //#endregion





















            }


            //HttpClient client = new HttpClient();

            ////var responseBody = response.Result.Content.ReadAsStringAsync();
            ////var test=JsonConvert.DeserializeObject<List<ContactModel.ReturnMessage>>(responseBody.Result);
            ////var client = new HttpClient();
            //client.BaseAddress = new Uri("http://localhost:22049/");
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //StringContent content = new StringContent(json, Encoding.UTF8, "text/plain");
            //var response = client.PostAsync("api/CreateMember", content);

            //string responseBody = response.Result.Content.ReadAsStringAsync().Result;
            //dynamic jObj = (JObject)JsonConvert.DeserializeObject(responseBody);






            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
            //StringContent content = new StringContent(json, Encoding.UTF8, "text/json");

            //string JSON = "[{\"itemKey\":\"\",\"returnCode\":\"20\",\"returnMsg\":\"歸戶會員新增成功\"},{\"itemKey\":\"\",\"returnCode\":\"20\",\"returnMsg\":\"歸戶會員新增成功\"}]";
            //ContactModel.ReturnMessage[] Msg = null;
            //Msg = JsonConvert.DeserializeObject<ContactModel.ReturnMessage[]>(JSON.ToString());

            //var response = client.PostAsync("api/CreateMember", content).Result;
            //string responseBody = response.Content.ReadAsStringAsync().Result;

            //ContactModel.ReturnMessage[] ett = null;
            //ett = JsonConvert.DeserializeObject<ContactModel.ReturnMessage[]>(responseBody);


            //sw.Close();
        }
    }
    //static async Task RunAsync(string data)
    //{
    //    using (var client = new HttpClient())
    //    {
    //        // New code:
    //        client.BaseAddress = new Uri("http://localhost:22049/");
    //        client.DefaultRequestHeaders.Accept.Clear();
    //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    //        // Http Post
    //        HttpResponseMessage response = await client.PostAsync("api/CreateMember",);

    //    }
    //}
    public class LogWriter
    {
        private string m_exePath = string.Empty;
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }
        public void LogWrite(string logMessage)
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText("C:\\New\\Sigmulog.txt"))
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
}

