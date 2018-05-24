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

		public List<object> AddAttribute( string _name, object _value = null)
		{
			if (null == Attributes)
				Attributes = new AttributeMap();

			List<object> attrValues = null;
			if (Attributes.ContainsKey(_name))
				attrValues = Attributes[_name];

			if( null == attrValues )
			{
				attrValues = new List<object>();
				Attributes[_name] = attrValues;
			}

			AddValue(attrValues, _value);

			return attrValues;
		}

		public bool HasAttribute( string _name )
		{
			if (null == Attributes)
				return false;

			if (!Attributes.ContainsKey(_name))
				return false;

			return true;
		}

		public List<object> GetAttributeValues( string _name )
		{
			if (!HasAttribute(_name))
				return null;

			return Attributes[_name];
		}

		public void AddValue<T>( string _attributeName, T _value )
		{
			AddValue(AddAttribute(_attributeName), _value);
		}

		protected void AddValue<T>( List<object> _attrValues, T _value )
		{
			if (null != _value)
				_attrValues.Add(_value);
		}

		public void AddAttributes( AttributeMap _attributes )
		{
			foreach (string key in _attributes.Keys)
			{
				List<object> attrVals = AddAttribute(key);
				List<object> addVals = _attributes[key];
				if (null != addVals)
					foreach (object val in addVals)
						AddValue(attrVals, val);
			}
		}
	}

	[Serializable]
	public class ActionMap : Dictionary<string, ActionData> { }

	[Serializable]
	public class DailyRecord : Dictionary<int, ActionMap> { }

	[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, KeyLabel = "Day", ValueLabel = "Record")]
	public Dictionary<int, DailyRecord> Log = new Dictionary<int, DailyRecord>();
	
	public ActionData AddAction(string _action, int _day, int _playerID)
	{
		DailyRecord dr = null;
		if (Log.ContainsKey(_day))
			dr = Log[_day];

		if( null == dr )
		{
			dr = new DailyRecord();
			Log[_day] = dr;
		}

		ActionMap am = null;
		if( dr.ContainsKey(_playerID) )
			am = dr[_playerID];

		if( null == am )
		{
			am = new ActionMap();
			dr[_playerID] = am;
		}

		ActionData ad = null;
		if (am.ContainsKey(_action))
			ad = am[_action];

		if( null == ad )
		{
			ad = new ActionData();
			ad.Attributes = new AttributeMap();
			am[_action] = ad;
		}
			
		return ad;
	}

	public ActionData GetAction(string _action, int _day, int _playerID)
	{
		DailyRecord dr = null;
		if (!Log.ContainsKey(_day))
			return null;

		dr = Log[_day];
		ActionMap am = null;
		if (!dr.ContainsKey(_playerID))
			return null;

		am = dr[_playerID];
		if (!am.ContainsKey(_action))
			return null;

		return am[_action];
	}

	public bool HasAction(string _action, int _day, int _playerID)
	{
		return null != GetAction(_action, _day, _playerID);
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