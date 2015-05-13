namespace SinaWeiboCore.Com.UnfreezeCore
{
    interface ISmsapi
    {
        string GetMobile(string type);

        string Unlock(string mobile, string type);

        void CancelSmsRecvAll(int count);

        void AddIgnoreList(string mobile, string type);

        string SendSms(string mobile, string type, string content);

        string GetSmsStatus(string mobile, string type);
    }
}
