using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using System.Threading;
using System.Net.Sockets;
using SimpleJSON;
using InControl;
using System.Net;

public enum SelectorState {
    GRID, CONFIRM, TRANSITION, IDLING
};

public class GameManager : MonoBehaviour {
    static GameManager _instance;
    public SelectorState _state = SelectorState.GRID;
    Process _game_process;
    static float playtime = 0;
    public static SelectorState GetSelectorState() {
        return _instance._state;
    }
    public static SelectableCell GetFocusedCell() {
        return _instance.cells [(int)_instance.current_cursor.y] [(int)_instance.current_cursor.x];
    }

    public float transition_progress = 0.0f;
    public static float GetTransitionProgress () {
        return Mathf.Clamp01 (_instance.transition_progress);
    }

    public Camera main_camera;

    public List<GameInfo> game_infos = new List<GameInfo>();

    List<List<SelectableCell>> cells = new List<List<SelectableCell>>();

    public List<Sprite> test_icons = new List<Sprite>();

    public Text game_title_text;
    public Text dev_names_text;
    public Text player_numbers_text;
    public Text logline_text;
    public Image event_icon;
    public GameObject normal_paper;
    public GameObject credits_paper;
    public List<Image> player_number_images = new List<Image>();

    public Image white_board;

    public List<Sprite> manually_prepared_icon_list = new List<Sprite>(); 

    public AudioClip boop;

    public AudioClip select_clip;
    public AudioClip deselect_clip;
    public AudioClip confirm_clip;

    public Vector3 grid_offset = Vector3.zero;
    public float icon_margin = 1.0f;

    public Sprite settings_icon;
    public Sprite credits_icon;

    public Sprite eecs_494_sprite;
    public Sprite wsoft_sprite;

	public Image LaunchPanel;
	public Text LaunchText1;
	public Text LaunchText2;

    public Text play_counter_text;

	public Color PinkColor;
	public Color LemonColor;
	public Color AquaColor;
	public Color OrangeColor;
	public Color SourAppleColor;

	public Image TransitionPanel;

    Vector2 current_cursor = Vector2.zero;

    public GameObject object_icon_prefab;

    public static bool sent_report = false;

    void Init() {
		string[] game_paths;
		try {
        	game_paths = Directory.GetDirectories ("./michigames_games_repo");
		} catch(Exception e) {
			ErrorUI.ShowMessage ("Failed to locate games directory");
			return;
		}
			
        foreach (string s in game_paths) {
            AddGame (s);
        }

        print (Directory.GetCurrentDirectory());

        Screen.SetResolution(1920, 1080, true);
        //Screen.SetResolution (1280, 768, true);
    }

    int icon_number = 0;

    void AddGame(string path) {
        print ("Adding game at path: " + path);
        string config_path = path + "/config.json";
        string icon_path = path + "/icon.png";
        string screenshots_folder_path = path + "/screenshots";
        string build_path = path + "/build.exe";

        string config_data = System.IO.File.ReadAllText (config_path);
        JSONNode root = JSON.Parse (config_data);

        GameInfo new_game_info = new GameInfo ();

        new_game_info.game_title = root ["game_title"];
        new_game_info.genres = root ["genres"];
        new_game_info.dev_names = root ["developer_names"];

        // Icon
        byte[] data = File.ReadAllBytes (icon_path);
        Texture2D tex = new Texture2D (120, 120, TextureFormat.ARGB4444, true);
        tex.LoadImage (data);
        tex.name = Path.GetFileNameWithoutExtension (icon_path);
        //new_game_info.icon = Sprite.Create (tex, new Rect (Vector2.zero, Vector2.one * 120), new Vector2 (0.5f, 0.5f));
        UnityEngine.Debug.Log(icon_number);
        icon_number = Mathf.Min(icon_number, 8);
        new_game_info.icon = manually_prepared_icon_list[icon_number++];

        // Screenshots
        string[] screenshot_paths = Directory.GetFiles (screenshots_folder_path);
        foreach (string screen_path in screenshot_paths) {
            if (Path.GetExtension(screen_path) != ".png" && Path.GetExtension(screen_path) != ".PNG")
                continue;
            byte[] screen_data = File.ReadAllBytes (screen_path);
            Texture2D screen_tex = new Texture2D (384, 240, TextureFormat.ARGB32, false);
            screen_tex.LoadImage (screen_data);
            screen_tex.name = Path.GetFileName (screen_path);
            
            //new_game_info.icon = Sprite.Create (screen_tex, new Rect (Vector2.zero, new Vector2 (384, 240)), Vector2.zero);
            new_game_info.screenshots.Add(screen_tex);
        }

        new_game_info.game_id = root ["game_id"];
        new_game_info.logline = root ["logline"];
        new_game_info.max_number_players = root["max_number_players"].AsInt;
        new_game_info.min_number_players = root ["min_number_players"].AsInt;
        new_game_info.path_to_executable = build_path;
        new_game_info.trailer_video_path = "";
        new_game_info.semester_or_event_code = root["semester_or_event_code"];
        new_game_info.add_date = root["add_date"];
		new_game_info.paper_color = GetColorFromColorName (root ["paper_color"]);
        LoadPlayCounts(new_game_info);
        game_infos.Add (new_game_info);
    }

	Color GetColorFromColorName(string cn) {
		if (cn == "pink")
			return PinkColor;
		if (cn == "lemon")
			return LemonColor;
		if (cn == "aqua")
			return AquaColor;
		if (cn == "orange")
			return OrangeColor;
		if (cn == "sour_apple")
			return SourAppleColor;

		return Color.white;
	}
        
    // Use this for initialization
    void Start () {
        if (_instance != null && _instance != this) {
            UnityEngine.Debug.LogError ("Singleton Violation: GameManager");
            Destroy (gameObject);
            return;
        } else {
            _instance = this;
        }

        Application.runInBackground = true;
        //cells.Clear ();

        Init ();
        Cursor.visible = false;

        //current_cursor = new Vector2 (UnityEngine.Random.Range (0, 4), UnityEngine.Random.Range (0, 4));
        current_cursor = Vector2.zero;
        List<SelectableCell> new_list = null;
        for (int i = 0; i < game_infos.Count; i++) {
            int x = i % 4;
            int y = i / 4;

            if (i % 4 == 0) {
                new_list = new List<SelectableCell>();
                new_list.Clear ();
                cells.Add (new_list);
            }

            SelectableCell cell = new SelectableCell ();

            if (y == current_cursor.y && x == current_cursor.x)
                cell.is_selected = true;

            cell.coords = new Vector2 (x, y);

            cell.game_info = game_infos [i];
            cell.ob = Instantiate (object_icon_prefab, Vector3.zero, Quaternion.identity) as GameObject;
            cell.ob.GetComponent<ObjectIcon> ().SetSelectableCell (cell);
            cell.ob.GetComponent<ObjectIcon>().main_camera = main_camera;
            cell.initial_desired_position = new Vector3 (x * icon_margin + grid_offset.x, y * icon_margin +grid_offset.y, 0+grid_offset.z);
            new_list.Add (cell);
        }
        /*
        // Settings Icon
        SelectableCell settings_cell = new SelectableCell ();
        settings_cell.coords = new Vector2 (4, 0);
        settings_cell.icon = settings_icon;
        settings_cell.ob = Instantiate (object_icon_prefab, Vector3.zero, Quaternion.identity) as GameObject;
        settings_cell.ob.GetComponent<ObjectIcon> ().SetSelectableCell (settings_cell);
        settings_cell.ob.GetComponent<ObjectIcon> ().SetDesiredPosition(new Vector3 (4 * icon_margin + grid_offset.x, 0 * icon_margin +grid_offset.y, 0+grid_offset.z));
        cells [0].Add (settings_cell);

        // Credits Icon
        SelectableCell credits_cell = new SelectableCell ();
        credits_cell.coords = new Vector2 (4, 1);
        credits_cell.icon = credits_icon;
        credits_cell.ob = Instantiate (object_icon_prefab, Vector3.zero, Quaternion.identity) as GameObject;
        credits_cell.ob.GetComponent<ObjectIcon> ().SetSelectableCell (credits_cell);
        credits_cell.ob.GetComponent<ObjectIcon> ().SetDesiredPosition(new Vector3 (4 * icon_margin + grid_offset.x, 1 * icon_margin +grid_offset.y, 0+grid_offset.z));
        cells [1].Add (credits_cell);
        */

        RefreshUI ();
    }
        
    // Update is called once per frame
    void Update () {

        ConsiderSendingReport();
        //print ("watchdog: " + WatchdogManager.GetWatchdogTimer().ToString());

        if (_state == SelectorState.GRID)
            ProcessGrid ();
        else if (_state == SelectorState.CONFIRM)
            ProcessConfirm ();
        else if (_state == SelectorState.TRANSITION)
            ProcessTransition ();
        else if (_state == SelectorState.IDLING)
            ProcessIdling ();

        var inputDevice = InputManager.ActiveDevice;

        white_board.color = new Color(1, 1, 1, transition_progress);

        if (GetSelectorState() == SelectorState.GRID || GetSelectorState() == SelectorState.CONFIRM)
        {
            
            if (transition_progress > 0)
                transition_progress -= 0.02f;
        }

        playtime += Time.time;
    }

    void ConsiderSendingReport()
    {
        // Send report to server once we pass 11:45pm.
        if (DateTime.Now.TimeOfDay > new TimeSpan(23, 45, 0) && !sent_report)
        {
            ReportToServer();
            sent_report = true;
        }
    }

    void EnforceResolution()
    {
        Screen.SetResolution(1280, 768, true);
    }
        
    void ProcessGrid () {
        Vector2 movement = Vector2.zero;
        InputManager.ActiveDevice.LeftStick.LowerDeadZone = 0.25f;

		TransitionPanel.gameObject.SetActive (false);

        // Keyboard
        if (Input.GetKeyDown (KeyCode.RightArrow))
            movement = new Vector2 (1, 0);
        else if (Input.GetKeyDown (KeyCode.LeftArrow))
            movement = new Vector2 (-1, 0);
        else if (Input.GetKeyDown (KeyCode.UpArrow))
            movement = new Vector2 (0, 1);
        else if (Input.GetKeyDown (KeyCode.DownArrow))
            movement = new Vector2 (0, -1);

        // Controllers
        float last_horizontal_value = Mathf.Abs(InputManager.ActiveDevice.Direction.LastValue.x);
        float last_vertical_value = Mathf.Abs(InputManager.ActiveDevice.Direction.LastValue.y);

        if (InputManager.ActiveDevice.Direction.Value.x >= 0.5f && last_horizontal_value < 0.5f)
            movement = new Vector2(1, 0);
        else if (InputManager.ActiveDevice.Direction.Value.x <= -0.5f && last_horizontal_value < 0.5f)
            movement = new Vector2(-1, 0);
        else if (InputManager.ActiveDevice.Direction.Value.y >= 0.5f && last_vertical_value < 0.5f)
            movement = new Vector2(0, 1);
        else if (InputManager.ActiveDevice.Direction.Value.y <= -0.5f && last_vertical_value < 0.5f)
            movement = new Vector2(0, -1);
        
        // Apply Movement
        MoveCursor((int)movement.x, (int)movement.y);

        // Return
		if (Input.GetKeyDown (KeyCode.Return) || InputManager.ActiveDevice.Action1.WasPressed) {
			SelectableCell cell = cells [(int)current_cursor.y] [(int)current_cursor.x];
			if (cell.game_info.game_title == "Credits")
				return;
			_state = SelectorState.CONFIRM;
			StartCoroutine (short_rumble());
            AudioSource.PlayClipAtPoint(select_clip, main_camera.transform.position);
            //LaunchPanel.gameObject.SetActive (true);
            if (cell.game_info.semester_or_event_code.Length == 3)
				LaunchText1.text = "This game was created in EECS 494.";
			else 
				LaunchText1.text = "This game was created at a Wolverine Soft Game Jam in 48 Hours.";
			LaunchText2.text = "Play \"" + cell.game_info.game_title + "\"?";
		}
    }

    public static bool IsGameRunning () {
        return _instance._game_process != null && !_instance._game_process.HasExited;
    }

    void ProcessConfirm () {
		if (Input.GetKeyDown (KeyCode.Return) || InputManager.ActiveDevice.Action1.WasPressed) {
            _state = SelectorState.TRANSITION;
            normal_paper.SetActive(false);
            AudioSource.PlayClipAtPoint(confirm_clip, main_camera.transform.position);
			StartCoroutine (short_rumble());
			//LaunchPanel.gameObject.SetActive (false);
			//TransitionPanel.gameObject.SetActive (true);
		} else if (Input.GetKeyDown (KeyCode.Backspace) || InputManager.ActiveDevice.Action2.WasPressed) {
            _state = SelectorState.GRID;
            AudioSource.PlayClipAtPoint(deselect_clip, main_camera.transform.position);
            //LaunchPanel.gameObject.SetActive (false);
        }
    }

    void ProcessTransition() {

        Vector3 desired_cam_position = GetFocusedCell().ob.transform.position - Vector3.forward * (8 - (transition_progress * 6));
        main_camera.transform.position += (desired_cam_position - main_camera.transform.position) * 0.1f;

        if (transition_progress >= 1.0f) {
            if (!IsGameRunning()) {
                //WatchdogManager.WatchdogCheckin ();
                LaunchGame ();
                transition_progress = 1.0f;
                
            }
        } else {
			transition_progress += 0.0075f * Time.deltaTime * 50;
        }
    }

    void ProcessIdling() {
        // Monitor watchdog while the game plays.
        //bool should_terminate_game = WatchdogManager.GetWatchdogTimer() > WatchdogManager.GetWatchdogDuration();
        //if (should_terminate_game) {
		if (_game_process.HasExited) {
			//_game_process.Kill ();
			ReturnToSelector ();
            
        }
        //}
    }

    void ReturnToSelector() {
        normal_paper.SetActive(true);
        print ("RETURNING TO SELECTOR!");
        cells[(int)current_cursor.y][(int)current_cursor.x].game_info.playtime_sec_delta += (int)(playtime / 1000);
        playtime = 0.0f;
        _state = SelectorState.GRID;
    }

    public void MoveCursor(int x, int y) {
        if (x == 0 && y == 0)
            return;

        if (current_cursor.x + x >= cells [(int)current_cursor.y].Count)
            return;
        if (current_cursor.x + x < 0)
            return;
        if (current_cursor.y + y >= cells.Count)
            return;
        if (current_cursor.y + y < 0)
            return;
        if (cells [(int)(current_cursor.y + y)].Count <= current_cursor.x)
            return;
        if (cells [(int)(current_cursor.y + y)] [(int)(current_cursor.x + x)] == null)
            return;

		AudioSource.PlayClipAtPoint (boop, main_camera.transform.position);

        ResetCells ();
        current_cursor += new Vector2 (x, y);
        cells [(int)current_cursor.y] [(int)current_cursor.x].is_selected = true;
        //print ("man: " + cells [(int)current_cursor.y] [(int)current_cursor.x].is_selected.ToString ());

		StartCoroutine (short_rumble());

        RefreshUI ();
    }

	IEnumerator short_rumble() {
		InputDevice dev = InputManager.ActiveDevice;
		dev.Vibrate (1.5f);
		yield return new WaitForSeconds (0.1f);
		dev.Vibrate (0.0f);
	}

    void RefreshUI() {
        SelectableCell cell = cells [(int)current_cursor.y] [(int)current_cursor.x];
        game_title_text.text = cell.game_info.game_title;
        dev_names_text.text = cell.game_info.dev_names;
        player_numbers_text.text = GetPlayerNumbersText(cell);
        logline_text.text = cell.game_info.logline;

        for (int i = 0; i < 4; i++)
            player_number_images [i].enabled = true;

        for (int i = cell.game_info.max_number_players; i < 4; i++)
            player_number_images [i].enabled = false;

        PreviewPanel.SetGameInfo (cell.game_info);
        
        if(cell.game_info.semester_or_event_code.Length == 3)
        {
            event_icon.sprite = eecs_494_sprite;
            event_icon.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
        } else
        {
            event_icon.sprite = wsoft_sprite;
            event_icon.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
        }

        if (GetSelectorState() != SelectorState.TRANSITION && GetSelectorState() != SelectorState.IDLING)
        {
            if (cell.game_info.game_title == "Credits")
            {
                normal_paper.SetActive(false);
                credits_paper.SetActive(true);
            }
            else
            {
                normal_paper.SetActive(true);
                credits_paper.SetActive(false);
            }
        }

		normal_paper.GetComponent<Image> ().color = cell.game_info.paper_color;

        Paper.Refresh();

        // PlayCount
        int total_play_count = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < cells[i].Count; j++)
            {
                total_play_count += cells[i][j].game_info.total_play_count;
            }
        }

        play_counter_text.text = total_play_count.ToString();
    }

    string GetPlayerNumbersText(SelectableCell cell)
    {
        int max_players = cell.game_info.max_number_players;
        int min_players = cell.game_info.min_number_players;
        string result = max_players.ToString();

        if (max_players > min_players)
            result = min_players.ToString() + "-" + max_players.ToString();

        return result + " Player";
    }

    public void ResetCells() {
        for (int i = 0; i < cells.Count; i++) {
            for (int j = 0; j < cells [i].Count; j++) {
                cells [i] [j].is_selected = false;
            }
        }
    }

    void LaunchGame() {
        if (IsGameRunning ()) {
            UnityEngine.Debug.LogError ("Attempting to launch game with one already playing.");
            return;
        }

        playtime = 0.0f;
        SelectableCell cell = cells [(int)current_cursor.y] [(int)current_cursor.x];
        string exe_path = Path.GetFullPath(cell.game_info.path_to_executable);
        print ("Launching: " + exe_path);

        _game_process = Process.Start (exe_path);
        _game_process.EnableRaisingEvents = true;
        _game_process.PriorityBoostEnabled = true;
        //_game_process.Exited += ReturnToSelector;
		_state = SelectorState.IDLING;
        IncrementPlayCounter(cell.game_info);
        SavePlayCounts();
        RefreshUI();
    }

    void ReportToServer()
    {
        string json_data = GetReportJson();
        print("REPORTING:::::::::::::::::::::::::::::");
        print(json_data);
        print("REPORTING2:::::::::::::::::::::::::::::");

        WWWForm form = new WWWForm();
        form.AddField("secret", "qLM1Top9MdanhipfCKBc");
        form.AddField("data", json_data);

        WWW request = new WWW("http://www.michigames.org/reporting/record_stats.php", form.data, form.headers);
        StartCoroutine(WaitForWWW(request));
    }

    IEnumerator WaitForWWW(WWW www)
    {
        yield return www;

        string txt = "";
        if (string.IsNullOrEmpty(www.error))
            txt = www.text;  //text of success
        else
            txt = www.error;  //error
        print(txt);
    }

    string GetReportJson()
    {
        string result = "{\"game_deltas\":[";

        List<GameInfo> valid_infos = new List<GameInfo>();
        foreach (GameInfo gi in GameManager._instance.game_infos)
            if (gi.play_count_delta > 0)
                valid_infos.Add(gi);

        for (int i = 0; i < valid_infos.Count; i++)
        {
            GameInfo info = valid_infos[i];

            result += "{\"game_id\":" + info.game_id + ",";
            result += "\"plays\":" + info.play_count_delta.ToString() + ",";
            result += "\"playtime_sec\":" + info.playtime_sec_delta.ToString() + ",";
            if(i == valid_infos.Count-1)
                result += "\"rating\":" + "-1" + "}";
            else
                result += "\"rating\":" + "-1" + "},";
        }

        return result + "]}";
    }

    public static Dictionary<string, Dictionary<DateTime, int>> play_counts = new Dictionary<string, Dictionary<DateTime, int>>();
    void SavePlayCounts()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < cells[i].Count; j++)
            {
                string game_count_id = cells[i][j].game_info.GetPlayCountID();
                Persistence.SetInt(game_count_id, cells[i][j].game_info.total_play_count);
            }
        }
    }

    void LoadPlayCounts(GameInfo gi)
    {
        string game_count_id = gi.GetPlayCountID();
        int count = 0;
        if(Persistence.HasKey(game_count_id))
            count = Persistence.GetInt(game_count_id);
        gi.total_play_count = count;
    }

    void IncrementPlayCounter(GameInfo gi)
    {
        gi.total_play_count ++;
        gi.play_count_delta ++;
    }
}

public class SelectableCell {
    public GameObject ob;
    public bool is_selected = false;
    public Vector2 coords;
    public GameInfo game_info;
    public Vector3 initial_desired_position;
}

[System.Serializable]
public class GameInfo {
    public string game_id;
    public string game_title;
    public string logline;
    public string path_to_executable;
    public string trailer_video_path;
    public Sprite icon;
    public string genres;
    public string dev_names;
    public MovieTexture video_texture;
    public int max_number_players;
    public int min_number_players;
    public string semester_or_event_code;
    public string add_date;
    public List<Texture2D> screenshots = new List<Texture2D>();
	public Color paper_color;
    public int total_play_count = 0;
    public int play_count_delta = 0;
    public int playtime_sec_delta = 0;

    public string GetPlayCountID()
    {
        return game_title + "_" + add_date + "_play_count";
    }
}