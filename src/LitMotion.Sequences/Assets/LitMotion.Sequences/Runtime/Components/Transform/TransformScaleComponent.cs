using System;
using UnityEngine;
using LitMotion.Extensions;

namespace LitMotion.Sequences.Components
{
    [SequenceComponentMenu("Transform/Scale")]
    public sealed class TransformScaleComponent : SequenceComponentBase<Vector3, Transform>
    {
        static readonly Type componentType = typeof(TransformScaleComponent);

        public override void ResetComponent()
        {
            base.ResetComponent();
            displayName = "Scale";
        }

        public override void Configure(ISequencePropertyTable sequencePropertyTable, MotionSequenceItemBuilder builder)
        {
            var target = this.target.Resolve(sequencePropertyTable);
            if (target == null) return;
            
            if (!sequencePropertyTable.TryGetInitialValue<(Transform, Type), Vector3>((target, componentType), out var initialLocalScale))
            {
                initialLocalScale = target.localScale;
                sequencePropertyTable.SetInitialValue((target, componentType), initialLocalScale);
            }

            var currentValue = Vector3.zero;

            switch (motionMode)
            {
                case MotionMode.Relative:
                    currentValue = initialLocalScale;
                    break;
                case MotionMode.Additive:
                    currentValue = target.localScale;
                    break;
            }

            var motionBuilder = LMotion.Create(currentValue + startValue, currentValue + endValue, duration);
            ConfigureMotionBuilder(ref motionBuilder);

            var handle = motionBuilder.BindToLocalScale(target);
            builder.Add(handle);
        }

        public override void RestoreValues(ISequencePropertyTable sequencePropertyTable)
        {
            var target = this.target.Resolve(sequencePropertyTable);
            if (target == null) return;

            if (sequencePropertyTable.TryGetInitialValue<(Transform, Type), Vector3>((target, componentType), out var initialLocalScale))
            {
                target.localScale = initialLocalScale;
            }
        }
    }
}
