using UnityEngine;
using System.Collections;

[RequireComponent (typeof (UILabel))]
public class MSHintLabel : MonoBehaviour {

	UILabel label;

	const string MONSTER_NAME = "mobster";

	public static string[] hints = 
	{
		"Need more cash? Upgrade your Cash Printers.",
		"Need more oil? Upgrade your Oil Drills.",
		"Click on the element tree in battle to see which elements prevail over others.",
		"Upgrade your hospitals to heal your toons faster.",
		"Match 5 orbs in a line to create a rainbow power-up.",
		"Match 5 orbs in an L or T shape to create a grenade power-up.",
		"Combine a grenade and a rocket orb to activate a triple powered rocket.",
		"Make sure your team is always equipped with 3 players to defend against attackers.",
		"Remove obstacles to make room for new buildings in your base.",
		"Recruit scientists from daily events and use them to evolve new toon forms.",
		"You need 2 max level toons to evolve to their next form.",
		"Upgrade your Enhancement lab to speed up enhancing.",
		"Use the Grab machines to find exciting and powerful toons.",
		"Watch out for Lil’ Kim. He’s a bad apple.",
		"The mail popup shows you info about your recent attacks.",
		"Complete Achievements to earn free Purple Gems.",
		"An active shield protects you from being attacked.",
		"In multiplayer battles toons always start at full health.",
		"Join a Squad to to chat and play with others from around the world.",
		"Night and Day toons are powerful against each other.",
		"Rock orbs are neutral.",
		"Water, Fire, and Earth elements work like Rock, Paper, Scissors.",
		"Click on a player in chat to see their profile or send them a private message.",
		"Upgrade or build more hospitals to increase the number of toons they hold.",
		"You can play single player missions without losing your shield.",
		"Upgrade your storages to increase your max Cash and Oil.",
		"Enhancing and evolving toons is important for crafting a strong team.",
		"Find all of a toon’s pieces? See how long it will take to complete in your Team Center.",
		"Tap on the heart in the upper left of a toon’s info page to make it your avatar.",
		"Tap on your level icon to see your profile.",
		"Creating a Squad is easy… recruiting members for it is hard.",
		"Upgrade your Command Center to increase the max level of your buildings.",
		"Complete Mini Jobs from your pier to earn extra Cash, Oil, or even free Purple Gems.",
		"All toons are grouped into Common, Rare, Super, Ultra, Epic, and Legendary.",
		"Muting someone in chat lasts for 24 hours.",
		"Hire Facebook friends as workers in your residence’s to earn bonus slots.",
		"Upgrade your residences to make room for more toons.",
		"Don’t want to sell your toons? Use them to enhance another!",
		"Always keep your builder busy by upgrading your buildings.",
		"Check the shop for new buildings after upgrading your Command Center."
	};

	void OnEnable()
	{
		if (label == null)
		{
			label = GetComponent<UILabel>();
		}

		label.text = hints[Random.Range(0, hints.Length)];
	}
}
