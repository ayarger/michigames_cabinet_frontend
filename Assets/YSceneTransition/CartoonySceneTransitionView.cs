using UnityEngine;
using System.Collections;

public class CartoonySceneTransitionView : TransitionView {

	private Material mat;
	public float intensity = 0.5f;
	public float weird = 0.0f;
	public ParticleSystem ps;

	public Texture symbol_tex;

	ParticleSystem.Particle[] m_Particles;

	// Use this for initialization
	void Awake () {
		//mat = new Material( Shader.Find("Hidden/CartoonySceneTransitionView") );
	}

	public override void TransitionViewStart () {
		//YAudioManager.RegisterBeatCallback (OnBeat);
	}

	void OnDestroy() {
		//YAudioManager.RemoveBeatCallback (OnBeat);
	}

	bool b = false;
	void OnBeat() {
		if (ps == null)
			return;
		
		InitializeIfNeeded();

		// GetParticles is allocation free because we reuse the m_Particles buffer between updates
		int numParticlesAlive = ps.GetParticles(m_Particles);

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{
			if((b && i % 2 == 0) || (!b && i % 2 == 1))
				m_Particles [i].startSize = 4.0f;
		}
		b = !b;

		// Apply the particle changes to the particle system
		ps.SetParticles(m_Particles, numParticlesAlive);
	}

	private void LateUpdate()
	{
		if (ps == null)
			return;
		
		InitializeIfNeeded();

		// GetParticles is allocation free because we reuse the m_Particles buffer between updates
		int numParticlesAlive = ps.GetParticles(m_Particles);

		// Change only the particles that are alive
		for (int i = 0; i < numParticlesAlive; i++)
		{
			m_Particles [i].startSize += (1.0f - m_Particles [i].startSize) * 0.05f;
		}

		// Apply the particle changes to the particle system
		ps.SetParticles(m_Particles, numParticlesAlive);
	}

	float ApplyEase(float val) {
		// Formula:
		// http://math.stackexchange.com/questions/1917471/easing-function-ease-out-in-opposite-of-ease-in-out
		val = Mathf.Clamp01 (val);
		float m0 = 1.75f; 
		float m1 = 1.75f;

		return (Mathf.Pow (val, 3) - 2 * Mathf.Pow (val, 2) + val) * m0
			+ (-2 * Mathf.Pow (val, 2) + 3 * Mathf.Pow (val, 2))
			+ (Mathf.Pow (val, 3) - Mathf.Pow (val, 2)) * m1;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (mat == null)
			return;
		
		mat.SetFloat ("_bwBlend", intensity);
		mat.SetFloat ("_weird", weird);
		mat.SetTexture ("_SymbolTex", symbol_tex);

		float progress_param = YSceneTransitionManager.GetProgress ();
		if (YSceneTransitionManager.GetState () == SceneTransitionState.TO_CHANGE)
			progress_param = 1.0f - progress_param;

		mat.SetFloat ("_progress", ApplyEase(progress_param));
		Graphics.Blit (source, destination, mat);
	}

	void InitializeIfNeeded()
	{
		if (m_Particles == null || m_Particles.Length < ps.maxParticles)
			m_Particles = new ParticleSystem.Particle[ps.maxParticles]; 
	}
}
