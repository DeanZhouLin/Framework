using System;
using System.IO;
using System.Net;
using System.Reflection;


namespace JFx.Utils
{
    /// <summary>
    /// ftp上传下载文件方法
    /// </summary>
    public class FtpHelper
    {
        string ftpServerIP;
        string ftpServerPot;
        string ftpUserID;
        string ftpPassword;
        FtpWebRequest reqFTP;

        /// <summary>
        /// FTP构建方法
        /// </summary>
        /// <param name="ftpServerIP">IP</param>
        /// <param name="ftpServerPot">Port</param>
        /// <param name="ftpUserID">User</param>
        /// <param name="ftpPassword">PassWord</param>
        public FtpHelper(string ftpServerIP, string ftpServerPot, string ftpUserID, string ftpPassword)
        {
            this.ftpServerIP = ftpServerIP;
            this.ftpServerPot = ftpServerPot;
            this.ftpUserID = ftpUserID;
            this.ftpPassword = ftpPassword;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileinfo">需要上传的文件</param>
        /// <param name="filename">需要上传的文件名称</param>
        /// <param name="fileDeptempent">需要上传的文件部门名称，按照部门划分上传路径文件夹</param>
        public void UploadFile(MemoryStream fileinfo, string filename,string fileDeptempent)
        {

            string URI = string.Format("ftp://{0}/{1}/{2}", ftpServerIP, fileDeptempent, filename);
          
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(URI));
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.KeepAlive = false;
            reqFTP.ContentLength = fileinfo.Length;

            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // 打开一个文件流 (System.IO.FileStream) 去读上传的文件
            //FileStream fs = fileinfo.OpenRead();
            Stream strm = null;
            //重要
            fileinfo.Position = 0;
            try
            {
                // 把上传的文件写入流
                strm = reqFTP.GetRequestStream();
                // 每次读文件流的2kb
                contentLen = fileinfo.Read(buff, 0, buffLength);
                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入 upload stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fileinfo.Read(buff, 0, buffLength);
                }


            }
            catch (Exception ex)
            {

            }
            finally
            {
                // 关闭两个流
                if (strm != null)
                    strm.Close();
            }

        }
        /// <summary>
        /// Ftp下载
        /// </summary>
        /// <param name="remoteFileName">远程文件路径</param>
        /// <param name="localFileName">本地文件路径</param>
        /// <param name="outMsg">错误消息</param>
        /// <returns>true 下载成功 false 下载失败</returns>
        public bool DownloadFile(string remoteFileName, string localFileName, out string outMsg)
        {
            Stream ftpStream = null;
            FtpWebResponse response = null;
            FileStream outputStream = null;

            try
            {
                outMsg = string.Empty;

                //本地文件是否存在
                if (File.Exists(localFileName))
                {
                    outMsg = string.Format("本地文件{0}已存在,无法下载", localFileName);
                    return false; ;
                }

                //远程文件是否存在
                if (!IsRemoteFileExist(remoteFileName))
                {
                    outMsg = string.Format(" 远程文件{0}不存在,无法下载", remoteFileName);
                    return false;
                }

                //本地路径创建
                string strDirectory = localFileName.Substring(0, localFileName.LastIndexOf("\\"));
                if (!Directory.Exists(strDirectory))
                {
                    Directory.CreateDirectory(strDirectory);
                }

                //ftp下载
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(remoteFileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                response = (FtpWebResponse)reqFTP.GetResponse();

                ftpStream = response.GetResponseStream();

                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);

                //写入本地文件
                outputStream = new FileStream(localFileName, FileMode.Create);

                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                return true;

            }

            catch (Exception ex)
            {
                outMsg = string.Format("远程文件：{0},无法下载错误信息：{1}", remoteFileName, ex.Message);
                return false;
            }
            finally
            {
                if (outputStream != null)
                {
                    outputStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (ftpStream != null)
                {
                    ftpStream.Close();
                }
            }
        }

        /// <summary>
        /// ftp服务器上文件是否存在
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool IsRemoteFileExist(string fileName)
        {
            bool IsResult = false;

            FtpWebResponse response = null;
            StreamReader reader = null;
            try
            {

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(fileName));

                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

                response = (FtpWebResponse)reqFTP.GetResponse();

                reader = new StreamReader(response.GetResponseStream());

                string line = reader.ReadLine();

                if (line != null && line.Trim().Length > 0)
                {
                    IsResult = true;
                }
            }

            catch (Exception ex)
            {

                IsResult = false;
            }

            finally
            {

                if (reader != null)
                {
                    reader.Close();
                }

                if (response != null)
                {
                    response.Close();
                }

            }

            return IsResult;

        }

    }
}
