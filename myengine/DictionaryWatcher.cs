﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEngine
{

	public class DictionaryWatcher<TKey, TItem>
	{
		public event Action<TKey, TItem> OnAdded;
		/// <summary>
		/// Guarantees that OnAdded was called on same TKey before.
		/// </summary>
		public event Action<TKey, TItem> OnRemoved;
		/// <summary>
		/// Guarantees that OnAdded was called on same TKey before.
		/// </summary>
		public event Action<TKey, TItem> OnUpdated;


		Dictionary<TKey, TItem> currentValues = new Dictionary<TKey, TItem>();

		public void UpdateBy(IEnumerable<KeyValuePair<TKey, TItem>> source)
		{
			foreach (var kvp in source)
			{
				TItem currentValue;
				if (currentValues.TryGetValue(kvp.Key, out currentValue))
				{
					if (currentValue.Equals(kvp.Value) == false)
					{
						currentValues[kvp.Key] = kvp.Value;
						OnUpdated.Raise(kvp.Key, kvp.Value);
					}
				}
				else
				{
					currentValues[kvp.Key] = kvp.Value;
					OnAdded.Raise(kvp.Key, kvp.Value);
				}
			}

			var keysRemoved = currentValues.Keys.Except(source.Select(kvp => kvp.Key)).ToArray();
			foreach (var keyRemoved in keysRemoved)
			{
				OnRemoved.Raise(keyRemoved, currentValues[keyRemoved]);
				currentValues.Remove(keyRemoved);
			}

		}

	}

	public class DictionaryWatcher<TKey, TItem, TValueToDetectChangesOn>
	{
		public event Action<TKey, TItem> OnAdded;
		/// <summary>
		/// Guarantees that OnAdded was called on same TKey before.
		/// </summary>
		public event Action<TKey, TItem> OnRemoved;
		/// <summary>
		/// Guarantees that OnAdded was called on same TKey before.
		/// </summary>
		public event Action<TKey, TItem> OnUpdated;


		public Func<TKey, TItem, TValueToDetectChangesOn> getValueToDetectChangesOn;


		Dictionary<TKey, Tuple<TItem, TValueToDetectChangesOn>> currentValues = new Dictionary<TKey, Tuple<TItem, TValueToDetectChangesOn>>();

		public void UpdateBy(IEnumerable<KeyValuePair<TKey, TItem>> source)
		{
			foreach (var kvp in source)
			{
				Tuple<TItem, TValueToDetectChangesOn> currentValue;
				var v = getValueToDetectChangesOn(kvp.Key, kvp.Value);
				if (currentValues.TryGetValue(kvp.Key, out currentValue))
				{
					if (currentValue.Item2.Equals(v) == false)
					{
						currentValues[kvp.Key] = Tuple.Create(kvp.Value, v);
						OnUpdated.Raise(kvp.Key, kvp.Value);
					}
				}
				else
				{
					currentValues[kvp.Key] = Tuple.Create(kvp.Value, v);
					OnAdded.Raise(kvp.Key, kvp.Value);
				}
			}

			var keysRemoved = currentValues.Keys.Except(source.Select(kvp => kvp.Key)).ToArray();
			foreach (var keyRemoved in keysRemoved)
			{
				OnRemoved.Raise(keyRemoved, currentValues[keyRemoved].Item1);
				currentValues.Remove(keyRemoved);
			}

		}

	}
}
