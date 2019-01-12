using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PreviewPanel : MonoBehaviour {

	private static PreviewPanel instance;

	RawImage raw_image;
	GameInfo current_game_info;

	float screenshot_slideshow_duration = 2.0f;
	float screenshot_slideshow_previous_time;
	int current_screenshot_index = 0;

	void Start () {
		instance = this;
		raw_image = GetComponent<RawImage> ();
        screenshot_slideshow_previous_time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (current_game_info == null)
            return;

		if (IsTrailerAvailable())
			return;

		// Screenshots
		
		if (Time.time - screenshot_slideshow_previous_time >= screenshot_slideshow_duration) {
            screenshot_slideshow_previous_time = Time.time;
			if (current_game_info.screenshots.Count > 0) {
				current_screenshot_index = (++current_screenshot_index) % current_game_info.screenshots.Count;
			}
			RefreshScreenshot ();
		}
	}

	void RefreshScreenshot() {
		if (current_game_info.screenshots.Count > current_screenshot_index)
			raw_image.texture = current_game_info.screenshots [current_screenshot_index];
		raw_image.color = new Color (raw_image.color.r, raw_image.color.g, raw_image.color.b, 1.0f);
	}

	public static void SetGameInfo(GameInfo gi) {
        
        // Screenshots
        instance.current_game_info = gi;
		instance.current_screenshot_index = 0;
        instance.screenshot_slideshow_previous_time = Time.time;
		instance.RefreshScreenshot ();

		// Trailer
		if (!IsTrailerAvailable())
			return;
		instance.raw_image.texture = instance.current_game_info.video_texture;
		MovieTexture m = (MovieTexture)instance.raw_image.texture;
		m.Stop ();
		m.Play ();
		m.loop = true;
		AudioSource.PlayClipAtPoint (m.audioClip, Camera.main.transform.position);
	}

	static bool IsTrailerAvailable() {
		return instance.current_game_info.trailer_video_path != null
			&& instance.current_game_info.trailer_video_path.Length > 0;
	}
}
