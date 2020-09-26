//using System;
//using System.IO;
//using System.Threading.Tasks;
//#if __ANDROID__
//using Android.App;
//using Android.OS;
//#endif
//#if __IOS__
//using Foundation;
//#endif

//namespace SalveminiApp.Helpers
//{
//    public class GetStorageInfo
//    {

//        public static Task<ulong[]> storageInfo(bool delete)
//        {
//            try
//            {
//                //Create list
//                ulong[] sInfo;

//                //Get Size of specific Path
//                long DirSize(DirectoryInfo d)
//                {
//                    long size = 0;

//                    // Add file sizes.
//                    FileInfo[] fis = d.GetFiles();
//                    if (d.Name != "com.apple.metal" || d.Name != "fsCachedData")
//                    {

//                        foreach (FileInfo fi in fis)
//                        {

//                            if (delete)
//                            {

//                                try
//                                {
//                                    fi.Delete();
//                                    continue;
//                                }
//                                catch
//                                {
//                                    continue;
//                                }

//                            }
//                            try
//                            {

//                                size += fi.Length;
//                            }
//                            catch
//                            {
//                                continue;
//                            }

//                        }
//                    }

//                    // Add subdirectory sizes.
//                    DirectoryInfo[] dis = d.GetDirectories();
//                    foreach (DirectoryInfo di in dis)
//                    {
//                        try
//                        {
//                            size += DirSize(di);
//                        }
//                        catch
//                        {
//                            continue;
//                        }
//                    }
//                    return size;
//                }

//                string cacheFolder = "";
//                ulong freeSpace = 0;
//                ulong totalSpace = 0;
//#if __IOS__


//                //Cache Path
//                cacheFolder = NSSearchPath.GetDirectories(NSSearchPathDirectory.CachesDirectory, NSSearchPathDomain.User)[0];

//                //FreeSpace (GB)
//                freeSpace = NSFileManager.DefaultManager.GetFileSystemAttributes(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).FreeSize;

//                //TotalSpace (GB)
//                totalSpace = NSFileManager.DefaultManager.GetFileSystemAttributes(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).Size;
//#endif

//#if __ANDROID__

//            //Cache Path
//            cacheFolder = Application.Context.CacheDir.AbsolutePath;
//            StatFs statFs = new StatFs("/");
//            totalSpace = (ulong)((long)statFs.BlockCount * (long)statFs.BlockSize);
//            freeSpace = (ulong)(statFs.AvailableBlocks * (long)statFs.BlockSize);
//#endif
//                DirectoryInfo info = new DirectoryInfo(cacheFolder);

//                var cacheSize = (ulong)DirSize(info);

//                sInfo = new ulong[] { cacheSize, freeSpace, totalSpace };

//                return Task.FromResult(sInfo);
//            }
//            catch
//            {
//                return null;
//            }
          

//        }

//        public string ChangeUnit(ulong arg)
//        {
//            var value = (double)arg;

//            if (value < 1024)
//            {
//                return "0 Kb";
//            }
//            //Check if value is bigger than 1kb
//            if (value >= 1024)
//            {
//                //Convert byte to kilobyte
//                value = value / 1024;

//                //Check if value is bigger than 1Mb
//                if (value >= 1024)
//                {
//                    //Convert kilobyte to Megabyte
//                    value = value / 1024;

//                    //Check if value is bigger than 1Gb
//                    if (value >= 1024)
//                    {
//                        //Convert Megabyte to Gigabyte
//                        value = value / 1024;

//                        return value.ToString("##.#") + " Gb";
//                    }
//                    else
//                    {
//                        return value.ToString("##.#") + " Mb";
//                    }
//                }
//                else
//                {
//                    return value.ToString("##.#") + " Kb";
//                }
//            }
//            else
//            {
//                return value.ToString("##.#") + " Byte";
//            }
//        }

//    }
//}

