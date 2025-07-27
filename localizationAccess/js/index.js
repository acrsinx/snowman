/**
 * 语言表
 * @type {Set<string>}
 */
const languages = new Set();
/**
 * 文件列表
 * @type {Array<File>}
 */
const mdFiles = [];
/**
 * 文件是否被收取
 * @type {boolean}
 */
let isFileReceived = false;
/**
 * 阻止页面刷新或关闭
 * @returns {void}
 */
function stopRefresh() {
    window.addEventListener("beforeunload", (e) => {
        if (true) { // 检查是否有未保存的更改
            e.preventDefault();
        }
    });
}
/**
 * 检查文件路径
 * @param {string} path 文件路径
 * @returns {boolean} 路径是否正常
 */
function checkFiles(path) {
    if (!path.startsWith("localization/")) {
        return false;
    }
    if (path.startsWith("localization/localization")) {
        return false;
    }
    return true;
}
/**
 * 文件夹选择错误
 * @returns {void}
 */
function folderError() {
    alert("请选择正确的文件夹");
    isFileReceived = false;
    languages.clear();
    document.getElementById("folderInput").value = "";
}
/**
 * 初始化
 * @returns {void}
 */
function init() {
    stopRefresh();
    // 收取文件
    document.getElementById("folderInput").addEventListener("change", (e) => {
        const files = e.target.files;
        if ((!files) || files.length === 0 || (!checkFiles(files[0].webkitRelativePath))) {
            folderError();
            return;
        }
        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            const path = file.webkitRelativePath;
            if (!path.endsWith(".md")) {
                continue;
            }
            languages.add(path.split("/")[1]);
            mdFiles.push(file);
        }
        if (languages.size === 0) {
            folderError();
            return;
        }
        isFileReceived = true;
        // 删除选择文件
        document.getElementById("folderInputButton").innerHTML = "";
        document.getElementById("folderInput").disabled = true;
        // 显示进度
        showProgress();
    });
}
/**
 * 显示进度
 * @type {Map<string, Array<number, number>>} 进度列表
 * @returns {void}
 */
function renderProgress(progressList) {
    const total = progressList.get("template")[1];
    for (const language of languages) {
        if (language === "template") {
            continue;
        }
        const progress = progressList.get(language);
        if (!progress) {
            continue;
        }
        const all = Math.max(progress[1], total);
        const button = document.createElement("label");
        const label = document.createElement("label");
        const progressBar = document.createElement("progress");
        button.innerText = "开始本地化";
        button.className = "buttonLabel"
        button.addEventListener("click", () => {
            const code = Math.random().toString(36).substring(2);
            const translateWindow = window.open("html/localization.html?language=" + language + "&code=" + code, "_blank");
            const filesToPost = mdFiles.filter((file) => (file.webkitRelativePath.startsWith(`localization/${language}/`) || file.webkitRelativePath.startsWith(`localization/template/`)));
            const checkReady = setInterval(() => {
                translateWindow.postMessage({
                    type: "localization",
                    data: filesToPost
                }, "*");
            }, 100);
            window.addEventListener("message", (e) => {
                if (e.data !== language + "_localization_DATA_RECEIVED_" + code) {
                    return;
                }
                if (e.source !== translateWindow) {
                    return;
                }
                clearInterval(checkReady);
            });
        })
        label.innerText = `${language}: ${Math.round(progress[0] / all * 100)}% (${progress[0]}/${all})`;
        progressBar.value = progress[0];
        progressBar.max = all;
        document.getElementById('progressBarContainer').appendChild(button);
        document.getElementById('progressBarContainer').appendChild(label);
        document.getElementById('progressBarContainer').appendChild(progressBar);
        document.getElementById('progressBarContainer').appendChild(document.createElement("br"));
    }
}
/**
 * 统计并显示本地化进度
 * @returns {void}
 */
async function showProgress() {
    if (!isFileReceived) {
        return;
    }
    /**
     * 进度列表
     * @type {Map<string, Array<number, number>>}
     */
    const progressList = new Map();
    const fileReadPromises = [];
    for (const file in mdFiles) {
        const language = mdFiles[file].webkitRelativePath.split("/")[1];
        if (!progressList.has(language)) {
            progressList.set(language, [0, 0]);
        }
        const promise = new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = function (e) {
                /**
                 * 文件内容
                 * @type {string}
                 */
                const content = e.target.result;
                const lines = content.split("\n");
                const progress = progressList.get(language);
                for (const line of lines) {
                    /**
                     * 词句
                     * @type {string[]}
                     */
                    const tokens = line.split("|");
                    if (tokens.length !== 4) {
                        continue;
                    }
                    if (tokens[1].trim() === "" || tokens[1].trim() === "---") { // 第 1 行第 2 行跳过
                        continue;
                    }
                    if (tokens[2].trim() !== "") {
                        progress[0] += 1;
                    }
                    progress[1] += 1;
                }
                progressList.set(language, progress);
                resolve();
            };
            reader.onerror = function () {
                console.error("读取文件失败");
                reject();
            };
            reader.readAsText(mdFiles[file]);
        });
        fileReadPromises.push(promise);
    }
    await Promise.all(fileReadPromises);
    renderProgress(progressList);
}
init();
