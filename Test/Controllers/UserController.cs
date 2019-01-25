using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;
using Test.Helper;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Test.Controllers
{
    public class UserController : Controller
    {

        public const string constr = @"Data Source=.;Initial Catalog=UserInfo;User ID=dongcheng;Password=Aa336699";
        public static List<User> regionUserInfoList { get; set; }
        HttpClient client = new HttpClient();
        
        // GET: User
        #region Index
        public async Task<ActionResult> Index()//Index
        {
            List<User> userInfoList = new List<User>();
            
            var userInfoListCacheKey = "cache_2018-12-28_userinfoList";

            if (CacheHelper.GetCache(userInfoListCacheKey) != null)
            {
                userInfoList = (List<User>)CacheHelper.GetCache(userInfoListCacheKey);
            }
            else
            {
                client.BaseAddress = new Uri("http://10.194.46.143:5533/api/values/getall");
                var response = await client.GetStringAsync(client.BaseAddress);
                userInfoList = JsonConvert.DeserializeObject<List<User>>(response);
            }

            regionUserInfoList = userInfoList;
        
            return View(userInfoList);
        } 
        #endregion

        #region AddInfo
        public ActionResult AddInfo()
        {

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddInfo1(string UserName,string UserSex,int UserAge)
        {
            client.BaseAddress = new Uri("http://10.194.46.143:5533/api/values/add");
           
            Para para = new Para
            {
                Name = UserName,
                Sex = UserSex,
                Age = UserAge
            };
            var temp = JsonConvert.SerializeObject(para);
            var response = await client.PostAsJsonAsync(client.BaseAddress, para);
            return View();
        }
        #endregion

        
        #region DelInfo
        //[HttpPost]
        public async Task<ActionResult> DelInfo1(int id)
        {
            client.BaseAddress = new Uri("http://10.194.46.143:5533/api/values/delete");

            var response = await client.PostAsJsonAsync(client.BaseAddress,id);

            return View();
        }
        #endregion

        #region UpdateInfo
        public ActionResult UpdateInfo(int id)
        {
            User user = new User();

            if (regionUserInfoList.Count > 0)
            {
                user = regionUserInfoList.Where(x => x.Id == id).FirstOrDefault();
            }
            else
            {
                //if region user information list is null, then get the user from database
            }

            return View(user);
        }
        //[HttpPost]
        public async Task<ActionResult> UpdateInfo1(int id,string UserName,string UserSex,int UserAge)
        {
            client.BaseAddress = new Uri("http://10.194.46.143:5533/api/values/update");
            Para para = new Para
            {
                Id = id,
                Name = UserName,
                Sex = UserSex,
                Age = UserAge

            };
            //将Post请求发送给指定URI
            var response = await client.PostAsJsonAsync(client.BaseAddress,para);
            return View();
        }
        #endregion

        #region SelInfo
        [HttpGet]
        public async Task<ActionResult> SelInfo(string UserName)
        {
            client.BaseAddress = new Uri("http://10.194.46.143:5533/api/values/get?name="+UserName);
            //await异步等待回应
            var response = await client.GetStringAsync(client.BaseAddress);

            List<User> list = new List<User>();

            list = JsonConvert.DeserializeObject<List<User>>(response);

            return View(list);
        }

        #endregion
        public static void ToXml(DataTable dt)
        {
            dt.WriteXml(@"D:\abk.xml");//以xml形式写入DataTable中的内容并保存在ck.xml文件中
        }

        public static void ReadXmlByDataSet()
        {
            DataSet ds = new DataSet();
        }
    }

}