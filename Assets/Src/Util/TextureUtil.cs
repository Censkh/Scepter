using UnityEngine;
using System.IO;

class TextureUtil
{

	public static void SaveTextureToFile(Texture2D texture, string fileName)
	{
		byte[] bytes = texture.EncodeToPNG();
		FileStream file = File.Open(Application.dataPath + "/" + fileName, FileMode.Create);
		BinaryWriter binary = new BinaryWriter(file);
		binary.Write(bytes);
		file.Close();
	}

}