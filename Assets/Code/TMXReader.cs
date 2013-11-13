using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.IO;

public class TMXReader {

	XmlDocument xmlDoc;

	const string WALKABLE_LAYER = "Walkable";
	const string LAYER = "layer";

	public TMXReader(string filename)
	{
		Debug.Log("Loading reader for tmx: " + filename);
		TextAsset text = Resources.Load(CBKUtil.StripExtensions(filename)) as TextAsset;
		xmlDoc = new XmlDocument();
		xmlDoc.LoadXml (text.ToString());
	}

	public Stream GetWalkableData()
	{
		XmlNodeList layers = xmlDoc.GetElementsByTagName(LAYER);
		foreach (XmlNode item in layers) 
		{
			if (item.Attributes["name"].Value == WALKABLE_LAYER)
			{
				Stream data = new MemoryStream(Convert.FromBase64String(item.InnerText));
				data = new Ionic.Zlib.ZlibStream(data, Ionic.Zlib.CompressionMode.Decompress, false);

				return data;
			}
		}
		return null;
	}
}
