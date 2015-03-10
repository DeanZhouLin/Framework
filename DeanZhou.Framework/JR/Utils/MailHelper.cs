using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;

namespace JFx.Utils
{
    /// <summary>
    /// 发送邮件辅助类
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// 初始化MailHelper对象
        /// <para>例如：</para>
        /// <para>MailHelper mail= new MailHelper("smtp.jinri.cn", "sendMail@jinri.cn", "password");</para>
        /// <para>IList&lt;Attachment&gt; list = new List&lt;Attachment&gt;();</para>
        /// <para>list.Add(MailHelper.CreateAttachment("ZipFile\\test.txt"));</para>
        /// <para>mail.SendMail("tonghangzhou@jinri.cn", "邮件发送", "请查收！","",list);</para>
        /// </summary>
        /// <param name="sHost">邮件服务器地址</param>
        /// <param name="formMail">发送邮件账户地址</param>
        /// <param name="strPassword">账户密码</param>
        public MailHelper(string sHost, string formMail, string strPassword)
        {
            m_Host = sHost;
            m_Form = formMail;
            m_Password = strPassword;
        }

        #region Private Property
        string m_Host;
        string m_Form;
        /// <summary>
        /// 发送邮件的账户密码
        /// </summary>
        string m_Password;
        MailPriority priority = MailPriority.Normal;
        #endregion

        #region Public Property
        /// <summary>
        /// STMP服务器地址
        /// </summary>
        public string Host
        {
            get { return m_Host; }
            set { m_Host = value; }
        }

        /// <summary>
        /// 发送邮件的邮箱地址
        /// </summary>
        public string Form
        {
            get { return m_Form; }
            set { m_Form = value; }
        }

        /// <summary>
        /// 邮件优先级
        /// </summary>
        public MailPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }
        #endregion

        #region SendMail
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sendTo">收件人地址</param>
        /// <param name="title">标题</param>
        /// <param name="content">邮件内容</param>
        /// <param name="strCC">抄送人，多个地址用","分隔</param>
        /// <param name="attachments">附件集合</param>
        /// <returns>是否成功</returns>
        public bool SendMail(string sendTo, string title, string content, string strCC = "", IList<Attachment> attachments = null)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Host = Host; ;
            smtpClient.Credentials = new System.Net.NetworkCredential(Form, m_Password);

            using (MailMessage mailMessage = new MailMessage(Form, sendTo))
            {
                mailMessage.Subject = title;
                mailMessage.Body = content;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = priority;

                #region CC
                if (!string.IsNullOrEmpty(strCC))
                {
                    string[] mailList = strCC.Split(',');
                    foreach (string str in mailList)
                    {
                        mailMessage.CC.Add(str);
                    }
                }
                #endregion

                #region Attachments
                if (attachments != null && attachments.Count > 0)
                {
                    foreach (Attachment att in attachments)
                    {
                        mailMessage.Attachments.Add(att);
                    }
                }
                #endregion

                try
                {
                    smtpClient.Send(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region CreateAttachment
        /// <summary>
        /// 创建附件
        /// 注意：如果附件太大，可能需要等待的时候会很长
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns></returns>
        public static Attachment CreateAttachment(string filePath)
        {
            Attachment attachment = new Attachment(filePath, MediaTypeNames.Application.Octet);
            ContentDisposition disposition = attachment.ContentDisposition;
            disposition.CreationDate = File.GetCreationTime(filePath);
            disposition.ModificationDate = File.GetLastWriteTime(filePath);
            disposition.ReadDate = File.GetLastAccessTime(filePath);
            return attachment;
        }
        #endregion
    }
}
