let currentPage = 1;
let recordsPerPage =  10;
let allRecords = []; // данные записей с файла

document.addEventListener("DOMContentLoaded", () => {
    fetch("/history/get-all")
        .then(res => res.json())
        .then(data => {
            allRecords = data;
            updateSummary();
            renderPage(currentPage);
        });

    document.getElementById("sortSelect").addEventListener("change", () => {
        sortRecords();
        renderPage(currentPage);
    });

    document.getElementById("prevPage").addEventListener("click", () => {
        if (currentPage > 1) renderPage(--currentPage);
    });

    document.getElementById("nextPage").addEventListener("click", () => {
        const maxPage = Math.ceil(allRecords.length / recordsPerPage);
        if (currentPage < maxPage) renderPage(++currentPage);
    });
});

function renderPage(page) {
    const list = document.getElementById("historyList");
    list.innerHTML = "";

    const start = (page - 1) * recordsPerPage;
    const end = start + recordsPerPage;
    const pageRecords = allRecords.slice(start, end);

    pageRecords.forEach(record => {
        const fileSizeMB = (record.fileSize / 1048576).toFixed(2);
        const displayDate = new Date(record.downloadDate).toLocaleDateString();
        const item = `
<div class="file-item" data-id="${record.id}" data-filepath="${record.filepath}">
    <div>
        <strong>${record.filename}</strong><br />
        <small>ID: ${record.id}</small><br />
        <small><a href="${record.url}" target="_blank">${record.url}</a></small><br />
        <small>${fileSizeMB} MB • ${displayDate}</small>
    </div>
    <div class="actions">
        <img src="/img/repeat.png" class="icon-btn repeat-icon" title="Повторить" />
        <img src="/img/trash.png" class="icon-btn trash-icon" alt="Удалить" title="Удалить" />
    </div>
</div>`;
        list.insertAdjacentHTML("beforeend", item);
    });

    document.getElementById("pageIndicator").textContent = `${page} / ${Math.ceil(allRecords.length / recordsPerPage)}`;
}

function updateSummary() {
    document.getElementById("recordCount").textContent = allRecords.length;
    const totalSize = allRecords.reduce((sum, r) => sum + r.fileSize, 0);
    document.getElementById("totalSize").textContent = (totalSize / 1048576).toFixed(2);
}

function sortRecords() {
    const value = document.getElementById("sortSelect").value;
    switch (value) {
        case "date-desc":
            allRecords.sort((a, b) => new Date(b.downloadDate) - new Date(a.downloadDate));
            break;
        case "date-asc":
            allRecords.sort((a, b) => new Date(a.downloadDate) - new Date(b.downloadDate));
            break;
        case "size-desc":
            allRecords.sort((a, b) => b.fileSize - a.fileSize);
            break;
        case "size-asc":
            allRecords.sort((a, b) => a.fileSize - b.fileSize);
            break;
        case "name-asc":
            allRecords.sort((a, b) => a.filename.localeCompare(b.filename));
            break;
        case "name-desc":
            allRecords.sort((a, b) => b.filename.localeCompare(a.filename));
            break;
    }
}

document.addEventListener("click", function (e) {
    if (e.target.classList.contains("trash-icon")) {
        const fileItem = e.target.closest(".file-item");
        if (!fileItem) return;

        const id = fileItem.dataset.id;
        const filepath = fileItem.dataset.filepath;
        const filename = fileItem.querySelector("strong").textContent;
        const url = fileItem.querySelector("a").href;
        const infoText = fileItem.querySelectorAll("small")[2].textContent;
        const fileSize = extractFileSize(infoText);

        const record = {
            id: id,
            filepath: filepath,
            filename: filename,
            url: url,
            fileSize: fileSize,
            downloadDate: new Date().toISOString()
        };

        fetch("/history/delete", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(record)
        })
            .then(response => {
                if (!response.ok) throw new Error("Ошибка при удалении записи из истории.");
                // Удалить из DOM
                fileItem.remove();
                // Удалить из массива
                allRecords = allRecords.filter(r => r.id !== id);
                updateSummary();
                renderPage(currentPage);
            })
            .catch(error => alert(error.message));
    }
});

function extractFileSize(text) {
    // пример строки: "1.25 MB • 11.05.2025" или "1,25 MB • 11.05.2025"
    const match = text.match(/([\d.,]+)\s*MB/i);
    if (!match) return 0;
    const number = match[1].replace(",", ".");
    return Math.round(parseFloat(number) * 1048576); // MB → bytes
}


document.addEventListener("click", function (e) {
    if (e.target.classList.contains("repeat-icon")) {
        const fileItem = e.target.closest(".file-item");
        if (!fileItem) return;

        const filepath = fileItem.dataset.filepath;
        const filename = fileItem.querySelector("strong").textContent;
        const url = fileItem.querySelector("a").href;
        const infoText = fileItem.querySelectorAll("small")[2].textContent;
        const fileSize = extractFileSize(infoText);

        const record = {
            filepath: filepath,
            filename: filename,
            url: url,
            fileSize: fileSize,
            downloadDate: new Date().toISOString()
        };

        // Сначала отправляем метаданные и получаем новый ID
        fetch("/history/repeat", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(record)
        })
            .then(res => {
                if (!res.ok) throw new Error("Ошибка при повторной загрузке.");
                return res.json();
            })
            .then(newRecord => {
                // Скачиваем файл по новому пути
                const params = new URLSearchParams({
                    filepath: newRecord.filepath,
                    filename: newRecord.filename
                });

                return fetch("/history/download?" + params.toString())
                    .then(response => {
                        if (!response.ok) throw new Error("Ошибка при скачивании файла.");
                        return response.blob().then(blob => ({ blob, response, newRecord }));
                    });
            })
            .then(({ blob, response, newRecord }) => {
                const link = document.createElement("a");
                link.href = URL.createObjectURL(blob);
                link.download = newRecord.filename;
                link.click();

                alert("Файл успешно повторно загружен и добавлен в историю.");

                allRecords.unshift(newRecord);
                updateSummary();
                renderPage(1);
            })
            .catch(err => alert(err.message));
    }
});

