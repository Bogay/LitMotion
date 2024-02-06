using System;

namespace LitMotion.Sequences
{
    public static class MotionSequenceBuilderExtensions
    {
        public static IMotionSequenceBuilder Append(this IMotionSequenceBuilder builder, Func<MotionHandle> factoryDelegate)
        {
            builder.Factories.Add(new AppendConfiguration(factoryDelegate));
            return builder;
        }

        public static IMotionSequenceBuilder Append<TState>(this IMotionSequenceBuilder builder, TState state, Func<TState, MotionHandle> factoryDelegate)
        {
            builder.Factories.Add(new AppendConfigurationWithState<TState>(state, factoryDelegate));
            return builder;
        }

        public static IMotionSequenceBuilder AppendCallback(this IMotionSequenceBuilder builder, Action callback)
        {
            builder.Factories.Add(new AppendCallbackConfiguration(callback));
            return builder;
        }

        public static IMotionSequenceBuilder AppendCallback<TState>(this IMotionSequenceBuilder builder, TState state, Action<TState> callback)
        {
            builder.Factories.Add(new AppendCallbackConfigurationWithState<TState>(state, callback));
            return builder;
        }

        public static IMotionSequenceBuilder AppendGroup(this IMotionSequenceBuilder builder, Action<MotionSequenceBufferWriter> configureDelegate)
        {
            builder.Factories.Add(new AppendGroupConfiguration(configureDelegate));
            return builder;
        }

        public static IMotionSequenceBuilder AppendGroup<TState>(this IMotionSequenceBuilder builder, TState state, Action<TState, MotionSequenceBufferWriter> configureDelegate)
        {
            builder.Factories.Add(new AppendGroupCongigurationWithState<TState>(state, configureDelegate));
            return builder;
        }

        sealed class AppendConfiguration : IMotionSequenceConfiguration
        {
            public AppendConfiguration(Func<MotionHandle> factoryDelegate)
            {
                this.factoryDelegate = factoryDelegate;
            }

            readonly Func<MotionHandle> factoryDelegate;

            public void Configure(MotionSequenceBufferWriter writer)
            {
                writer.Add(factoryDelegate());
            }
        }

        sealed class AppendConfigurationWithState<T> : IMotionSequenceConfiguration
        {
            public AppendConfigurationWithState(T state, Func<T, MotionHandle> factoryDelegate)
            {
                this.state = state;
                this.factoryDelegate = factoryDelegate;
            }

            readonly T state;
            readonly Func<T, MotionHandle> factoryDelegate;

            public void Configure(MotionSequenceBufferWriter writer)
            {
                writer.Add(factoryDelegate(state));
            }
        }


        sealed class AppendCallbackConfiguration : IMotionSequenceConfiguration
        {
            public AppendCallbackConfiguration(Action callback)
            {
                this.callback = callback;
            }

            readonly Action callback;

            public void Configure(MotionSequenceBufferWriter writer)
            {
                callback.Invoke();
            }
        }

        sealed class AppendCallbackConfigurationWithState<T> : IMotionSequenceConfiguration
        {
            public AppendCallbackConfigurationWithState(T state, Action<T> callback)
            {
                this.state = state;
                this.callback = callback;
            }

            readonly T state;
            readonly Action<T> callback;

            public void Configure(MotionSequenceBufferWriter writer)
            {
                callback.Invoke(state);
            }
        }

        sealed class AppendGroupConfiguration : IMotionSequenceConfiguration
        {
            public AppendGroupConfiguration(Action<MotionSequenceBufferWriter> configurationDelegate)
            {
                this.configurationDelegate = configurationDelegate;
            }

            readonly Action<MotionSequenceBufferWriter> configurationDelegate;

            public void Configure(MotionSequenceBufferWriter writer)
            {
                configurationDelegate(writer);
            }
        }

        sealed class AppendGroupCongigurationWithState<T> : IMotionSequenceConfiguration
        {
            public AppendGroupCongigurationWithState(T state, Action<T, MotionSequenceBufferWriter> configurationDelegate)
            {
                this.state = state;
                this.configurationDelegate = configurationDelegate;
            }

            readonly T state;
            readonly Action<T, MotionSequenceBufferWriter> configurationDelegate;

            public void Configure(MotionSequenceBufferWriter writer)
            {
                configurationDelegate(state, writer);
            }
        }
    }
}