namespace Dragonfly.NetHelperServices;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Dragonfly.NetHelpers;
using Dragonfly.NetModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

#pragma warning disable 0168

/// <summary>
/// Helpers to handle File I/O
/// </summary>
public class FileHelperService
{
	private readonly IWebHostEnvironment _webHostEnvironment;

	private readonly ILogger _logger;
	private readonly string _MappedRootPath;
	private string DefaultLogPrefix = "Dragonfly.NetHelperServices.FileHelperService";

	//TODO: HLF - TEST ALL Methods in Service

	/// <summary>
	/// Helpers to handle File I/O
	/// </summary>
	/// <param name="logger"></param>
	/// <param name="webHostEnvironment"></param>
	public FileHelperService(
		 ILogger<FileHelperService> logger,
		 IWebHostEnvironment webHostEnvironment
		)
	{
		_logger = logger;
		_webHostEnvironment = webHostEnvironment;
		_MappedRootPath = webHostEnvironment.MapPathWebRoot("/");
	}

	#region Retrieve Remote Files (HTTP)

	/// <summary>
	/// Get a file from a url and save it to the filesystem.
	/// </summary>
	/// <param name="FileUrl">Http url of file to save</param>
	/// <param name="SaveLocationFolder">disk folder where the file should be saved (can be virtual or mapped)</param>
	/// <param name="SaveFileName">Desired filename for saved file</param>
	public void DownloadAndSaveHttpFile(string FileUrl, string SaveLocationFolder, string SaveFileName)
	{
		string SaveLocation = String.Concat(SaveLocationFolder, "\\", SaveFileName);

		DownloadAndSaveHttpFile(FileUrl, SaveLocation);
	}

	/// <summary>
	/// Get a file from a url and save it to the filesystem.
	/// </summary>
	/// <param name="FileUrl">Http url of file to save</param>
	/// <param name="SaveLocation">Disk location (incl. filename) where the file should be saved (can be virtual or mapped)</param>
	public void DownloadAndSaveHttpFile(string FileUrl, string SaveLocation)
	{
		string RemoteURL = FileUrl;

		// _logger.LogInformation($"{DefaultLogPrefix}.DownloadHttpFile: RemoteURL=" + RemoteURL);

		string ServerPath = "";
		try
		{
			var canGetPath = TryGetMappedPath(SaveLocation, out ServerPath);
			//ServerPath = _webHostEnvironment.MapPathWebRoot(SaveLocation);
		}
		catch (Exception exMapPath)
		{
			ServerPath = SaveLocation;
		}

		//FileStream writeStream = new FileStream(ServerPath, FileMode.Create);

		try
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(RemoteURL);
			request.Method = WebRequestMethods.Http.Get;

			//Stream fileResponseStream;

			HttpWebResponse fileResponse = (HttpWebResponse)request.GetResponse();

			//fileResponseStream = fileResponse.GetResponseStream();

			using (Stream writeStream = File.OpenWrite(ServerPath))
			using (Stream fileResponseStream = fileResponse.GetResponseStream())
			{
				fileResponseStream.CopyTo(writeStream);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Error in {DefaultLogPrefix}.DownloadFtpFile");
		}
	}

	#endregion

	#region FTP

	/// <summary>
	/// Gets a list of files in the FTP folder
	/// </summary>
	/// <param name="FtpHostServer"></param>
	/// <param name="FtpUserName"></param>
	/// <param name="FtpPassword"></param>
	/// <param name="UsePassive"></param>
	/// <returns></returns>
	public string[] GetFtpFileList(string FtpHostServer, string FtpUserName, string FtpPassword, bool UsePassive = true)
	{
		string[] downloadFiles;
		StringBuilder result = new StringBuilder();
		WebResponse response = null;
		StreamReader reader = null;
		try
		{
			FtpWebRequest reqFTP;
			reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + FtpHostServer + "/"));
			reqFTP.UseBinary = true;
			reqFTP.Credentials = new NetworkCredential(FtpUserName, FtpPassword);
			reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
			reqFTP.Proxy = null;
			reqFTP.KeepAlive = false;
			reqFTP.UsePassive = UsePassive;
			response = reqFTP.GetResponse();
			reader = new StreamReader(response.GetResponseStream());
			string line = reader.ReadLine();
			while (line != null)
			{
				result.Append(line);
				result.Append("\n");
				line = reader.ReadLine();
			}
			// to remove the trailing '\n'
			result.Remove(result.ToString().LastIndexOf('\n'), 1);
			return result.ToString().Split('\n');
		}
		catch (Exception ex)
		{
			if (reader != null)
			{
				reader.Close();
			}
			if (response != null)
			{
				response.Close();
			}
			downloadFiles = null;
			return downloadFiles;
		}
	}

	/// <summary>
	/// Retrieves a file from an FTP server and saves it 
	/// </summary>
	/// <param name="FtpHostServer"></param>
	/// <param name="FtpUserName"></param>
	/// <param name="FtpPassword"></param>
	/// <param name="FtpDirectoryPath"></param>
	/// <param name="FtpFileName"></param>
	/// <param name="SaveLocationPath"></param>
	/// <param name="SaveFileName"></param>
	/// <param name="TimeoutMinutes">NULL = Timeout.Infinite</param>
	/// <returns></returns>
	public bool DownloadFtpFile(string FtpHostServer, string FtpUserName, string FtpPassword, string FtpDirectoryPath, string FtpFileName, string SaveLocationPath, string SaveFileName, int? TimeoutMinutes = null)
	{
		string remoteUrl = "ftp://" + FtpHostServer + "/" + FtpDirectoryPath + "/" + FtpFileName;

		// _logger.LogInformation($"{DefaultLogPrefix}.Files.DownloadFtpFile: RemoteURL=" + RemoteURL);

		string filePath = FtpDirectoryPath + "/" + FtpFileName;
		string serverPath = "";
		try
		{
			var canGetPath = TryGetMappedPath(SaveLocationPath, out serverPath);
			//ServerPath = _webHostEnvironment.MapPathWebRoot(SaveLocationPath);
		}
		catch (Exception exMapPath)
		{
			serverPath = SaveLocationPath;
		}

		string fullSaveLocation = String.Concat(serverPath, "\\", SaveFileName);

		_logger.LogInformation($"{DefaultLogPrefix}.DownloadFtpFile: FullSaveLocation=" + fullSaveLocation);

		//Test that server can be accessed
		_logger.LogInformation($"{DefaultLogPrefix}.DownloadFtpFile: FtpServerStatus=" + FtpServerStatus(FtpHostServer, FtpUserName, FtpPassword));
		_logger.LogInformation($"{DefaultLogPrefix}.DownloadFtpFile: FtpDirectoryStatus=" + FtpDirectoryStatus(FtpHostServer, FtpUserName, FtpPassword, FtpDirectoryPath));

		FileStream writeStream = new FileStream(fullSaveLocation, FileMode.Create);
		try
		{
			long length = GetFileLength(FtpHostServer, FtpUserName, FtpPassword, filePath, true);

			_logger.LogInformation($"{DefaultLogPrefix}.DownloadFtpFile: GetFileLength(RemoteURL)=" + length);
			long offset = 0;
			int retryCount = 10;
			int? readTimeout = TimeoutMinutes != null ? TimeoutMinutes * 60 * 1000 : null; //TimeoutMinutes calculated to milliseconds

			while (retryCount > 0)
			{
				using (Stream responseStream = GetFileAsStream(remoteUrl, FtpUserName, FtpPassword, true, offset, RequestTimeout: readTimeout != null ? readTimeout.Value : Timeout.Infinite))
				{
					_logger.LogInformation($"{DefaultLogPrefix}.DownloadFtpFile: GetFileAsStream(RemoteURL).length" + responseStream.Length);

					using (FileStream fileStream = new FileStream(fullSaveLocation, FileMode.Append))
					{
						byte[] buffer = new byte[4096];
						try
						{
							int bytesRead = responseStream.Read(buffer, 0, buffer.Length);

							while (bytesRead > 0)
							{
								fileStream.Write(buffer, 0, bytesRead);

								bytesRead = responseStream.Read(buffer, 0, buffer.Length);
							}

							return true;
						}
						catch (WebException exWeb)
						{
							// Do nothing - consume this exception to force a new read of the rest of the file
							_logger.LogError(exWeb, $"{DefaultLogPrefix}.DownloadFtpFile - HANDLED");
						}
					}

					_logger.LogInformation($"{DefaultLogPrefix}.DownloadFtpFile : File.Exists(FullSaveLocation)=", File.Exists(fullSaveLocation));

					if (File.Exists(fullSaveLocation))
					{
						offset = new FileInfo(fullSaveLocation).Length;
					}
					else
					{
						offset = 0;
					}

					retryCount--;

					if (offset == length)
					{
						return true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"{DefaultLogPrefix}.DownloadFtpFile", ex);
		}

		return false;
	}

	private string FtpDirectoryStatus(string FtpHostServer, string username, string password, string FtpFolderPathToTest = "")
	{
		string ReturnMsg = "";
		string FullDirToTest = "ftp://" + FtpHostServer + "/" + FtpFolderPathToTest;

		_logger.LogInformation($"{DefaultLogPrefix}.FtpDirectoryExists: FullDirToTest=" + FullDirToTest);

		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FullDirToTest);
		request.Credentials = new NetworkCredential(username, password);
		string CredentialsInfo = request.Credentials.ToString();
		request.Method = WebRequestMethods.Ftp.ListDirectory;

		try
		{
			using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
			{
				// Okay.  
				ReturnMsg = "Server Connection Successful";
			}
		}
		catch (WebException ex)
		{
			if (ex.Response != null)
			{
				FtpWebResponse response = (FtpWebResponse)ex.Response;
				ReturnMsg = response.StatusCode.ToString();

				if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
				{
					// Directory not found.  
					ReturnMsg += " - " + FullDirToTest;
				}
				else if (response.StatusCode == FtpStatusCode.NotLoggedIn)
				{
					// Directory not found.  
					ReturnMsg += " - " + CredentialsInfo;
				}

			}
			ReturnMsg = "Unknown";
		}
		return ReturnMsg;
	}

	private string FtpServerStatus(string FtpHostServer, string username, string password)
	{
		string ReturnMsg = "";
		string ServerToTest = "ftp://" + FtpHostServer + "/";

		_logger.LogInformation($"{DefaultLogPrefix}.FtpServerStatus: ServerToTest=" + ServerToTest);

		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ServerToTest);
		request.Credentials = new NetworkCredential(username, password);
		string CredentialsInfo = request.Credentials.ToString();
		request.Method = WebRequestMethods.Ftp.ListDirectory;

		try
		{
			using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
			{
				// Okay.  
				ReturnMsg = "Server Connection Successful";
			}
		}
		catch (WebException ex)
		{
			if (ex.Response != null)
			{
				FtpWebResponse response = (FtpWebResponse)ex.Response;
				ReturnMsg = response.StatusCode.ToString();

				if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
				{
					// Directory not found.  
					ReturnMsg += " - " + ServerToTest;
				}
				else if (response.StatusCode == FtpStatusCode.NotLoggedIn)
				{
					// Directory not found.  
					ReturnMsg += " - " + CredentialsInfo;
				}

			}
			ReturnMsg = "Unknown";
		}
		return ReturnMsg;
	}

	private Stream GetFileAsStream(string FtpUrl, string Username, string Password, bool UsePassive, long Offset, int RequestTimeout)
	{
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FtpUrl);

		request.KeepAlive = false;
		request.ReadWriteTimeout = RequestTimeout;
		request.Timeout = RequestTimeout;
		request.ContentOffset = Offset;
		request.UsePassive = UsePassive;
		request.UseBinary = true;

		request.Credentials = new NetworkCredential(Username, Password);

		request.Method = WebRequestMethods.Ftp.DownloadFile;

		Stream fileResponseStream;

		FtpWebResponse fileResponse = (FtpWebResponse)request.GetResponse();

		fileResponseStream = fileResponse.GetResponseStream();

		return fileResponseStream;
	}


	#endregion

	#region Create Files/Folders

	/// <summary>
	/// Will check for the existence of a directory on disk and create it if missing
	/// </summary>
	/// <param name="FolderPath">Path to directory</param>
	/// <returns>TRUE if sucessful</returns>
	public bool CreateDirectoryIfMissing(string FolderPath)
	{
		bool success = false;

		string mappedFolderPath = "";
		var canGetPath = TryGetMappedPath(FolderPath, out mappedFolderPath);

		if (Directory.Exists(mappedFolderPath))
		{
			success = true;
		}
		else
		{
			try
			{
				Directory.CreateDirectory(mappedFolderPath);
				success = true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"{DefaultLogPrefix}.CreateDirectoryIfMissing - [MappedFolderPath=" + mappedFolderPath + "]");
				success = false;
			}
		}
		return success;
	}

	/// <summary>
	/// Creates an empty file at a location, creating directories as needed
	/// </summary>
	/// <param name="FullFilePath">Path for directories and file</param>
	/// <returns>Filestream for new file</returns>
	public FileStream CreateFileAndDirectory(string FullFilePath)
	{
		string directoryName = Path.GetDirectoryName(FullFilePath);

		if (Directory.Exists(directoryName) == false)
		{
			Directory.CreateDirectory(directoryName);
		}

		FileStream fs = File.Create(FullFilePath);

		return fs;
	}

	/// <summary>
	/// Writes some text to a provided file location.
	/// </summary>
	/// <param name="FilePath">Virtual or Physical path - Inlcuding the desired filename with a text-compatible extension (ex: .txt, .xml, .json, etc.)</param>
	/// <param name="TextContent">Text to write to file</param>
	/// <param name="CreateDirectoryIfMissing">If the directories int he path don't exist, create them rather than failing</param>
	/// <param name="FailSilently">If TRUE won't throw an error on failure. Included for backward compatibility.</param>
	/// <returns></returns>
	public bool CreateTextFile(string FilePath, string TextContent, bool CreateDirectoryIfMissing = false, bool FailSilently = true)
	{
		string mappedFilePath = "";
		var canGetPath = TryGetMappedPath(FilePath, out mappedFilePath);
		try
		{
			if (CreateDirectoryIfMissing)
			{
				string mappedPath = "";
				var isMappable = TryGetMappedPath(FilePath, out mappedPath);
				string directoryName = Path.GetDirectoryName(mappedPath);

				if (Directory.Exists(directoryName) == false)
				{
					Directory.CreateDirectory(directoryName);
				}
			}

			// WriteAllText creates a file, writes the specified string to the file,
			// and then closes the file.    You do NOT need to call Flush() or Close().
			System.IO.File.WriteAllText(mappedFilePath, TextContent);
		}
		catch (Exception ex)
		{
			if (!FailSilently)
			{
				//Pass error back up
				throw;
			}

			return false;
		}
		return true;
	}

	///// <summary>
	///// Writes text to a file
	///// </summary>
	///// <param name="FilePath">Path and filename</param>
	///// <param name="TextToWrite">Text content to add to file</param>
	///// <param name="Overwrite">If FALSE will just append as a line to existing file contents, TRUE will overwrite all file contents</param>
	///// <param name="PrefixWithTimestamp">Add a timestamp to the beginning of the line appended (useful for log files)</param>
	//TODO: Fix
	//public static void WriteToTextFile(string FilePath, string TextToWrite, bool Overwrite = false, bool PrefixWithTimestamp = true)
	//{
	//    string LogFilePath = "";
	//    try
	//    {
	//var canGetPath = TryGetMappedPath(FilePath, out LogFilePath);
	//       // LogFilePath = _webHostEnvironment.MapPathWebRoot(FilePath);
	//    }
	//    catch (System.Web.HttpException exMapPath)
	//    {
	//        var functionName = string.Format("{0}.WriteToTextFile", ThisClassName);
	//        //_logger.LogError(ex,functionName, exMapPath, "(Error handled by Code)", true);
	//        LogFilePath = FilePath;
	//    }

	//    string textLine;

	//    if (PrefixWithTimestamp)
	//    {
	//        textLine = DateTime.Now + "---" + TextToWrite;
	//        //('yyyy-mm-dd-HH:MM:SS') + 
	//    }
	//    else
	//    {
	//        textLine = TextToWrite;
	//    }

	//    if (Overwrite == true | File.Exists(LogFilePath) == false)
	//    {
	//        FileStream fsNew = Files.CreateFileAndDirectory(LogFilePath);
	//        StreamWriter swNew = new StreamWriter(fsNew);
	//        swNew.WriteLine(textLine);
	//        swNew.Close();
	//        fsNew.Close();
	//    }
	//    else
	//    {
	//        StreamWriter swAppend = File.AppendText(LogFilePath);
	//        swAppend.WriteLine(textLine);
	//        swAppend.Close();
	//    }
	//}

	#endregion

	#region Read Files

	/// <summary>
	/// Gets a list of the files in a directory
	/// </summary>
	/// <param name="FolderPath"></param>
	/// <returns></returns>
	public IEnumerable<string> ListLocalFiles(string FolderPath)
	{
		string mappedPath;
		var isMappable = TryGetMappedPath(FolderPath, out mappedPath);
		var files = System.IO.Directory.EnumerateFiles(mappedPath);

		return files;
	}

	/// <summary>
	/// Reads a Text file, returning contents as a string
	/// </summary>
	/// <param name="FilePath">Full path to file</param>
	/// <returns></returns>
	public string GetTextFileContents(string FilePath)
	{
		string mappedFilePath;
		var isMappable = TryGetMappedPath(FilePath, out mappedFilePath);

		string readText = File.ReadAllText(mappedFilePath);

		return readText;
	}

	//public static bool DisplayFileFromServer(Uri serverUri)
	//{
	//    // The serverUri parameter should start with the ftp:// scheme. 
	//    if (serverUri.Scheme != Uri.UriSchemeFtp)
	//    {
	//        return false;
	//    }
	//    // Get the object used to communicate with the server.
	//    WebClient request = new WebClient();

	//    // This example assumes the FTP site uses anonymous logon.
	//    request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
	//    try
	//    {
	//        byte[] newFileData = request.DownloadData(serverUri.ToString());
	//        string fileString = System.Text.Encoding.UTF8.GetString(newFileData);
	//        Console.WriteLine(fileString);
	//    }
	//    catch (WebException e)
	//    {
	//        Console.WriteLine(e.ToString());
	//    }
	//    return true;
	//}

	#endregion

	#region Get File Information

	/// <summary>
	/// Checks whether a file exists on disk
	/// </summary>
	/// <param name="FullFilePath">Relative or Mapped Path</param>
	/// <returns>True if file found, false if not</returns>
	public bool FileExists(string FullFilePath)
	{
		string mappedFilePath = "";
		try
		{
			var isMappable = TryGetMappedPath(FullFilePath, out mappedFilePath);
			//mappedFilePath = _webHostEnvironment.MapPathWebRoot(FullFilePath);
		}
		catch (Exception exMapPath)
		{
			_logger.LogWarning(exMapPath, $"{DefaultLogPrefix}.FileExists - (Error handled by Code)");
			mappedFilePath = FullFilePath;
		}

		if (File.Exists(mappedFilePath))
		{
			return true;
		}
		else
		{
			return false;
		}

	}

	/// <summary>
	/// Convert bytes into a friendlier format
	/// </summary>
	/// <param name="Bytes">value of the file size in bytes</param>
	/// <param name="FormatString">Adjust the format string to your preferences. For example "{0:0.#}{1}" would show a single decimal place, and no space.</param>
	/// <returns></returns>
	public string GetFriendlyFileSize(double Bytes, string FormatString = "{0:0.##} {1}")
	{
		string[] sizes = { "B", "KB", "MB", "GB" };
		double len = Bytes;
		int order = 0;
		while (len >= 1024 && order + 1 < sizes.Length)
		{
			order++;
			len = len / 1024;
		}

		string result = string.Format(FormatString, len, sizes[order]);

		return result;
	}

	/// <summary>
	/// Gets the dimensions of an image
	/// </summary>
	/// <param name="Filename"></param>
	/// <returns></returns>
	/// <exception cref="ApplicationException"></exception>
	public Size GetJpegDimensions(string Filename)
	{
		FileStream stream = null;
		BinaryReader rdr = null;
		try
		{
			stream = System.IO.File.OpenRead(Filename);
			rdr = new BinaryReader(stream);
			// keep reading packets until we find one that contains Size info
			for (; ; )
			{
				byte code = rdr.ReadByte();
				if (code != 0xFF) throw new ApplicationException(
						   "Unexpected value in file " + Filename);
				code = rdr.ReadByte();
				switch (code)
				{
					// filler byte
					case 0xFF:
						stream.Position--;
						break;
					// packets without data
					case 0xD0:
					case 0xD1:
					case 0xD2:
					case 0xD3:
					case 0xD4:
					case 0xD5:
					case 0xD6:
					case 0xD7:
					case 0xD8:
					case 0xD9:
						break;
					// packets with size information
					case 0xC0:
					case 0xC1:
					case 0xC2:
					case 0xC3:
					case 0xC4:
					case 0xC5:
					case 0xC6:
					case 0xC7:
					case 0xC8:
					case 0xC9:
					case 0xCA:
					case 0xCB:
					case 0xCC:
					case 0xCD:
					case 0xCE:
					case 0xCF:
						ReadBEUshort(rdr);
						rdr.ReadByte();
						ushort h = ReadBEUshort(rdr);
						ushort w = ReadBEUshort(rdr);
						return new System.Drawing.Size(w, h);
					// irrelevant variable-length packets
					default:
						int len = ReadBEUshort(rdr);
						stream.Position += len - 2;
						break;
				}
			}
		}
		finally
		{
			if (rdr != null) rdr.Close();
			if (stream != null) stream.Close();
		}
	}

	private long GetFileLength(string FtpHostServer, string Username, string Password, string FtpFilePath, bool UsePassive = true)
	{
		string RemoteURL = "ftp://" + FtpHostServer + "/" + FtpFilePath;

		FtpWebRequest requestServerTest = (FtpWebRequest)WebRequest.Create(FtpHostServer);
		requestServerTest.Credentials = new NetworkCredential(Username, Password);
		requestServerTest.Method = WebRequestMethods.Ftp.ListDirectory;
		requestServerTest.UsePassive = UsePassive;

		FtpWebResponse ServerResponse = (FtpWebResponse)requestServerTest.GetResponse();
		//TODO: Update using new code pattern:
		//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
		//var msg = string.Format("");
		// _logger.LogInformation($"{DefaultLogPrefix}.Files.GetFileLength : Server Test Response=" + ServerResponse.ToString());


		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(RemoteURL);
		//TODO: Update using new code pattern:
		//var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
		//var msg = string.Format("");
		// _logger.LogInformation($"{DefaultLogPrefix}.Files.GetFileLength : RequestUri=" + request.RequestUri);
		request.KeepAlive = false;
		request.UsePassive = UsePassive;
		request.Credentials = new NetworkCredential(Username, Password);
		request.Method = WebRequestMethods.Ftp.GetFileSize;

		FtpWebResponse lengthResponse = (FtpWebResponse)request.GetResponse();
		long length = lengthResponse.ContentLength;
		lengthResponse.Close();
		return length;

	}

	private ushort ReadBEUshort(BinaryReader rdr)
	{
		ushort hi = rdr.ReadByte();
		hi <<= 8;
		ushort lo = rdr.ReadByte();
		return (ushort)(hi | lo);
	}

	#endregion

	#region MapPath

	/// <summary>
	/// Changes a mapped path into a virtual path
	/// </summary>
	/// <param name="MappedPath"></param>
	/// <returns></returns>
	public string UnMapPath(string MappedPath)
	{

		string RootMapPath;
		var isMappable = TryGetMappedPath("/", out RootMapPath);
		string VirtualPath = "";

		VirtualPath = MappedPath.ToLower(); //start with the provided MappedPath, standardized to lowercase to make replacing easy.
		VirtualPath = VirtualPath.Replace(RootMapPath.ToLower(), ""); //Get rid of the portion to the website root

		string BackslashChar = @"\";
		VirtualPath = VirtualPath.Replace(BackslashChar, "/"); //flip the slashes

		return VirtualPath;
	}


	/// <summary>
	/// Tries to get a mapped path of the provided path
	/// </summary>
	/// <param name="MappedOrRelativePath">The Path to look for</param>
	/// <param name="MappedFolderPath">The Mapped path, returned</param>
	/// <returns>False, if an exception occurred</returns>
	public bool TryGetMappedPath(string MappedOrRelativePath, out string MappedFolderPath)
	{
		MappedFolderPath = "";
		var status = TryGetMappedPathWithStatus(MappedOrRelativePath, out MappedFolderPath);
		return status.Success;

	}

	/// <summary>
	/// Tries to get a mapped path of the provided path
	/// </summary>
	/// <param name="MappedOrRelativePath">The Path to look for</param>
	/// <param name="MappedFolderPath">The Mapped path, returned</param>
	/// <returns>StatusMessage with information about the operation</returns>
	public StatusMessage TryGetMappedPathWithStatus(string MappedOrRelativePath, out string MappedFolderPath)
	{
		var status = new StatusMessage();
		MappedFolderPath = "";
		if (MappedOrRelativePath != null)
		{
			try
			{
				if (MappedOrRelativePath.Contains("\\"))
				{
					//Path is already a physical path - return it.
					MappedFolderPath = MappedOrRelativePath;
					status.Message = $"'{MappedOrRelativePath}' is already a physical path.";
					status.Success = true;
				}
				else
				{
					//Try to Map it
					MappedFolderPath = _webHostEnvironment.MapPathWebRoot(MappedOrRelativePath);
					status.Message = $"'{MappedOrRelativePath}' maps to '{MappedFolderPath}'";
					status.Success = true;
				}
			}
			catch (ArgumentException ae)
			{
				if (ae.Message.Contains("The path appears to already be fully qualified."))
				{
					MappedFolderPath = MappedOrRelativePath;
					status.Message = $"'{MappedOrRelativePath}' appears to already be fully qualified.";
					status.Success = true;
				}
			}
			catch (Exception ex)
			{
				status.SetRelatedException(ex);

				MappedFolderPath = MappedOrRelativePath;
				status.Success = false;
				status.Message = $"An error occurred and MappedOrRelativePath '{MappedOrRelativePath}' doesn't start with '~' or '/'";
			}
		}

		return status;
	}

	#endregion

}
