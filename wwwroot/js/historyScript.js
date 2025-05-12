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
