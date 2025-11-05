using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCC.Services.Utils
{
    public class LoadingService
    {
        private readonly List<LoadingItem> _loadingItems = new();
        public IReadOnlyList<LoadingItem> LoadingItems => _loadingItems.AsReadOnly();

        public event Action? OnLoadingStateChanged;

        public bool IsLoading 
        {
            get { return _loadingItems.Count > 0; }
            set { }
        }

        public void Loading(bool isLoading, string message = "Cargando...")
        {
            if (isLoading)
            {
                // Agrega un nuevo item de carga
                var newItem = new LoadingItem(message);
                _loadingItems.Add(newItem);
            }
            else
            {
                var lastItem = _loadingItems.LastOrDefault();
                if (lastItem != null)
                {
                    _loadingItems.Remove(lastItem);
                }
            }
            NotifyStateChanged();
        }

        // --- Los métodos anteriores se mantienen si aún los necesitas para casos de uso más específicos ---
        public LoadingItem AddLoadingItem(string message)
        {
            var newItem = new LoadingItem(message);
            _loadingItems.Add(newItem);
            NotifyStateChanged();
            return newItem;
        }

        public void CompleteLoadingItem(string id)
        {
            var item = _loadingItems.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                item.IsCompleted = true;
                item.EndTime = DateTime.Now;
                NotifyStateChanged();
            }
        }

        public void RemoveLoadingItem(string id)
        {
            var item = _loadingItems.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                _loadingItems.Remove(item);
                NotifyStateChanged();
            }
        }

        public void ClearCompletedItems()
        {
            _loadingItems.RemoveAll(item => item.IsCompleted);
            NotifyStateChanged();
        }
        // ---------------------------------------------------------------------------------------------------


        private void NotifyStateChanged() => OnLoadingStateChanged?.Invoke();
    }

    public class LoadingItem
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Message { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime? EndTime { get; set; }

        public LoadingItem(string message)
        {
            Message = message;
        }
    }
}