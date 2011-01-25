using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Xml;
using System.Windows;

namespace Coding4Fun.CurrencyExchange.Helpers
{
	public class StorageHelper
	{
		public static T LoadXml<T>(string fileName) where T : class, new()
		{
			return LoadContract<T>(fileName, false);
		}

		public static T LoadContract<T>(string fileName, bool useBinary) where T : class, new()
		{
			T loadedObject = null;

			try
			{
				using (var storageFile = IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (var stream = new IsolatedStorageFileStream(fileName, FileMode.OpenOrCreate, storageFile))
					{
						using (var reader = (useBinary
												? XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max)
												: XmlReader.Create(stream)))
						{
							if (stream.Length > 0)
							{
								var serializer = new DataContractSerializer(typeof(T));
								loadedObject = serializer.ReadObject(reader) as T;
							}
						}
						stream.Close();
					}
				}
			}
			catch
			{
				MessageBox.Show("Sorry, but something went wrong and I can't retrieve your saved settings.");

				if (IsolatedStorageFile.GetUserStoreForApplication().FileExists(fileName))
					IsolatedStorageFile.GetUserStoreForApplication().DeleteFile(fileName);
			}

			return loadedObject ?? new T();
		}

		public static void SaveXml<T>(string fileName, T objectToSave)
		{
			SaveContract(fileName, objectToSave, false);
		}

		public static void SaveContract<T>(string fileName, T objectToSave, bool useBinary)
		{
			try
			{
				using (var storageFile = IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (var stream = new IsolatedStorageFileStream(fileName, FileMode.Create, storageFile))
					{
						using (var writer =
							(useBinary ? XmlDictionaryWriter.CreateBinaryWriter(stream) : XmlWriter.Create(stream)))
						{
							var serializer = new DataContractSerializer(typeof(T));
							serializer.WriteObject(writer, objectToSave);
							writer.Flush();
						}
					}
				}
			}
			catch (Exception x)
			{
				MessageBox.Show("Sorry, but something went wrong and I can't save your settings.\n" + x.Message);

				if (IsolatedStorageFile.GetUserStoreForApplication().FileExists(fileName))
					IsolatedStorageFile.GetUserStoreForApplication().DeleteFile(fileName);
			}
		}
	}
}