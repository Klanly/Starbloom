using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EventRecord : SerializedMonoBehaviour
{
	[Serializable]
	public class AttributeMap : Dictionary<string, List<object>> { }

	[Serializable]
	public class ActionData
	{
		[SerializeField, ReadOnly]
		public int Minute;

		[SerializeField]
		public AttributeMap Attributes = new AttributeMap();
	}

	[Serializable]
	public class ActionMap : Dictionary<string, ActionData> { }

	[Serializable]
	public class DailyRecord : Dictionary<int, ActionMap> { }

	[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Day", ValueLabel = "Record")]
	public Dictionary<int, DailyRecord> Log = new Dictionary<int, DailyRecord>();

	public void OnEnable()
	{
		AttributeMap attributes = new AttributeMap();
		attributes.Add("Water", new List<object> { 1 });
		AddAction("water", 0, 1, attributes );
	}

	public void AddAction(string _action, int _day, int _playerID, AttributeMap _attributes = null)
	{
		DailyRecord dr = null;
		if (!Log.ContainsKey(_day))
		{
			dr = new DailyRecord();
			Log[_day] = dr;
		}
		else
			dr = Log[_day];

		ActionMap am = null;
		if (!dr.ContainsKey(_playerID))
		{
			am = new ActionMap();
			dr[_playerID] = am;
		}
		else
			am = dr[_playerID];

		ActionData ad = null;
		if (!am.ContainsKey(_action))
		{
			ad = new ActionData();
			am[_action] = ad;
		}
		else
			ad = am[_action];

		if (null != _attributes)
			foreach (string key in _attributes.Keys)
			{
				if (null == ad.Attributes)
					ad.Attributes = new AttributeMap();

				List<object> attrValues;
				if (!ad.Attributes.ContainsKey(key))
				{
					attrValues = new List<object>();
					ad.Attributes[key] = attrValues;
				}
				else
					attrValues = ad.Attributes[key];

				List<object> addVals = _attributes[key];
				if( null != addVals )
					foreach (object val in addVals)
						if( null != val )
							attrValues.Add(val);
			}
	}
	
	public string Serialize()
	{
		return JsonUtility.ToJson(Log);
	}

	public void Deserialize( string _json )
	{
		JsonUtility.FromJsonOverwrite(_json, Log);
	}
}