using UnityEngine;
using System.Collections;

public class MSLabel : UILabel 
{
	[SerializeField] Color shadowColor;
	[SerializeField] Color outlineColor;

	[SerializeField] Vector2 shadowDist;
	[SerializeField] Vector2 outlineDist;

	public override void OnFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (!isValid) return;
		
		int offset = verts.size;
		Color col = color;
		col.a = finalAlpha;
		
		if (mFont != null && mFont.premultipliedAlphaShader) col = NGUITools.ApplyPMA(col);
		
		string text = processedText;
		int start = verts.size;
		
		UpdateNGUIText();
		
		NGUIText.tint = col;
		NGUIText.Print(text, verts, uvs, cols);
		
		// Center the content within the label vertically
		Vector2 pos = ApplyOffset(verts, start);
		
		// Effects don't work with packed fonts
		if (mFont != null && mFont.packedFontShader) return;
		
		// Apply an effect if one was requested
		if (effectStyle != Effect.None)
		{
			if (((int)effectStyle & (int)Effect.Shadow) > 0)
			{
				int end = verts.size;
				pos.x = shadowDist.x;
				pos.y = shadowDist.y;
				
				ApplyShadow(verts, uvs, cols, offset, end, pos.x, -pos.y, shadowColor);
			}
			
			if (((int)effectStyle & (int)Effect.Outline) > 0)
			{
				int end = verts.size;
				pos.x = outlineDist.x;
				pos.y = outlineDist.y;

				ApplyShadow(verts, uvs, cols, offset, end, pos.x, -pos.y, outlineColor);

				offset = end;
				end = verts.size;
				
				ApplyShadow(verts, uvs, cols, offset, end, -pos.x, pos.y, outlineColor);
				
				offset = end;
				end = verts.size;
				
				ApplyShadow(verts, uvs, cols, offset, end, pos.x, pos.y, outlineColor);
				
				offset = end;
				end = verts.size;
				
				ApplyShadow(verts, uvs, cols, offset, end, -pos.x, -pos.y, outlineColor);
			}
		}
	}

	void ApplyShadow (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, int start, int end, float x, float y, Color mEffectColor)
	{
		Color c = mEffectColor;
		c.a *= finalAlpha;
		Color32 col = (bitmapFont != null && bitmapFont.premultipliedAlphaShader) ? NGUITools.ApplyPMA(c) : c;
		
		for (int i = start; i < end; ++i)
		{
			verts.Add(verts.buffer[i]);
			uvs.Add(uvs.buffer[i]);
			cols.Add(cols.buffer[i]);
			
			Vector3 v = verts.buffer[i];
			v.x += x;
			v.y += y;
			verts.buffer[i] = v;
			cols.buffer[i] = col;
		}
	}

}
