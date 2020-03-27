using System.IO;

using iBank.Services.Implementation.Utilities.eFFECTS;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.Utilities
{
    [TestClass]
    public class FileHandlerTests
    {
        private static readonly string _sourceDir = @"C:\CopyFilesTest\Source\";

        private static readonly string _targetDir = @"C:\CopyFilesTest\Target\";
        private static readonly string _sourceFile = _sourceDir + "test.txt";
        
        [TestInitialize]
        public void Init()
        {
            //create source directory
            if (!Directory.Exists(_sourceDir)) Directory.CreateDirectory(_sourceDir);
            //create source file
            File.WriteAllText(_sourceFile, "test");

            //create target directory
            if (!Directory.Exists(_targetDir)) Directory.CreateDirectory(_targetDir);
        }

        [TestCleanup]
        public void Teardown()
        {
            //cleanup source directory
            if (Directory.Exists(_sourceDir))
            {
                var dir = new DirectoryInfo(_sourceDir);
                dir.Delete(true);
            }

            //cleanup target directory
            if (Directory.Exists(_targetDir))
            {
                var dir = new DirectoryInfo(_targetDir);
                dir.Delete(true);
            }
        }

        [TestMethod]
        public void CopyFile_FilesExist_CopyFileReturnEmptyMessage()
        {
            var handler = new FileHandler();
            var targetFile = _targetDir + "target.txt";

            var output = handler.CopyFile(_sourceFile, targetFile);

            Assert.AreEqual(true, File.Exists(targetFile));
            Assert.AreEqual("", output.ReturnMessage);
            Assert.AreEqual(1, output.ReturnCode);
        }

        [TestMethod]
        public void CopyFile_FilesDontExist_DontCopyReturnErrorMessage()
        {
            var handler = new FileHandler();
            var targetFile = _targetDir + "target.txt";
            var nonExistentSource = _sourceDir + "noexist.txt";

            var output = handler.CopyFile(nonExistentSource, targetFile);

            Assert.AreEqual(false, File.Exists(targetFile));
            Assert.AreEqual("Unable to copy eFFECTS output to " + targetFile, output.ReturnMessage);
            Assert.AreEqual(3, output.ReturnCode);
        }

        [TestMethod]
        public void ZipFile_FilesExistTargetDirectoryExists_CreateZipFile()
        {
            var handler = new FileHandler();
            var targetFile = _targetDir + "targetfile.txt";

            var output = handler.ZipFile(_sourceFile, targetFile);
            
            Assert.AreEqual("", output.ReturnMessage);
            Assert.AreEqual(1, output.ReturnCode);
        }
    }
}
