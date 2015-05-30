using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinaWeiboCore.Com.UnfreezeCore;

namespace SinaWeiboCoreUnitTest
{
    [TestClass]
    public class TestAccountExceptionHandling
    {
        /*
       Yunma
      demo001
      123456vim6

      Yzm1
      leodaping
      123456yzm1
       */

        private const string account = "tvjfix@ninth.keyandvalue.com";
        private const string password = "nQQ8NgoNTa";

        private const string receive_sms_name = "Yunma";
        private const string receive_sms_account = "demo001";
        private const string receive_sms_password = "123456vim6";

        private const string send_sms_name = "Yzm1";
        private const string send_sms_account = "leodaping";
        private const string send_sms_password = "123456yzm1";

        private const string type = "small_weibo";

        /// <summary>
        /// jiefeng
        /// </summary>
        [TestMethod]
        public void Test_jiefeng()
        {

            AccountExceptionHandling sinaweibo = new AccountExceptionHandling();
            object[] res = sinaweibo.jiefeng
                (account, password, receive_sms_name,
                receive_sms_account, receive_sms_password, send_sms_name,
                send_sms_account, send_sms_password, type);

            Console.WriteLine(string.Join("|", res));

            AccountExceptionHandling AccountExceptionHandling = new AccountExceptionHandling();
            AccountExceptionHandling.jiefeng("qwkpvl@after.taechk.com", "RainF3N3mU", "Yunma", "demo001", "123456", "Yzm1", "Yunma", "demo001", "123456");

        }//jiefeng


    }
}
