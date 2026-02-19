const urlParams = new URLSearchParams(window.location.search);
const language = urlParams.get("language");
const code = urlParams.get("code");
const path = document.createElement("label");
const text = document.createElement("label");
const edit = document.createElement("input");
const last = document.createElement("label");
const next = document.createElement("label");
const relatedTokens = document.createElement("table");
let saved = true;
let index = 0;
/**
 * 内容  
 * [词条, 翻译, 文件路径]
 * @type {Array<Array<string>>}
 */
const content = [];
let localizationMdFiles;
/**
 * 数组是否相等
 * @param {Array} a 数组 a
 * @param {Array} b 数组 b
 * @returns {boolean} 是否相等
 */
function arraysEqual(a, b) {
    return a.length === b.length && a.every((value, index) => value === b[index]);
}
/**
 * 判断内容是否相等
 * @param {Array} a 内容 a
 * @param {Array} b 内容 b
 * @returns {boolean} 是否相等
 */
function contentEqual(a, b) {
    return a[0] === b[0] && arraysEqual(a[2].split("/").splice(2), b[2].split("/").splice(2));
}
/**
 * 判断词条相似程度
 * @param {Array<string>} a 词条 a
 * @param {Array<string>} b 词条 b
 * @returns {number} 相似程度
 */
function contentRelationship(a, b) {
    if (a[0] === b[0]) {
        return 1;
    }
    if (a[0].includes(b[0]) || b[0].includes(a[0])) {
        return 1;
    }
    /**
     * 使用的相同字符
     * @type {number}
     */
    let count = 0;
    for (const char of a[0]) {
        count += b[0].split(char).length - 1;
    }
    return count / Math.max(a[0].length, b[0].length);
}
/**
 * 显示与在翻译词条的相关词条
 * @returns {void}
 */
async function calculateAndShowRelatedTokens() {
    const contentIndexAndRelationship = [];
    for (let i = 0; i < content.length; i++) {
        if (content[i][1].trim() === "") {
            continue;
        }
        if (i == index) {
            continue;
        }
        contentIndexAndRelationship.push([i, contentRelationship(content[index], content[i])]);
    }
    const related = contentIndexAndRelationship.sort((a, b) => b[1] - a[1]).splice(0, 10);
    if (related.length === 0) {
        return;
    }
    relatedTokens.innerHTML = "";
    const firstRow = relatedTokens.insertRow();
    firstRow.className = "firstRow";
    const cell1 = firstRow.insertCell();
    const cell2 = firstRow.insertCell();
    const cell3 = firstRow.insertCell();
    const cell4 = firstRow.insertCell();
    cell1.innerText = "词条";
    cell2.innerText = "翻译";
    cell3.innerText = "文件路径";
    cell4.innerText = "相似程度";
    for (const relatedToken of related) {
        if (relatedToken[1] === 0) {
            break;
        }
        const row = relatedTokens.insertRow();
        const cell1 = row.insertCell();
        const cell2 = row.insertCell();
        const cell3 = row.insertCell();
        const cell4 = row.insertCell();
        cell1.innerText = content[relatedToken[0]][0];
        cell2.innerText = content[relatedToken[0]][1];
        cell3.innerText = content[relatedToken[0]][2];
        cell4.innerText = relatedToken[1];
    }
}
function lastContent() {
    if (index <= 0) {
        index = content.length - 1;
    } else {
        index--;
    }
    refreshElements();
}
function nextContent() {
    if (index >= content.length - 1) {
        index = 0;
    } else {
        index++;
    }
    refreshElements();
}
/**
 * 下载文件夹
 * @param {string} zipName 压缩包名，以`.zip`结尾
 * @param {Array<Array<string>>} files 文件`[文件名, 文件内容]`
 * @param {function} then 下载完成后的回调函数
 * @returns {void}
 */
async function downloadFolderAsZip(zipName, files, then) {
    await import("https://cdnjs.cloudflare.com/ajax/libs/jszip/3.10.1/jszip.min.js");
    const zip = new window.JSZip();
    files.forEach(file => {
        zip.file(file[0], file[1]);
    });
    const blob = await zip.generateAsync({ type: "blob" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = zipName;
    a.click();
    URL.revokeObjectURL(url);
    if (then) {
        then();
    }
}
/**
 * 保存
 * @returns {void}
 */
async function save() {
    /**
     * 文件内容
     * @type {Array<Array<string>>} [文件名, 文件内容]
     */
    const files = [];
    for (const contentLine of content) {
        const fileIndex = files.findIndex((e) => e[0] === contentLine[2]);
        const line = "|" + contentLine[0] + "|" + contentLine[1] + "|\n";
        if (fileIndex === -1) {
            files.push([contentLine[2], "|||\n|---|---|\n" + line]);
            continue;
        }
        files[fileIndex][1] += line;
    }
    downloadFolderAsZip("localization.zip", files, () => {
        saved = true;
    });
}
/**
 * 刷新词条
 * @returns {void}
 */
function refreshElements() {
    path.innerText = content[index][2];
    text.innerText = content[index][0];
    calculateAndShowRelatedTokens();
    if (content[index][1] === "-") {
        edit.value = content[index][0];
        return;
    }
    edit.value = content[index][1];
}
/**
 * 显示布局
 * @returns {void}
 */
function showLayout() {
    const fileReadPromises = [];
    for (const file of localizationMdFiles) {
        const isTemplate = file.webkitRelativePath.startsWith("localization/template/");
        const promise = new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = (e) => {
                const text = e.target.result;
                const lines = text.split("\n");
                for (const line of lines) {
                    if (!line) {
                        continue;
                    }
                    const tokens = line.split("|");
                    if (tokens.length !== 4) {
                        continue;
                    }
                    if (tokens[1].trim() === "") {
                        continue;
                    }
                    if (tokens[1] === "---") {
                        continue;
                    }
                    const contentIndex = content.findIndex((e) => contentEqual(e, [tokens[1], "", file.webkitRelativePath]));
                    if (contentIndex === -1) { // 首次出现
                        let tokenPath = file.webkitRelativePath;
                        if (isTemplate) {
                            tokenPath = tokenPath.replace("localization/template/", "localization/" + language + "/");
                        }
                        content.push([tokens[1], tokens[2], tokenPath]);
                        continue;
                    }
                    if (isTemplate) {
                        continue;
                    }
                    content[contentIndex][1] = tokens[2];
                }
                resolve();
            };
            reader.onerror = (e) => {
                reject(e);
            }
            reader.readAsText(file);
        });
        fileReadPromises.push(promise);
    }
    Promise.all(fileReadPromises).then(() => {
        refreshElements();
        const border = document.getElementsByClassName("border")[0];
        border.appendChild(path);
        border.appendChild(document.createElement("br"));
        border.appendChild(text);
        border.appendChild(document.createElement("br"));
        border.appendChild(edit);
        border.appendChild(document.createElement("br"));
        border.appendChild(last);
        border.appendChild(next);
        border.appendChild(document.createElement("br"));
        border.appendChild(relatedTokens);
        edit.addEventListener("input", () => {
            content[index][1] = edit.value;
            saved = false;
        });
    })
}
function init() {
    if (!language || !code) {
        alert("不要单独打开这个页面");
        return;
    }
    window.addEventListener("beforeunload", (e) => {
        if (!saved) {
            e.preventDefault();
        }
    });
    window.addEventListener("message", (event) => {
        if (event.data.type !== "localization") {
            return;
        }
        if (event.source !== window.opener) {
            return;
        }
        window.opener.postMessage(language + "_localization_DATA_RECEIVED_" + code, "*");
        localizationMdFiles = event.data.data;
        showLayout();
    });
    document.addEventListener('keydown', function (event) {
        if (event.key === "ArrowUp") {
            lastContent();
        }
        if (event.key === "ArrowDown") {
            nextContent();
        }
        if (event.ctrlKey && (event.key === "s" || event.key === "S")) {
            event.preventDefault();
            save();
        }
    });
    last.innerText = "上一条（↑）";
    last.className = "buttonLabel";
    last.onclick = lastContent;
    next.innerText = "下一条（↓）"
    next.className = "buttonLabel";
    next.onclick = nextContent;
}
init();
