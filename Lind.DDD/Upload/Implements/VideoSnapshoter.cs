using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lind.DDD.Upload
{
    public class VideoSnapshoter
    {
        public List<string> GetVideoSnapshots(string file, out string imgFilePath)
        {
            List<string> snapshotList = new List<string>();

            string p = HttpRuntime.AppDomainAppPath;
            string videoFile = file;
            string ffmpegfile = p + @"Images\ffmpeg.exe";
            DateTime dt = DateTime.Now;
            string ourFilePath = p + @"Images\ffmpegImage\" + Guid.NewGuid().ToString() + @"\";
            imgFilePath = ourFilePath;
            if (!Directory.Exists(ourFilePath))
            {
                Directory.CreateDirectory(ourFilePath);
            }

            ourFilePath += dt.Minute + "_" + dt.Millisecond + "img";

            ProcessStartInfo psi = new ProcessStartInfo(ffmpegfile);
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            psi.Arguments = " -i " + videoFile + " -r 1 -ss 00:00:08 -t 00:00:12 -s 688x389 " + ourFilePath + "%05d.png";


            System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);
            process.WaitForExit();
            for (int i = 1; i <= 8; i++)
            {
                string fileName = ourFilePath + i.ToString("D5")+".png";
                if (File.Exists(fileName))
                    snapshotList.Add(fileName);
            }
            return snapshotList;
        }
    }
}
