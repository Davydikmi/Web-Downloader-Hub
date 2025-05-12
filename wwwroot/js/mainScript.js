// ======= POST Запросы =======

// POST запрос на добавление элемента в список
document.getElementById("downloadForm").addEventListener("submit", function (e) {
    e.preventDefault();
    const url = document.getElementById("urlInput").value;
    const errorContainer = document.getElementById("errorContainer");
    errorContainer.innerHTML = "";

    fetch("/add", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(url)
    })
        .then(response => {
            if (!response.ok) return response.text().then(text => { throw new Error(text); });
            return response.json();
        })
        .then(record => {
            const fullUrl = record.url;
            const shortUrl = fullUrl.length > 50
                ? fullUrl.substring(0, 47) + "..."
                : fullUrl;

            const fileSizeMB = (record.fileSize / 1048576).toFixed(2); // bytes → MB
            const displayDate = new Date(record.downloadDate).toLocaleDateString();

            const html = `
    <div class="file-item shadow-sm" data-id="${record.id}" data-filepath="${record.filepath}">
        <div class="file-header">
            <div>
                <strong>${record.filename}</strong><br />
                <small>
                    <a href="${fullUrl}" target="_blank" title="${fullUrl}">
                        ${shortUrl}
                    </a>
                </small><br />
                <small>${fileSizeMB} MB • ${displayDate}</small>
            </div>
            <div class="btn-group actions">
                <img src="/img/trash.png" class="icon-btn trash-icon" alt="Удалить" title="Удалить" />
            </div>
        </div>
    </div>`;
            document.getElementById("fileList").insertAdjacentHTML("beforeend", html);
            document.getElementById("urlInput").value = "";
        })

});

// POST запрос на удаление элемента из списка
document.addEventListener("click", function (e) {
    if (e.target.classList.contains("trash-icon")) {
        const fileItem = e.target.closest(".file-item");
        const id = fileItem.dataset.id;
        const filepath = fileItem.dataset.filepath;
        const filename = fileItem.querySelector("strong").textContent;
        const url = fileItem.querySelector("a").href;
        const infoText = fileItem.querySelectorAll("small")[1].textContent;
        const fileSize = extractFileSize(infoText);

        const record = {
            id: id,
            filepath: filepath,
            filename: filename,
            url: url,
            fileSize: fileSize,
            downloadDate: new Date().toISOString()
        };

        fetch("/delete", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(record)
        })
            .then(response => {
                if (!response.ok) throw new Error("Ошибка при удалении элемента.");
                fileItem.remove();
            })
            .catch(error => alert(error.message));
    }
});




// POST запрос на очистку очереди
document.getElementById("clearListBtn").addEventListener("click", function () {
    const fileItems = document.querySelectorAll(".file-item");
    if (fileItems.length === 0) return;

    const records = Array.from(fileItems).map(item => {
        const filename = item.querySelector("strong").textContent;
        const url = item.querySelector("a").href;
        const infoText = item.querySelectorAll("small")[1].textContent;
        const fileSize = extractFileSize(infoText);

        return {
            id: item.dataset.id,
            filepath: item.dataset.filepath,
            filename: filename,
            url: url,
            fileSize: fileSize,
            downloadDate: new Date().toISOString()
        };
    });

    fetch("/clear-all", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(records)
    })
        .then(response => {
            if (!response.ok) throw new Error("Не удалось очистить список.");
            fileItems.forEach(item => item.remove());
        })
        .catch(error => alert(error.message));
});

// POST запрос на скачивание файлов
document.getElementById("downloadBtn").addEventListener("click", function () {
    const fileItems = document.querySelectorAll(".file-item");
    const filePaths = [];

    fileItems.forEach(item => {
        const filepath = item.getAttribute("data-filepath");
        if (filepath) filePaths.push(filepath);
    });

    fetch("/download", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(filePaths)
    })
        .then(response => {
            if (!response.ok) throw new Error("Ошибка при скачивании файлов.");
            return response.blob().then(blob => ({ blob, response }));
        })
        .then(({ blob, response }) => {
            const contentDisposition = response.headers.get("Content-Disposition");
            const filenameMatch = contentDisposition && contentDisposition.match(/filename="?([^"]+)"?/);
            const filename = filenameMatch ? filenameMatch[1] : "download.zip";

            const link = document.createElement("a");
            link.href = URL.createObjectURL(blob);
            link.download = filename;
            link.click();
        })
        .catch(error => alert(error.message));
});



// Функция безопасного парсинга fileSize из текста
function extractFileSize(text) {
    // пример строки: "1.25 MB • 11.05.2025" или "1,25 MB • 11.05.2025"
    const match = text.match(/([\d.,]+)\s*MB/i);
    if (!match) return 0;
    const number = match[1].replace(",", ".");
    return Math.round(parseFloat(number) * 1048576); // MB → bytes
}
