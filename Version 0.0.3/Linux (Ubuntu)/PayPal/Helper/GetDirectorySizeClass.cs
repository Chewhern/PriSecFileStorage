using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace PriSecFileStorageAPI.Helper
{
    public static class GetDirectorySizeClass
    {
        public static int GetDirectorySize(String Path)
        {
            String[] Files = Directory.GetFiles(Path);
            String[] SubDirectory = Directory.GetDirectories(Path);
            String[] FilesInSubDirectory = new String[] { };

            int FileLength = 0;
            int TemporaryDirectoryAccumulatedFileLength = 0;

            foreach (String File in Files)
            {
                FileInfo info = new FileInfo(File);
                FileLength += (int)info.Length;
            }

            foreach (String Folder in SubDirectory)
            {
                FilesInSubDirectory = Directory.GetFiles(Folder);
                foreach (String File in FilesInSubDirectory)
                {
                    FileInfo info = new FileInfo(File);
                    TemporaryDirectoryAccumulatedFileLength += (int)info.Length;
                }
            }

            return FileLength + TemporaryDirectoryAccumulatedFileLength;
        }
    }
}
