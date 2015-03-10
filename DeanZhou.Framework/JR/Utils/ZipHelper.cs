using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace JFx.Utils
{
    /// <summary>
    /// 文件压缩解压辅助类
    /// </summary>
    public class ZipHelper
    {
        /// <summary>
        /// 存放待压缩的文件的绝对路径
        /// </summary>
        private List<string> AbsolutePaths { set; get; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorMsg { set; get; }
        /// <summary>
        /// 文件压缩解压辅助类
        /// </summary>
        public ZipHelper()
        {
            ErrorMsg = "";
            AbsolutePaths = new List<string>();
        }
        /// <summary>
        /// 添加压缩文件
        /// </summary>
        /// <param name="fileAbsolutePath">文件的绝对路径</param>
        public void AddFile(string fileAbsolutePath)
        {
            AbsolutePaths.Add(fileAbsolutePath);
        }
        /// <summary>
        /// 压缩文件或者文件夹
        /// <para>zip.AddFile(Environment.CurrentDirectory + "\\ZipFile\\6035021_111610206000_2.jpg");</para>
        /// <para>zip.AddFile(Environment.CurrentDirectory + "\\ZipFile\\test.txt");</para>
        /// <para>bool zipResult = zip.CompressionZip(Environment.CurrentDirectory + "\\ZipFile\\test.zip");</para>
        /// </summary>
        /// <param name="depositPath">压缩后文件的存放路径   如C:\\windows\abc.zip</param>
        /// <param name="level">压缩等级：0-9,压缩等级越小，压缩比越高，压缩后的文件越小，耗时越长。</param>
        /// <returns>是否压缩成功</returns>
        public bool CompressionZip(string depositPath,int level=5)
        {
            bool result = true;
            FileStream fs = null;
            try
            {
                ZipOutputStream ComStream = new ZipOutputStream(File.Create(depositPath));
                ComStream.SetLevel(level);      //压缩等级
                foreach (string path in AbsolutePaths)
                {
                    //如果是目录
                    if (Directory.Exists(path))
                    {
                        ZipFloder(path, ComStream, path);
                    }
                    else if (File.Exists(path))//如果是文件
                    {
                        fs = File.OpenRead(path);
                        byte[] bts = new byte[fs.Length];
                        fs.Read(bts, 0, bts.Length);
                        ZipEntry ze = new ZipEntry(new FileInfo(path).Name);
                        ComStream.PutNextEntry(ze);             //为压缩文件流提供一个容器
                        ComStream.Write(bts, 0, bts.Length);  //写入字节
                    }
                }
                ComStream.Finish(); // 结束压缩
                ComStream.Close();
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                ErrorMsg = ex.Message;
                result = false;
            }
            return result;
        }
        //压缩文件夹
        private void ZipFloder(string ofloderPath, ZipOutputStream zos, string floderPath)
        {
            foreach (FileSystemInfo item in new DirectoryInfo(floderPath).GetFileSystemInfos())
            {
                if (Directory.Exists(item.FullName))
                {
                    ZipFloder(ofloderPath, zos, item.FullName);
                }
                else if (File.Exists(item.FullName))//如果是文件
                {
                    DirectoryInfo ODir = new DirectoryInfo(ofloderPath);
                    string fullName2 = new FileInfo(item.FullName).FullName;
                    string path = ODir.Name + fullName2.Substring(ODir.FullName.Length, fullName2.Length - ODir.FullName.Length);//获取相对目录
                    FileStream fs = File.OpenRead(fullName2);
                    byte[] bts = new byte[fs.Length];
                    fs.Read(bts, 0, bts.Length);
                    ZipEntry ze = new ZipEntry(path);
                    zos.PutNextEntry(ze);             //为压缩文件流提供一个容器
                    zos.Write(bts, 0, bts.Length);  //写入字节
                }
            }
        }

        /// <summary>
        /// 解压文件
        /// <para>zip.DeCompressionZip(zipPath, Environment.CurrentDirectory + "\\ZipFile\\DeCompression",out files)</para>
        /// </summary>
        /// <param name="depositPath">压缩文件路径</param>
        /// <param name="floderPath">解压的路径</param>
        /// <returns>是否解压成功</returns>
        public bool DeCompressionZip(string depositPath, string floderPath)
        {
            string[] files = new string[] { };
            return DeCompressionZip(depositPath, floderPath, out files);
        }
        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="depositPath">压缩文件路径</param>
        /// <param name="floderPath">解压的路径</param>
        /// <param name="filePaths">解压出来的所有文件路径</param>
        /// <returns>是否解压成功</returns>
        public bool DeCompressionZip(string depositPath, string floderPath, out string[] filePaths)
        {
            bool result = true;
            FileStream fs = null;
            List<string> files = new List<string>();
            try
            {
                ZipInputStream InpStream = new ZipInputStream(File.OpenRead(depositPath));
                ZipEntry ze = InpStream.GetNextEntry();//获取压缩文件中的每一个文件
                Directory.CreateDirectory(floderPath);//创建解压文件夹
                while (ze != null)//如果解压完ze则是null
                {
                    if (ze.IsFile)//压缩zipINputStream里面存的都是文件。带文件夹的文件名字是文件夹\\文件名
                    {
                        string[] strs = ze.Name.Split('\\');//如果文件名中包含’\\‘则表明有文件夹
                        if (strs.Length > 1)
                        {
                            //两层循环用于一层一层创建文件夹
                            for (int i = 0; i < strs.Length - 1; i++)
                            {
                                string floderPathStr = floderPath;
                                for (int j = 0; j < i; j++)
                                {
                                    floderPathStr = floderPathStr + "\\" + strs[j];
                                }
                                floderPathStr = floderPathStr + "\\" + strs[i];
                                Directory.CreateDirectory(floderPathStr);
                            }
                        }
                        files.Add(floderPath + "\\" + ze.Name);
                        fs = new FileStream(floderPath + "\\" + ze.Name, FileMode.OpenOrCreate, FileAccess.Write);//创建文件
                        //循环读取文件到文件流中
                        while (true)
                        {
                            byte[] bts = new byte[1024];
                            int i = InpStream.Read(bts, 0, bts.Length);
                            if (i > 0)
                            {
                                fs.Write(bts, 0, i);
                            }
                            else
                            {
                                fs.Flush();
                                fs.Close();
                                break;
                            }
                        }
                    }
                    ze = InpStream.GetNextEntry();
                }
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                ErrorMsg = ex.Message;
                result = false;
            }
            filePaths = files.ToArray();
            return result;
        }
    }
}

