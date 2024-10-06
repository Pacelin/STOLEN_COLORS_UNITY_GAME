using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace Runtime.Utils
{
    public class ReactiveLocalizedString : IReadOnlyReactiveProperty<string>
    {
        public string Value => _localizedString.GetLocalizedString();
        public bool HasValue => _localizedString != null;

        public TableEntryReference EntryReference => _localizedString.TableEntryReference;
        public TableReference TableReference => _localizedString.TableReference;
        
        private readonly LocalizedString _localizedString;

        public ReactiveLocalizedString([NotNull] LocalizedString localizedString)
        {
            _localizedString = localizedString;
        }
        
        public IDisposable Subscribe(IObserver<string> observer)
        {
            observer.OnNext(Value);
            _localizedString.StringChanged += Handle;
            
            return Disposable.Create(() => _localizedString.StringChanged -= Handle);
            void Handle(string change) => observer.OnNext(change);
        }
    }
}