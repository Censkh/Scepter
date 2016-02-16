using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

class PropertyFile
{

	public string path;
	private Dictionary<string, string> data = new Dictionary<string, string>();

	public PropertyFile(string path)
	{
		this.path = path;
	}

	public void Load()
	{
		FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
		StreamReader reader = new StreamReader(stream);
		data.Clear();
		string line = null;
		while ((line = reader.ReadLine()) != null)
		{
			if (line.Contains("="))
			{
				int index = line.IndexOf("=");
				string propertyName = line.Substring(0, index);
				string propertyValue = line.Substring(index+1);
				data.Add(propertyName, propertyValue);
			}
		}
		stream.Close();
	}

	public void Save()
	{
		File.WriteAllText(path, String.Empty);
		FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
		StreamWriter writer = new StreamWriter(stream);
		foreach (string propertyName in data.Values)
		{
			string propertyValue = data[propertyName];
			writer.WriteLine(propertyName + "=" + propertyValue);
		}
		stream.Close();
	}

	public void SetProperty(string name, string value)
	{
		data.Add(name, value);
	}

	public string GetProperty(string name)
	{
		if (data.ContainsKey(name))
			return data[name];
		return null;
	}

}