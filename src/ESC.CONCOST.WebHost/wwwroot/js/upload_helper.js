window.uploadHelper = {
    uploadInChunks: async (inputElement, fileGuid, projectFolder, dotNetHelper) => {
        const file = inputElement.files[0];
        if (!file) return;

        const chunkSize = 1024 * 1024 * 20; // 20MB
        const totalChunks = Math.ceil(file.size / chunkSize);

        // --- Theo dõi thời gian ---
        const WINDOW_SIZE = 5; // Trung bình trượt 5 chunk gần nhất
        const chunkTimestamps = []; // [{ bytes, time }]
        let totalBytesUploaded = 0;

        for (let i = 0; i < totalChunks; i++) {
            const start = i * chunkSize;
            const end = Math.min(file.size, start + chunkSize);
            const chunk = file.slice(start, end);
            const chunkBytes = end - start;

            const formData = new FormData();
            formData.append('chunk', chunk, file.name);
            formData.append('fileGuid', fileGuid);
            formData.append('chunkIndex', i);
            formData.append('totalChunks', totalChunks);
            formData.append('fileName', file.name);
            formData.append('projectFolder', projectFolder || "");

            const chunkStartTime = performance.now();

            try {
                const response = await fetch('/api/Upload/UploadChunk', {
                    method: 'POST',
                    body: formData,
                });

                if (!response.ok) throw new Error('Upload failed');

                const result = await response.json();

                // --- Tính ETA ---
                const elapsed = performance.now() - chunkStartTime; // ms
                totalBytesUploaded += chunkBytes;

                chunkTimestamps.push({ bytes: chunkBytes, time: elapsed });
                if (chunkTimestamps.length > WINDOW_SIZE) {
                    chunkTimestamps.shift(); // Giữ sliding window
                }

                const windowBytes = chunkTimestamps.reduce((s, c) => s + c.bytes, 0);
                const windowTime = chunkTimestamps.reduce((s, c) => s + c.time, 0);
                const speedBytesPerMs = windowBytes / windowTime; // bytes/ms

                const remainingBytes = file.size - totalBytesUploaded;
                const etaSeconds = speedBytesPerMs > 0
                    ? Math.round(remainingBytes / speedBytesPerMs / 1000)
                    : null;

                const progress = Math.round(((i + 1) / totalChunks) * 100);

                await dotNetHelper.invokeMethodAsync(
                    'UpdateProgress',
                    fileGuid,
                    progress,
                    etaSeconds,                                         // giây còn lại
                    Math.round(speedBytesPerMs * 1000 / 1024 / 1024 * 100) / 100  // MB/s
                );

                if (result.success && result.item && result.item.startsWith("/upload")) {
                    await dotNetHelper.invokeMethodAsync('CompleteUpload', fileGuid, result.item, file.name);
                    return { success: true, url: result.item, fileName: file.name };
                }
            } catch (error) {
                console.error(error);
                await dotNetHelper.invokeMethodAsync('FailUpload', fileGuid, error.message);
                return { success: false, message: error.message };
            }
        }
    },

    preventNavigation: () => {
        window.onbeforeunload = () => "Upload in progress. If you leave now, the upload will be cancelled.";
    },
    allowNavigation: () => {
        window.onbeforeunload = null;
    },
    downloadFile: (url, fileName) => {
        const a = document.createElement('a');
        a.href = url;
        if (fileName) {
            a.download = fileName;
        }
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    }
};