﻿using MiniBiggy.DataStores;
using MiniBiggy.SaveStrategies;
using MiniBiggy.Serializers;
using System;

namespace MiniBiggy {

    [Obsolete("Use Create.ListOf<T> instead")]
    public class CreateList<T> : IChooseSerializer<T>, IChooseSaveMode<T> where T : new() {
        private IDataStore _dataStore;
        private ISerializer _serializer;
        private ISaveStrategy _saveStrategy;

        private CreateList() { }

        public static IChooseSerializer<T> SavingAt(string fullpath) {
            var builder = new CreateList<T>();
            builder._dataStore = new FileSystem(fullpath);
            return builder;
        }

        public IChooseSaveMode<T> UsingJsonSerializer() {
            _serializer = new JsonSerializer();
            return this;
        }

        public IChooseSaveMode<T> UsingPrettyJsonSerializer() {
            _serializer = new PrettyJsonSerializer();
            return this;
        }

        public PersistentList<T> BackgroundSavingEvery(TimeSpan timeSpan) {
            _saveStrategy = new BackgroundSave(timeSpan);
            return Create();
        }

        public PersistentList<T> BackgroundSavingEverySecond() {
            _saveStrategy = new BackgroundSave(TimeSpan.FromSeconds(1));
            return Create();
        }

        public PersistentList<T> BackgroundSavingEveryTwoSeconds() {
            _saveStrategy = new BackgroundSave(TimeSpan.FromSeconds(2));
            return Create();
        }

        public PersistentList<T> BackgroundSavingEveryFiveSeconds() {
            _saveStrategy = new BackgroundSave(TimeSpan.FromSeconds(5));
            return Create();
        }

        public PersistentList<T> BackgroundSavingEveryMinute() {
            _saveStrategy = new BackgroundSave(TimeSpan.FromMinutes(1));
            return Create();
        }

        public PersistentList<T> SavingWhenCollectionChanges() {
            _saveStrategy = new SaveOnEveryChange();
            return Create();
        }

        public PersistentList<T> SavingWhenRequested() {
            _saveStrategy = new SaveOnlyWhenRequested();
            return Create();
        }

        private PersistentList<T> Create() {
            if (_serializer == null) {
                _serializer = new JsonSerializer();
            }
            if (_saveStrategy == null) {
                _saveStrategy = new SaveOnlyWhenRequested();
            }
            return new PersistentList<T>(_dataStore, _serializer, _saveStrategy);
        }
    }

    public interface IChooseSerializer<T> where T : new() {
        IChooseSaveMode<T> UsingJsonSerializer();
        IChooseSaveMode<T> UsingPrettyJsonSerializer();

    }

    public interface IChooseSaveMode<T> where T : new() {
        PersistentList<T> BackgroundSavingEvery(TimeSpan timeSpan);
        PersistentList<T> BackgroundSavingEverySecond();

        PersistentList<T> BackgroundSavingEveryTwoSeconds();

        PersistentList<T> BackgroundSavingEveryFiveSeconds();

        PersistentList<T> BackgroundSavingEveryMinute();

        PersistentList<T> SavingWhenCollectionChanges();

        PersistentList<T> SavingWhenRequested();
    }
}