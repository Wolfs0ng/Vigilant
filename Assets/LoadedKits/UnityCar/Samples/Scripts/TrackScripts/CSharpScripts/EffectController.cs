using UnityEngine;

public class EffectController : MonoBehaviour
{
	public Generate2DReflection generate2dReflection;
	public GlowEffectThreshold_26 glowEffect;
	
	void Update()
	{
		if(QualitySettings.GetQualityLevel() < 3) // QualityLevel.Good
		{
			if (generate2dReflection)
				generate2dReflection.enabled = false;
			if (glowEffect)
				glowEffect.enabled = false;
		}
		else
		{
			if (generate2dReflection)
				generate2dReflection.enabled = true;
			if (glowEffect)
				glowEffect.enabled = true;
		}
	}
}
