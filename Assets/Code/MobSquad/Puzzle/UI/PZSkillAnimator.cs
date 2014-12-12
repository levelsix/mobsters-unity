using UnityEngine;
using System.Collections;
using com.lvl6.proto;

public class PZSkillAnimator : MonoBehaviour {

	[SerializeField] UI2DSprite character;
	[SerializeField] UI2DSprite logo;
	TweenPosition tPos;
	TweenAlpha tAlph;

	[SerializeField] float pauseTime;

	public Coroutine AnimateDefensive(PZCombatUnit unit)
	{
		return Animate(unit.monster.monster, unit.monster.defensiveSkill);
	}

	public Coroutine AnimateOffensive(PZCombatUnit unit)
	{
		return Animate(unit.monster.monster, unit.monster.offensiveSkill);
	}

	public Coroutine Animate(MonsterProto monster, SkillProto skill)
	{
		gameObject.SetActive(true);
		return StartCoroutine(DoAnimation(monster, skill));
	}

	IEnumerator DoAnimation(MonsterProto monster, SkillProto skill)
	{
		if (tPos == null) tPos = GetComponent<TweenPosition>();
		if (tAlph == null) tAlph = GetComponent<TweenAlpha>();

		MSSpriteUtil.instance.SetSprite(monster.imagePrefix, monster.imagePrefix + "Character", character);
		MSSpriteUtil.instance.SetSprite(skill.iconImgName, skill.iconImgName + "logo", logo);

		tPos.Sample(0, true);
		tAlph.Sample(0, true);

		tPos.PlayForward();
		tAlph.PlayForward();

		while (tPos.tweenFactor < 1)
		{
			yield return null;
		}
		yield return new WaitForSeconds(pauseTime);
		tPos.PlayReverse();
		tAlph.PlayReverse();
		while (tPos.tweenFactor > 0)
		{
			yield return null;
		}

		gameObject.SetActive(false);
	}
}
