
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

            const html = `
        <div class="file-item shadow-sm" data-id="${record.id}">
            <div class="file-header">
                <div>
                    <strong>${record.filename}</strong><br />
                    <small>
                        <a href="${fullUrl}" target="_blank" title="${fullUrl}">
                            ${shortUrl}
                        </a>
                    </small><br />
                    <small>${(record.fileSize / 1048576).toFixed(2)} MB • ${new Date(record.downloadDate).toLocaleDateString()}</small>
                </div>
                <div class="btn-group actions">
                    <img src="/img/repeat.png" class="icon-btn repeat-icon" alt="Повторить" title="Повторить" />
                    <img src="/img/trash.png" class="icon-btn trash-icon" alt="Удалить" title="Удалить" />
                </div>
            </div>
        </div>`;
            document.getElementById("fileList").insertAdjacentHTML("beforeend", html);
            document.getElementById("urlInput").value = "";
        })
        .catch(error => {
            errorContainer.innerHTML = `<div class="alert alert-danger">${error.message}</div>`;
        });
});

// POST запрос на удаление элемента из списка
document.addEventListener("click", function (e) {
    if (e.target.classList.contains("trash-icon")) {
        const fileItem = e.target.closest(".file-item");
        const filename = fileItem.querySelector("strong").textContent;

        fetch("/delete", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ filename })
        })
        .then(response => {
            if (!response.ok) throw new Error("Ошибка при удалении файла.");
            fileItem.remove();
        })
        .catch(error => {
            alert(error.message);
        });
    }
});