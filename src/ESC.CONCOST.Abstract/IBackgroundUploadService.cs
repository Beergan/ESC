using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace ESC.CONCOST.Abstract
{
    public interface IBackgroundUploadService
    {
        event Action OnChange;
        List<UploadProgressItem> ActiveUploads { get; }
        void StartUpload(string fileGuid, string fileName);
        [JSInvokable] void UpdateProgress(string fileGuid, int progress, int? etaSeconds, double? speedMBps);
        [JSInvokable] void CompleteUpload(string fileGuid, string url, string fileName);
        [JSInvokable] void FailUpload(string fileGuid, string error);
        DotNetObjectReference<BackgroundUploadService> GetDotNetRef();
    }

    public class UploadProgressItem
    {
        public string FileGuid { get; set; }
        public string FileName { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }
        public string Error { get; set; }
        public string Url { get; set; }
        public DateTime StartTime { get; set; }
        public string EstimatedRemainingTime { get; set; } = "";
        public string UploadSpeed { get; set; } = "";  // VD: "1.23 MB/s"
    }

    public class BackgroundUploadService : IBackgroundUploadService, IDisposable
    {
        public event Action OnChange;
        public List<UploadProgressItem> ActiveUploads { get; } = new();
        private DotNetObjectReference<BackgroundUploadService> _objRef;

        public DotNetObjectReference<BackgroundUploadService> GetDotNetRef()
        {
            if (_objRef == null) _objRef = DotNetObjectReference.Create(this);
            return _objRef;
        }

        public void StartUpload(string fileGuid, string fileName)
        {
            var item = ActiveUploads.FirstOrDefault(x => x.FileGuid == fileGuid);
            if (item == null)
            {
                item = new UploadProgressItem
                {
                    FileGuid = fileGuid,
                    FileName = fileName,
                    Progress = 0,
                    StartTime = DateTime.UtcNow
                };
                ActiveUploads.Add(item);
            }
            else
            {
                item.Progress = 0;
                item.IsCompleted = false;
                item.Error = null;
                item.EstimatedRemainingTime = "";
                item.UploadSpeed = "";
                item.StartTime = DateTime.UtcNow;
            }
            NotifyChanged();
        }

        [JSInvokable]
        public void UpdateProgress(string fileGuid, int progress, int? etaSeconds, double? speedMBps)
        {
            var item = ActiveUploads.FirstOrDefault(x => x.FileGuid == fileGuid);
            if (item == null) return;

            item.Progress = progress;

            // ETA từ JS (sliding window) — chính xác hơn tính từ StartTime
            if (etaSeconds.HasValue && progress < 100)
            {
                var eta = TimeSpan.FromSeconds(etaSeconds.Value);
                item.EstimatedRemainingTime = eta.TotalMinutes >= 1
                    ? $"{(int)eta.TotalMinutes}m {eta.Seconds}s"
                    : $"{eta.Seconds}s";
            }
            else
            {
                item.EstimatedRemainingTime = "";
            }

            // Tốc độ upload
            item.UploadSpeed = speedMBps.HasValue
                ? $"{speedMBps.Value:F2} MB/s"
                : "";

            NotifyChanged();
        }

        [JSInvokable]
        public void CompleteUpload(string fileGuid, string url, string fileName)
        {
            var item = ActiveUploads.FirstOrDefault(x => x.FileGuid == fileGuid);
            if (item != null)
            {
                item.IsCompleted = true;
                item.Progress = 100;
                item.Url = url;
                item.FileName = fileName;
                item.EstimatedRemainingTime = "";
                item.UploadSpeed = "";
                NotifyChanged();

                // Tự xóa sau 10 giây
                _ = Task.Delay(10000).ContinueWith(_ => {
                    ActiveUploads.Remove(item);
                    NotifyChanged();
                });
            }
        }

        [JSInvokable]
        public void FailUpload(string fileGuid, string error)
        {
            var item = ActiveUploads.FirstOrDefault(x => x.FileGuid == fileGuid);
            if (item != null)
            {
                item.Error = error;
                item.EstimatedRemainingTime = "";
                item.UploadSpeed = "";
                NotifyChanged();
            }
        }

        private void NotifyChanged() => OnChange?.Invoke();

        public void Dispose() => _objRef?.Dispose();
    }
}