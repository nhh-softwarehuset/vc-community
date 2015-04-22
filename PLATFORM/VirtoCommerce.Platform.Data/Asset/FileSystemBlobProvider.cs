﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using VirtoCommerce.Platform.Core.Asset;


namespace VirtoCommerce.Platform.Data.Asset
{
	public class FileSystemBlobProvider : IBlobStorageProvider, IBlobUrlResolver
	{
		private readonly string _storagePath;
		private readonly string _publicUrl;

		public FileSystemBlobProvider(string storagePath, string publicUrl)
		{
			_storagePath = storagePath;
			_publicUrl = publicUrl;

			if (_publicUrl != null && !_publicUrl.EndsWith("/"))
			{
				_publicUrl += "/";
			}
		}

		#region IBlobStorageProvider members

		public async Task<string> UploadAsync(UploadStreamInfo request)
		{
			var task = Task.Run(() => Upload(request));
			return await task;
		}

		public async Task<Stream> OpenReadOnlyAsync(string assetKey)
		{
			var task = Task.Run(() => OpenReadOnly(assetKey));
			return await task;
		}

		public string Upload(UploadStreamInfo request)
		{
			if (request == null)
				throw new ArgumentNullException("request");

			string folderName;
			string fileName;
			string key;
			string storagePath = _storagePath;

			folderName = request.FolderName;
			fileName = request.FileName;
			key = string.Format(@"{0}\{1}", folderName, fileName);
			storagePath = string.Empty;


			if (!string.IsNullOrEmpty(folderName))
				CreateFolder(_storagePath, folderName);

			var storageFileName = string.Format(CultureInfo.CurrentCulture, @"{0}\{1}\{2}", _storagePath, folderName, fileName);

			UpdloadFile(request.FileByteStream, storageFileName);


			return key;

		}

		public Stream OpenReadOnly(string assetKey)
		{
			if (string.IsNullOrEmpty(assetKey))
				throw new ArgumentNullException("assetKey");

			var fileName = _storagePath + "\\" + assetKey;

			var stream = LoadFile(fileName);

			return stream;
		}

		#endregion

		#region IBlobUrlResolver Members

		public string GetAbsoluteUrl(string assetKey)
		{
			return _publicUrl + assetKey;
		}

		#endregion


		private static void UpdloadFile(Stream stream, string filePath)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}

			using (var uploadedFileStream = File.Open(filePath, FileMode.Create))
			{
				var buffer = new byte[256];
				int count;
				while ((count = stream.Read(buffer, 0, 256)) > 0)
					uploadedFileStream.Write(buffer, 0, count);
				uploadedFileStream.Flush();
			}
		}


		private static Stream LoadFile(string filePath)
		{
			Stream copyStream;
			using (var uploadedFileStream = File.Open(filePath, FileMode.Open))
			{
				copyStream = new MemoryStream();
				uploadedFileStream.CopyTo(copyStream);
				copyStream.Seek(0, SeekOrigin.Begin);
			}
			return copyStream;
		}

		private static string CreateFileName()
		{
			return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
						  .Substring(0, 22)
						  .Replace("/", "_")
						  .Replace("+", "-");
		}

		private static string GetFileExtension(string fileName)
		{
			return Path.GetExtension(fileName);
		}

		private static string CreateFolder(string storagePath, string folderName)
		{
			var directory = string.Format(CultureInfo.CurrentCulture, @"{0}\{1}", storagePath, folderName);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			return directory;
		}


	}
}
