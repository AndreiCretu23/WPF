using Quantum.Exceptions;
using System;
using System.Diagnostics;

namespace Quantum.Utils
{
    public static class GeneralExtensions
    {
        [DebuggerHidden]
        public static T AssertNotNull<T>(this T source, string sourceName = null)
        {
            if (source != null)
            {
                return source;
            }
            else
            {
                if (sourceName != null)
                {
                    throw new ArgumentNullException($"Error : {sourceName} cannot be null.");
                }
                else
                {
                    throw new ArgumentNullException($"Error : Object cannot be null.");
                }
            }
        }

        [DebuggerHidden]
        public static T AssertParameterNotNull<T>(this T source, string parameterName = null)
        {
            if(source != null) {
                return source;
            }
            else {
                if(parameterName != null) {
                    throw new ArgumentNullException($"Error : {parameterName} cannot be null.");
                }
                else {
                    throw new ArgumentNullException($"Error : Parameter cannot be null.");
                }
            }
        }
        

        [DebuggerHidden]
        public static void IfNotNull<T>(this T source, Action<T> action)
            where T : class
        {
            action.AssertParameterNotNull(nameof(action));
            if (source != null) {
                action(source);
            }
        }

        [DebuggerHidden]
        public static void IfNotNull<T>(this T? source, Action<T> action)
         where T : struct
        {
            action.AssertParameterNotNull(nameof(action));
            if (source != null) {
                action(source.Value);
            }
        }
        
        [DebuggerHidden]
        public static TResult IfNotNull<T, TResult>(this T source, Func<T, TResult> getter, TResult defaultValue = default(TResult))
            where T : class
        {
            getter.AssertParameterNotNull(nameof(getter));
            if (source != null) {
                return getter(source);
            }
            return defaultValue;
        }
        
        [DebuggerHidden]
        public static TResult IfNotNull<T, TResult>(this T? source, Func<T, TResult> getter, TResult defaultValue = default(TResult))
         where T : struct
        {
            getter.AssertParameterNotNull(nameof(getter));
            if (source != null) {
                return getter(source.Value);
            }
            return defaultValue;
        }


        [DebuggerHidden]
        public static T SafeCast<T>(this object source, string message = null)
        {
            source.AssertNotNull();
            try {
                return (T)source;
            }
            catch {
                if(message == null) {
                    throw new UnexpectedTypeException(typeof(T), source.GetType());
                }
                else {
                    throw new UnexpectedTypeException(typeof(T), source.GetType(), message);
                }
            }
        }
        
        [DebuggerHidden]
        public static void IfIs<T>(this object source, Action<T> action)
        {
            source.AssertNotNull();
            action.AssertParameterNotNull(nameof(action));
            try {
                action(source.SafeCast<T>());
            }
            catch(UnexpectedTypeException) { } // Do nothing. It means that the object is not of the specified type.
        }
        
        [DebuggerHidden]
        public static TResult IfIs<T, TResult>(this object source, Func<T, TResult> func, TResult defaultValue = default(TResult))
            where T : class
        {
            source.AssertNotNull();
            func.AssertParameterNotNull(nameof(func));
            try {
                return func(source.SafeCast<T>());
            }
            catch (UnexpectedTypeException) { return defaultValue; }  // Return the defaultValue. It means that the object is not of the specified type.
        }


        [DebuggerHidden]
        public static TypeCase<T> CaseType<T, TDerived>(this T source, Action<TDerived> action)
         where TDerived : T
        {
            return new TypeCase<T>(source).CaseType(action);
        }

        [DebuggerHidden]
        public static TypeCase<T, TResult> CaseType<T, TDerived, TResult>(this T source, Func<TDerived, TResult> getter, TResult defaultValue = default(TResult))
           where TDerived : T
        {
            return new TypeCase<T, TResult>(source, defaultValue).CaseType(getter);
        }
    }

    public class TypeCase<T>
    {
        public T Target { get; set; }
        public bool IsMatched { get; set; }

        [DebuggerHidden]
        public TypeCase(T target)
        {
            this.Target = target;
        }
        [DebuggerHidden]
        public TypeCase<T> CaseType<TDerived>(Action<TDerived> action)
           where TDerived : T
        {
            action = action.AssertParameterNotNull(nameof(action));

            if (!this.IsMatched && this.Target is TDerived)
            {
                action((TDerived)this.Target);
                this.IsMatched = true;
            }
            return this;
        }
        [DebuggerHidden]
        public TypeCase<T> CaseType<TDerived>(Func<TDerived, bool> extraMatch, Action<TDerived> action)
           where TDerived : T
        {
            extraMatch = extraMatch.AssertParameterNotNull(nameof(extraMatch));
            action = action.AssertParameterNotNull(nameof(action));


            if (!this.IsMatched && this.Target is TDerived && extraMatch((TDerived)this.Target))
            {
                action((TDerived)this.Target);
                this.IsMatched = true;
            }
            return this;
        }
        [DebuggerHidden]
        public void Default(Action<T> defaultAction)
        {
            defaultAction = defaultAction.AssertParameterNotNull(nameof(defaultAction));
            if (!this.IsMatched)
            {
                defaultAction(this.Target);
            }
        }

        [DebuggerHidden]
        public void DefaultThrow(string message)
        {
            if (!this.IsMatched)
            {
                throw new NotSupportedException(message);
            }
        }

        [DebuggerHidden]
        public void DefaultThrow(string message, params object[] args)
        {
            if (!this.IsMatched)
            {
                throw new NotSupportedException(string.Format(message, args));
            }
        }
    }

    public class TypeCase<T, TResult>
    {
        public T Target { get; set; }
        public TResult Result { get; set; }
        public bool IsMatched { get; set; }

        public TypeCase(T target, TResult defaultResult)
        {
            this.Target = target;
            this.Result = defaultResult;
        }

        [DebuggerHidden]
        public TypeCase<T, TResult> CaseType<TDerived>(Func<TDerived, TResult> action)
           where TDerived : T
        {
            action = action.AssertParameterNotNull(nameof(action));

            if (!this.IsMatched && this.Target is TDerived)
            {
                this.Result = action((TDerived)this.Target);
                this.IsMatched = true;
            }
            return this;
        }

        [DebuggerHidden]
        public TypeCase<T, TResult> Default(TResult defaultValue)
        {
            if (!this.IsMatched)
            {
                this.Result = defaultValue;
            }
            return this;
        }

        [DebuggerHidden]
        public TypeCase<T, TResult> Default(Func<T, TResult> defaultAction = null)
        {
            if (!this.IsMatched && defaultAction != null)
            {
                this.Result = defaultAction(this.Target);
            }
            return this;
        }

        [DebuggerHidden]
        public TypeCase<T, TResult> DefaultThrow(string message)
        {
            if (!this.IsMatched)
            {
                throw new NotSupportedException(message);
            }
            return this;
        }
    }
}
