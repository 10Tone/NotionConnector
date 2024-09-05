#if TOOLS
using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Notion.Client;

[Tool]
public partial class NotionConnectorPlugin : EditorPlugin
{
	private NotionConnectorDock _dock;
	private NotionClient _client;
	private string _token = "";
	private string _talentsDbId = "a06f31b6b7a64f56b387d19fc85e727f";
	public override void _EnterTree()
	{
		_dock = GD.Load<PackedScene>("res://addons/notionconnector/notion_connector_dock.tscn").Instantiate() as NotionConnectorDock;
		AddControlToDock(DockSlot.LeftUl, _dock);
		_dock.ConnectPressed += OnConnectPressed;
	}

	public override void _ExitTree()
	{
		RemoveControlFromDocks(_dock);
	}
	
	private void OnConnectPressed()
	{
		if (_client != null) return;
		_client = NotionClientFactory.Create(new ClientOptions
		{
			AuthToken = _token
		});
			
		GD.PushWarning("Connected to Notion API");
		DatabaseTest();
	}
	
	private async void DatabaseTest()
	{
		var query = new DatabasesQueryParameters
		{
			PageSize = 100 // Retrieve up to 100 rows at a time
		};

		var allRows = new List<Page>();
		string startCursor = null;

		do
		{
			var response = await _client.Databases.QueryAsync(_talentsDbId, query);

			allRows.AddRange(response.Results);

			startCursor = response.HasMore ? response.NextCursor : null;

			query.StartCursor = startCursor;

		} while (startCursor != null);
		
		var json = JsonConvert.SerializeObject(allRows, Formatting.Indented);
		
		List<Root> rows = JsonConvert.DeserializeObject<List<Root>>(json);

		var newRows = new List<object>();

		foreach (var row in rows)
		{
			var talentName = row.Properties.TalentName.Title[0].PlainText;
			var description = row.Properties.Description.RichText[0].PlainText;

			newRows.Add(new { TalentName = talentName, Description = description });
		}

		var newJson = JsonConvert.SerializeObject(newRows, Formatting.Indented);

		GD.PushWarning(newJson);
		
		using var file = FileAccess.Open("res://data/talents.json", FileAccess.ModeFlags.Write);
		file.StoreString(newJson);
		file.Close();
		
	}
}

public class Parent
{
	[JsonProperty("database_id")]
	public string DatabaseId { get; set; }

	[JsonProperty("Type")]
	public string Type { get; set; }
}
public class UniqueId
{
    [JsonProperty("prefix")]
    public object Prefix { get; set; }

    [JsonProperty("number")]
    public double Number { get; set; }
}

public class ID
{
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("unique_id")]
    public UniqueId UniqueId { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }
}

public class Text
{
    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("link")]
    public object Link { get; set; }
}

public class Annotations
{
    [JsonProperty("bold")]
    public bool Bold { get; set; }

    [JsonProperty("italic")]
    public bool Italic { get; set; }

    [JsonProperty("strikethrough")]
    public bool Strikethrough { get; set; }

    [JsonProperty("underline")]
    public bool Underline { get; set; }

    [JsonProperty("code")]
    public bool Code { get; set; }

    [JsonProperty("color")]
    public string Color { get; set; }
}

public class Title
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("text")]
    public Text Text { get; set; }

    [JsonProperty("plain_text")]
    public string PlainText { get; set; }

    [JsonProperty("href")]
    public object Href { get; set; }

    [JsonProperty("annotations")]
    public Annotations Annotations { get; set; }
}

public class TalentName
{
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("title")]
    public List<Title> Title { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }
}

public class Description
{
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("rich_text")]
    public List<Title> RichText { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }
}

public class Properties
{
    [JsonProperty("ID")]
    public ID ID { get; set; }

    [JsonProperty("TalentName")]
    public TalentName TalentName { get; set; }

    [JsonProperty("Description")]
    public Description Description { get; set; }
}

public class CreatedBy
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("object")]
    public string Object { get; set; }
}

public class LastEditedBy
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("object")]
    public string Object { get; set; }
}

public class Root
{
    [JsonProperty("parent")]
    public Parent Parent { get; set; }

    [JsonProperty("archived")]
    public bool Archived { get; set; }

    [JsonProperty("properties")]
    public Properties Properties { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("icon")]
    public object Icon { get; set; }

    [JsonProperty("cover")]
    public object Cover { get; set; }

    [JsonProperty("object")]
    public string Object { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("created_time")]
    public DateTime CreatedTime { get; set; }

    [JsonProperty("last_edited_time")]
    public DateTime LastEditedTime { get; set; }

    [JsonProperty("created_by")]
    public CreatedBy CreatedBy { get; set; }

    [JsonProperty("last_edited_by")]
    public LastEditedBy LastEditedBy { get; set; }

    [JsonProperty("public_url")]
    public object PublicUrl { get; set; }
}
#endif
