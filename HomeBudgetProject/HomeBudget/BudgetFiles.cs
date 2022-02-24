using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{

    /// <summary>
    /// BudgetFiles class is used to manage the files used in the Budget project.
    /// </summary>
    public class BudgetFiles
    {
        private static String DefaultSavePath = @"Budget\";
        private static String DefaultAppData = @"%USERPROFILE%\AppData\Local\";

        // ====================================================================
        // verify that the name of the file, or set the default file, and 
        // is it readable?
        // throws System.IO.FileNotFoundException if file does not exist
        // ====================================================================

        /// <summary>
        /// Verifies if you can read from a file and returns the file path. It receives 2 string values as arguments (<c>FilePath</c> and <c>DefaultFileName</c>). 
        /// If <c>FilePath</c> is null, it will set the value to a file path with the file name <c>DefaultFileName</c> and It will return the value of <c>FilePath</c> if it exists.
        /// </summary>
        /// <param name="FilePath">The file path of the file to read from</param>
        /// <param name="DefaultFileName">The default file name given to file path if <c>FilePath</c> is null</param>
        /// <exception cref="FileNotFoundException">Thrown when <c>FilePath</c> does not exist</exception>
        /// <returns>The valid file path will be returned if it exists</returns>
        /// <remarks>
        /// The default filepath that will be used if <c>FilePath</c> is null consists of a given filepath and a file name(<c>DefaultFileName</c>).
        /// </remarks>
        /// <example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// BudgetFiles files = new BudgetFiles();
        /// 
        /// //Calls method to verify read from filepath and stores return value in GoodFilePath.
        /// String GoodFilePath = files.VerifyReadFromFileName(FilePath, DefaultFileName)
        /// 
        /// //Outputs the file path received from the previous method to the console.
        /// Console.WriteLine(GoodFilePath)
        /// ]]>
        /// </code>
        /// </example>
        public static String VerifyReadFromFileName(String FilePath, String DefaultFileName)
        {

            // ---------------------------------------------------------------
            // if file path is not defined, use the default one in AppData
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
            }

            // ---------------------------------------------------------------
            // does FilePath exist?
            // ---------------------------------------------------------------
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException("ReadFromFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ----------------------------------------------------------------
            // valid path
            // ----------------------------------------------------------------
            return FilePath;

        }

        // ====================================================================
        // verify that the name of the file, or set the default file, and 
        // is it writable
        // ====================================================================

        /// <summary>
        /// Verifies if you can write to a file and returns the file path. It receives 2 string values as arguments (<c>FilePath</c> and <c>DefaultFileName</c>). 
        /// If <c>FilePath</c> is null, it will set the value to a file path with the file name <c>DefaultFileName</c> by creating the necessary directories.
        /// It will then verify if the directory and file exists. The method will finally check if you can write to the file.
        /// </summary>
        /// <param name="FilePath">The file path of the file to write in</param>
        /// <param name="DefaultFileName">Default filename given to filepath if FilePath is null</param>
        /// <exception cref="Exception">Thrown when <c>FilePath</c> does not exist or when <c>FilePath</c> is read-only meaning that you cannot write in the file.</exception>
        /// <returns>The valid file path will be returned if you could write to the file</returns>
        /// <remarks>
        /// The default filepath that will be used if <c>FilePath</c> is null consists of a given filepath and a file name(<c>DefaultFileName</c>).
        /// </remarks>
        /// <example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// BudgetFiles files = new BudgetFiles();
        /// 
        /// //Calls method to verify write to filepath and stores return value in GoodFilePath.
        /// String GoodFilePath = files.VerifyWriteToFileName(FilePath, DefaultFileName)
        /// 
        /// //Outputs the file path received from the previous method to the console.
        /// Console.WriteLine(GoodFilePath)
        /// ]]>
        /// </code>
        /// </example>
        public static String VerifyWriteToFileName(String FilePath, String DefaultFileName)
        {
            // ---------------------------------------------------------------
            // if the directory for the path was not specified, then use standard application data
            // directory
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                // create the default appdata directory if it does not already exist
                String tmp = Environment.ExpandEnvironmentVariables(DefaultAppData);
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                }

                // create the default Budget directory in the appdirectory if it does not already exist
                tmp = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath);
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                }

                FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
            }

            // ---------------------------------------------------------------
            // does directory where you want to save the file exist?
            // ... this is possible if the user is specifying the file path
            // ---------------------------------------------------------------
            String folder = Path.GetDirectoryName(FilePath);
            String delme = Path.GetFullPath(FilePath);
            if (!Directory.Exists(folder))
            {
                throw new Exception("SaveToFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ---------------------------------------------------------------
            // can we write to it?
            // ---------------------------------------------------------------
            if (File.Exists(FilePath))
            {
                FileAttributes fileAttr = File.GetAttributes(FilePath);
                if ((fileAttr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    throw new Exception("SaveToFileException:  FilePath(" + FilePath + ") is read only");
                }
            }

            // ---------------------------------------------------------------
            // valid file path
            // ---------------------------------------------------------------
            return FilePath;

        }



    }
}
