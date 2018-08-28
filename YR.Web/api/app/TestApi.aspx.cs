using Asiasofti.SmartVehicle.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Asiasofti.SmartVehicle.Manager;
using System.Data;
using Asiasofti.SmartVehicle.Common.Enum;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using YR.Common.DotNetData;
using Newtonsoft.Json.Converters;
using YR.Common.DotNetCache;
using System.Diagnostics;
using System.Threading;
using YR.Common.SystemInfo;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app
{

    public partial class TestApi : System.Web.UI.Page
    {
        private string apiUrl = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            /*ICache cache = CacheFactory.GetCache();
            int count = 1;
            string overSpeedKey = "over_speed_" + "123456";
            string cacheValue = "";
            cacheValue = cache.Get<string>(overSpeedKey);
            if (string.IsNullOrEmpty(cacheValue))
            {
                DateTime dt = DateTime.Now.AddMinutes(5);
                cache.Set(overSpeedKey, count + "," + dt.ToString("yyyy-MM-dd HH:mm:ss"), dt - DateTime.Now);
            }
            else
            {
                if (cacheValue.IndexOf(",") > 0)
                {
                    string[] countValue = cacheValue.Split(',');
                    if (!string.IsNullOrEmpty(countValue[0]) && !string.IsNullOrEmpty(countValue[1]))
                    {
                        int.TryParse(countValue[0], out count);
                        count += 1;
                        if (count >= 10)
                        {

                        }
                        DateTime lastTime = Convert.ToDateTime(countValue[1]);
                        TimeSpan timeSpan = lastTime - DateTime.Now;
                        if (timeSpan.Seconds > 1)
                        {
                            cache.Set(overSpeedKey, count + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), timeSpan);
                        }
                    }
                }
                else
                {
                    DateTime dt = DateTime.Now.AddMinutes(5);
                    cache.Set(overSpeedKey, count + "," + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), dt - DateTime.Now);
                }
            }
            cache.Dispose();*

            /*VehicleManager vm = new VehicleManager();
            bool isSuccess = vm.OpenVehicle("5f11db4f-7bab-4bf4-b546-7892141aecd6");
            Thread.Sleep(2000);
            isSuccess = vm.CloseVehicle("5f11db4f-7bab-4bf4-b546-7892141aecd6");
            */

            /*AlipayOrderQuery orderQuery = new AlipayOrderQuery();
            OrderQueryResult queryResult = orderQuery.QueryByTradeNO("2017091821001004820206358656");
            string buyer_id = queryResult.buyer_user_id;*/

            /*
            SysSettingManager settingManager = new SysSettingManager();
            string outServiceAreaFee = settingManager.GetValueByKey("OutServiceAreaFee");
            string s = string.Format("超出运营区域将加收{0}元调度费", outServiceAreaFee.Substring(1, outServiceAreaFee.Length - 1));

            decimal f = 5.00000000000m;
            int ff = (int)f;
            int fff = Convert.ToInt32(f);*/

            /*
            //获取当前进程对象
            Process cur = Process.GetCurrentProcess();

            PerformanceCounter curpcp = new PerformanceCounter("Process", "Working Set - Private", cur.ProcessName);
            PerformanceCounter curpc = new PerformanceCounter("Process", "Working Set", cur.ProcessName);
            PerformanceCounter curtime = new PerformanceCounter("Process", "% Processor Time", cur.ProcessName);

            //上次记录CPU的时间
            TimeSpan prevCpuTime = TimeSpan.Zero;
            //Sleep的时间间隔
            int interval = 1000;

            PerformanceCounter totalcpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            SystemInfo sys = new SystemInfo();
            const int MB_DIV = 1024 * 1024;
            const int GB_DIV = 1024 * 1024 * 1024;
            //while (true){
            //第一种方法计算CPU使用率
            //当前时间
            TimeSpan curCpuTime = cur.TotalProcessorTime;
            //计算
            double value = (curCpuTime - prevCpuTime).TotalMilliseconds / interval / Environment.ProcessorCount * 100;
            prevCpuTime = curCpuTime;

            string method1 = string.Format("{0}:{1}  {2:N}KB CPU使用率：{3}", cur.ProcessName, "工作集(进程类)", cur.WorkingSet64 / 1024, value);//这个工作集只是在一开始初始化，后期不变
            string method11 = string.Format("{0}:{1}  {2:N}KB CPU使用率：{3}", cur.ProcessName, "工作集        ", curpc.NextValue() / 1024, value);//这个工作集是动态更新的
                                                                                                                                            //第二种计算CPU使用率的方法
            string method2 = string.Format("{0}:{1}  {2:N}KB CPU使用率：{3}%", cur.ProcessName, "私有工作集    ", curpcp.NextValue() / 1024, curtime.NextValue() / Environment.ProcessorCount);
            Thread.Sleep(interval);

            //第一种方法获取系统CPU使用情况
            string method12 = string.Format("\r系统CPU使用率：{0}%", totalcpu.NextValue());
            //Thread.Sleep(interval);

            //第二章方法获取系统CPU和内存使用情况
            string method13 = string.Format("\r系统CPU使用率：{0}%，系统内存使用大小：{1}MB({2}GB)", sys.CpuLoad, (sys.PhysicalMemory - sys.MemoryAvailable) / MB_DIV, (sys.PhysicalMemory - sys.MemoryAvailable) / (double)GB_DIV);
            //Thread.Sleep(interval);
            //}
            */



            /*
            VehicleManager vm = new VehicleManager();
            Hashtable vehicle_ht = vm.GetVehicleInfoByGPSNum("865067026608649");
            DateTime lastUpdateTime;
            int timestamp = 0;
            if (vehicle_ht != null && vehicle_ht.Keys.Count > 0)
            {
                string strLastUpdateTime = SiteHelper.GetHashTableValueByKey(vehicle_ht, "LastUpdateTime");
                if (!string.IsNullOrEmpty(strLastUpdateTime))
                {


                    lastUpdateTime = DateTime.Parse(SiteHelper.GetHashTableValueByKey(vehicle_ht, "LastUpdateTime"));
                    timestamp = SiteHelper.ExecDateDiffSecond(lastUpdateTime, DateTime.Now);
                }


            }
            string vehicle = string.Empty;
            using (ICache Cache = CacheFactory.GetCache())
            {
                string keyid = "vehicle_865067026648629";
                vehicle = Cache.Get<String>(keyid);
            }*/
            //批量开关车
            /*string arr = "0793C612-BD4E-4516-8B70-E548EB69DAD5,083A5657-A035-4D69-8C0C-2D0009428D3B,11C8DF31-0155-4A05-987F-72B730943A49,13A20A27-2F01-47F7-AD8C-8098B7BF8049,15441899-D8AC-4B22-BED0-DDC100CAB85E,197d7c09-901b-479f-bfa1-ec62e7d5743a,1d533ace-5b61-4920-ad90-e7c3f96fa84c,1D8FA7E6-5A1B-4F8D-914A-3EB54CFBE54B,21802EE5-B032-4B89-89B4-06F0B9B39189,2226730C-521D-478A-AF59-D5B5AF8404E6,2a7c9619-b780-41d0-aff2-7d9e4f5a5045,2D138553-52E9-4E70-8105-5429C8060AB5,2d2777d9-ae58-4799-8602-5856d4e44ba7,2f8c19a2-9b5c-4423-a6dc-8ec5164b756f,36BEE5E5-BCE1-48A0-BE99-68A38F2867CA,3EA450C0-C6EC-43D6-B53F-C070D463D8B2,41416D3A-D6CC-4924-911F-927B53131484,43A0B845-D686-4278-85AC-186DF6A08A2B,44c77d24-bdc8-4767-8728-068bf01ab8d6,4527DDF1-0AEB-4CCC-8EA2-DA0F0C8B47B2,453952B7-DA3F-4281-8AB5-AD0D8C09D60E,45a11711-fdad-43f0-9425-95a4fc1b8ac2,4601A83E-17EF-4263-A670-DC25F0A78E94,48F7201D-042B-4A0B-8720-AEFCDC0F12FA,5124ff96-d9d1-4318-8a48-d6c94380adee,51406ef7-b78a-460c-b7e7-84be1c7bde60,51b1b3eb-912b-4b14-9230-ae3ecd10cc7b,568ba0ff-ccc7-4468-9efb-c5f39c39b18e,574cf23b-3302-4605-8ffe-e17f31d0eb3f,58bcf7f8-af93-42da-9d45-db1bc222e1dd,5C4C15E7-9F96-46D7-B76E-F1CAF1B3FE3B,5C5174B1-0097-47AF-A186-5A1E1D98FEB6,5E2A77A7-5F1F-40C1-9CDA-EDAC2199F529,60EADACA-A07D-4F4F-9488-38948E5202DB,63217f45-7a13-4f69-bb18-96510565fb20,6334ec15-9a08-455b-8b97-3229440130db,64328625-618A-488D-AF58-5193D4A84556,6460f473-2f03-4d12-980b-1f81bcff6f37,64E9B3F0-3126-4199-ACCC-48450F6EBB2D,68610F68-A290-49FA-A7A0-06A9DC2B5AC8,6A1B0AE0-5CB0-489E-8F76-44FF944127E2,6A77AF45-14BF-4ED6-BDB7-CDBC3D313C04,6B2BF971-8C94-408B-A265-DECE8914EDA1,6D50BD7C-2578-41AF-BD93-75E6F5060F92,71558F85-8D4E-4AF0-81B2-0E23F73629DF,7235C37D-BDE5-4B38-96F8-CCBD5B7F0195,72A4ABBA-CB9C-4C18-9BF0-06885C82353C,75FE9079-6762-47AE-8A08-504E98AA88D4,79F1BED2-A5FD-44B9-A417-F6B3139AA2DC,7CF90F85-68E7-4186-A56D-5DBCC15550E8,7F0C520D-2C3C-49D7-AF34-3AACC8EF2904,7FF9815E-7AB2-4639-ACE6-312F956F4067,84A99B69-1763-46BB-B346-03A931938DF5,8687d38d-4849-4604-9a72-3d0aa9a62961,8C94AEE2-E551-4AC7-BD83-4C1B5A83A28F,8CE92874-6CB1-49DA-9FF1-8EB4D4DB25EE,8D589459-85FC-42BE-8056-48194BD016AF,92744898-24B8-46FC-946F-F6FAE71D24F2,93C847C3-367E-479E-A278-E9227241C02E,993867E6-6961-421D-B6F3-1D162A99C63C,99C8BD52-F2CD-4F92-8AD3-988D177281F4,A276A7BC-624D-43F5-B775-BFD2D05BCF36,A9022D64-F6AE-4AE9-8206-7489FB25EE44,A907C498-8AE9-46FA-A6A1-9DD2E81BCC48,AA9126FD-7A0B-4254-A63D-EC3F16527DB6,AB6D75FA-CB77-4EC6-80E7-58328453F90D,ABB854D9-3BB9-4255-A650-F09A28A17169,AC811F06-C901-4C25-8333-62936E0BC869,ADA38EDA-33F3-414E-875D-D713ECEF5D73,B1B9E159-89E7-4903-A717-D9A4E1EF089F,BB970729-BDA3-423C-B6A9-1A8EFC190C21,bbccb0b4-335b-4269-a9d2-710a195294c6,c4871126-b055-4f21-b0cb-801b26cbccc8,D1CCFAAE-806E-477D-97EB-2063EA374421,D2071BD4-3095-4246-8C82-4E9BB50B98C2,D2A84FDF-F0AB-47A2-9579-1BE7736AC036,D399951E-C80E-4D87-91BC-5B5CC24C2838,D6A5560A-CB3D-459C-92E9-CCBA2BACAD49,D79A468F-6D05-451D-B64B-59917604FE52,DF16FC87-1AF8-4930-BCF6-7D1202B4A3E8,DF88B0FD-28F4-4A21-9CD4-CE5C9B059906,DFD97212-EF11-4108-B236-B53A84204CBE,DFDA9C19-28E3-4E19-A2A3-860BD0FE6395,E0F6502C-EC4B-426D-9892-C0EFB8723361,e1f6d0f0-ced7-4685-b7cf-2d0acfa492c4,e70021db-ef9b-412d-93fc-137c031a7739,E700E013-544E-412B-BC07-B883E3FD771E,E7DF14CF-D965-4E8B-A9D8-EE9BBB7A74EF,e81c8c72-861b-491f-9779-8d0ac920ec24,E8750B71-F1F4-4B57-9C72-6DB2362961C5,ECAC40E4-2B0F-4BCB-8384-CA4DAC6B990D,ed6dddd7-202b-49fe-bc03-5696dfcee5b7,F180A548-F864-415D-8EF1-16E890B246AA,F540D4E9-08F3-4538-ADB6-53A194655603,F690AC0A-7955-4714-85EA-78E0EC938009,FA3FAEEE-5362-4858-B56C-28AEB8635960,FBC9589B-1664-43D2-81CC-A26333251CB9,FDFEA86C-F19F-42CD-A5D1-DD2FC6F86680";
            VehicleManager vm = new VehicleManager();
            foreach (string vid in arr.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                vm.OpenVehicle(vid);
                //vm.CloseVehicle(vid);
            }*/

            apiUrl = string.Format("http://{0}:{1}", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.methodTxt.Text))
            {
                this.resultLit.Text = "调用方法未填写，请填写后重试!";
                return;
            }
            if (string.IsNullOrEmpty(this.argsTxt.Text))
            {
                this.resultLit.Text = "调用参数未填写，请填写后重试!";
                return;
            }

            string client = ClientList.SelectedValue;
            string data = "type=" + this.methodTxt.Text + "&Client=" + client + "&" + this.argsTxt.Text;
            data = SiteHelper.Encrypt(data, "qazwsxedcrfvtgbyhnujmikoyhbgtrew").Replace("+", ",");
            HttpWebRequest webrequest = WebRequest.Create(string.Format("{0}/api/app/RequestControler.ashx?data={1}", apiUrl, data)) as HttpWebRequest;
            WebResponse wr = webrequest.GetResponse();
            System.IO.Stream stream = wr.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string result = sr.ReadToEnd();
            result = SiteHelper.Decrypt(result, "qazwsxedcrfvtgbyhnujmikoyhbgtrew");

            this.resultLit.Text = result;
        }

    }
}