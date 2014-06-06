using GoogleFu;
using UnityEngine;

public enum Language {EN, FR, JA};

public class MSLocalization : MonoBehaviour
{
	public static Language language = Language.FR;
	
	public static string GetString(Sheet1.rowIds rowId)
	{
		return Sheet1.Instance.GetRow(rowId).GetStringData(language.ToString());
	}

	public void ChangeLanguage(Language language)
	{
		MSLocalization.language = language;
		foreach (MSLocalizedLabel label in GameObject.FindObjectsOfType(typeof(MSLocalizedLabel))) 
		{
			label.Refresh();
		}
	}

	[ContextMenu ("Set Language to English")]
	public void English()
	{
		ChangeLanguage(Language.EN);
	}

	[ContextMenu ("Set Language to French")]
	public void French()
	{
		ChangeLanguage(Language.FR);
	}

	[ContextMenu ("Set Language to Japanese")]
	public void Japanese()
	{
		ChangeLanguage(Language.JA);
	}
}
