using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace LitMotion.Sequences
{
    [AddComponentMenu("LitMotion/Sequence Player")]
    public sealed class SequencePlayer : MonoBehaviour, ISequencePropertyTable
    {
        [SerializeField] SequenceAsset asset;

        [SerializeField] List<PropertyName> propertyNameList;
        [SerializeField] List<UnityObject> objectList;

        MotionSequence sequence;

        public bool IsPlaying() => sequence != null && sequence.IsActive();

        public void Play()
        {
            if (IsPlaying()) throw new InvalidOperationException("Sequence is now playing.");
#if UNITY_EDITOR
            sequence = asset.CreateSequence(this);
#else
            if (sequence == null) sequence = asset.CreateSequence(this); // cache sequence
#endif
            sequence.Play();
        }

        public void Complete()
        {
            sequence?.Complete();
        }

        public void Cancel()
        {
            sequence?.Cancel();
        }

#if UNITY_EDITOR
        internal bool IsModified => dictionaries.Count > 0;
        
        internal void CancelAndRestoreValues()
        {
            sequence?.Cancel();
            foreach (var component in asset.Components)
            {
                component.RestoreValues(this);
            }
            ((ISequencePropertyTable)this).ClearInitialValues();
        }

        internal void PlayPreview()
        {
            if (asset == null) return;

            sequence?.Complete();
            sequence = asset.CreateSequence(this);
            sequence.Play();
        }
#endif

        void IExposedPropertyTable.ClearReferenceValue(PropertyName id)
        {
            var index = propertyNameList.IndexOf(id);
            if (index != -1)
            {
                propertyNameList.RemoveAt(index);
                objectList.RemoveAt(index);
            }
        }

        UnityObject IExposedPropertyTable.GetReferenceValue(PropertyName id, out bool idValid)
        {
            var index = propertyNameList.IndexOf(id);
            idValid = index != -1;
            return idValid ? objectList[index] : null;
        }

        void IExposedPropertyTable.SetReferenceValue(PropertyName id, UnityObject value)
        {
            if (PropertyName.IsNullOrEmpty(id)) return;

            var index = propertyNameList.IndexOf(id);
            if (index != -1)
            {
                propertyNameList[index] = id;
                objectList[index] = value;
            }
            else if (value == null)
            {
                propertyNameList.Add(id);
                objectList.Add(value);
            }
        }

        readonly Dictionary<Type, IDictionary> dictionaries = new();

        Dictionary<TKey, TValue> GetDictionary<TKey, TValue>()
        {
            var typeKey = typeof((TKey, TValue));
            if (dictionaries.TryGetValue(typeKey, out var dictionary)) return (Dictionary<TKey, TValue>)dictionary;
            dictionary = new Dictionary<TKey, TValue>();
            dictionaries.Add(typeKey, dictionary);
            return (Dictionary<TKey, TValue>)dictionary;
        }

        void ISequencePropertyTable.SetInitialValue<TKey, TValue>(TKey key, TValue value)
        {
            GetDictionary<TKey, TValue>()[key] = value;
        }

        bool ISequencePropertyTable.TryGetInitialValue<TKey, TValue>(TKey key, out TValue value)
        {
            return GetDictionary<TKey, TValue>().TryGetValue(key, out value);
        }

        void ISequencePropertyTable.ClearInitialValues()
        {
            dictionaries.Clear();
        }
    }
}
