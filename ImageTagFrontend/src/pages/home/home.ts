import {
    generateTags,
    getFiles,
    retryQueue,
    uploadFile
} from "../../api/fileApi";
import type {
    FileListItemResponse,
    AiTagItem,
} from "../../types/file";
import { shortenFileName,statusToString, statusColor } from "./ui/utils";


let lastStatuses: Record<number, number> = {};
let allFiles: FileListItemResponse[] = [];
let activeTags = new Set<string>();
// 10秒に1回データを取得して、前回取得時とstatusに変更があればrenderしなおす
async function startPolling() {
    setInterval(async () => {
        const files = await getFiles();
        let changed = false;

        for (const f of files) {
            if (lastStatuses[f.fileId] !== f.status) {
                changed = true;
            }
            lastStatuses[f.fileId] = f.status;
        }

        if (changed) {
            loadFiles();
        }
    }, 10 * 1000);
}


// ファイルフォーム要素
const uploadForm = document.getElementById("uploadForm") as HTMLFormElement;
const fileInput = document.getElementById("fileInput") as HTMLInputElement;
const uploadMessage = document.getElementById("uploadMessage") as HTMLParagraphElement;

uploadForm.addEventListener("submit", async (e) => {
    e.preventDefault();

    const file = fileInput.files?.[0];
    if (!file) {
        uploadMessage.textContent = "ファイルを選択してください";
        uploadMessage.className = "text-red-600 text-sm mt-2";
        return;
    }

    uploadMessage.textContent = "アップロード中...";
    uploadMessage.className = "text-gray-600 text-sm mt-2";

    try {
        await uploadFile(file);

        uploadMessage.textContent = "アップロード成功";
        uploadMessage.className = "text-green-600 text-sm mt-2";

        // 入力をクリア
        fileInput.value = "";
        await loadFiles();
    }
    catch (err) {
        uploadMessage.textContent = "アップロードに失敗しました";
        uploadMessage.className = "text-red-600 text-sm mt-2";
    }
});

// ファイル一覧
const fileList = document.getElementById("fileList") as HTMLDivElement;

async function loadFiles() {
    allFiles = await getFiles();
    renderTagFilter(allFiles);
    renderFileList(allFiles);
}

// タグ検索
const tagFilter = document.getElementById("tagFilter") as HTMLDivElement;

function renderTagFilter(files:FileListItemResponse[]){
    tagFilter.innerHTML = "";

    const tagSet = new Set<string>();
    files.forEach(f =>{
        f.tags.forEach(t => tagSet.add(t.tag));
    });

    tagSet.forEach(tag => {
        const tagButton = document.createElement("button");
        tagButton.textContent = tag;

        const isActive = activeTags.has(tag);
        tagButton.className = 
        `
        px-3 py-1 rounded-full text-sm font-semibold ${isActive ? "bg-blue-600 text-white" : "bg-gray-200 text-gray-700"} 
        hover:bg-blue-500 hover:text-white transition
        `;

        tagButton.addEventListener("click", ()=>{
            if(activeTags.has(tag)){
                activeTags.delete(tag);
            }
            else{
                activeTags.add(tag);
            }
            renderFileList(files);
            renderTagFilter(files);
        });

        tagFilter.appendChild(tagButton);
    });
}

function renderFileList(files: FileListItemResponse[]) {
    fileList.innerHTML = "";

    const filtered = activeTags.size > 0
        ? files.filter(f => f.tags.some(t => activeTags.has(t.tag)))
        : files;

    filtered.forEach(f => {
        const card = createFileCard(f);
        fileList.appendChild(card);
    });
}


function createFileCard(file: FileListItemResponse): HTMLDivElement {
    const card = document.createElement("div");
    card.className = "bg-white p-4 rounded-lg shadow hover:scale-[1.02] hover:shadow-xl transition-all duration-200 p-4 flex flex-col";

    // サムネイル
    // Thumbnail Imageは画像サイズがすべてwidth:960,height:540のサイズだからいい感じに調整したい
    if (file.thumbnailUrl) {
        const img = document.createElement("img");
        img.src = file.thumbnailUrl;
        img.className = "w-full aspect-[16/9] object-cover rounded object-color rounded-md";
        card.appendChild(img);
    }


    // アップロードしたときのファイル名
    const originalFileName = document.createElement("h2");
    originalFileName.textContent = shortenFileName(file.originalFileName);
    originalFileName.title = file.originalFileName;
    originalFileName.className = "font-semibold text-lg mt-3";
    card.appendChild(originalFileName);

    // Status
    const status = document.createElement("p");
    status.textContent = statusToString(file.status);
    status.className = `text-center inline-block mt-2 p-1  text-xs font-bold rounded ${statusColor(file.status)}`;
    card.appendChild(status);

    // AIタグ生成ボタン
    // status == 1 AIタグ生成可能時のみボタンの表示
    if (file.status == 1) {
        const aiProcessBtn = document.createElement("button");
        aiProcessBtn.textContent = "AIタグ生成";
        aiProcessBtn.className =
            "mt-3 bg-blue-600 text-white px-3 py-2 rounded hover:bg-blue-700 transition text-sm hover:cursor-pointer";

        aiProcessBtn.addEventListener("click", async () => {
            aiProcessBtn.textContent = "生成中...";
            aiProcessBtn.disabled = true;

            try {
                await generateTags(file.fileId);
                await loadFiles();
            }
            catch (err) {
                alert("AIタグ生成に失敗しました");
            }
        });
        card.appendChild(aiProcessBtn);
    }

    // エラー時のQueue再投入ボタン
    // status == 3
    if (file.status == 3) {
        const retryBtn = document.createElement("button");
        retryBtn.textContent = "再処理";
        retryBtn.className =
            "mt-3 bg-red-600 text-white px-3 py-2 rounded hover:bg-red-700 transition text-sm hover:cursor-pointer";

        retryBtn.addEventListener("click", async () => {
            retryBtn.textContent = "再処理中...";
            retryBtn.disabled = true;

            try {
                await retryQueue(file.fileId);
                await loadFiles();
            }
            catch (err) {
                alert("再処理に失敗しました");
            }
        });
        card.appendChild(retryBtn);
    }

    // 生成されたタグ
    const tagContainer = createAiAnalyzedTagsContainer(file.tags);
    card.appendChild(tagContainer);

    return card;
}

loadFiles();
startPolling();

function createAiAnalyzedTagsContainer(tags: AiTagItem[]): HTMLDivElement {
    const tagContainer = document.createElement("div");
    tagContainer.className = "flex flex-wrap gap-2 mt-3";

    tags.forEach(t => {
        const tag = document.createElement("span");
        tag.textContent = t.tag;
        tag.className = `${t.bgColor} text-white font-semibold px-2 py-1 rounded-full text-xs`;
        tag.style.backgroundColor = t.bgColor;
        tagContainer.appendChild(tag);
    });

    return tagContainer;
}

const logoutBtn = document.getElementById("logoutBtn") as HTMLButtonElement;
logoutBtn.addEventListener("click", () => {
    // JWT を削除
    localStorage.removeItem("token");

    // ログインページへ遷移
    window.location.href = "index.html";
});