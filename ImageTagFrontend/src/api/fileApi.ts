import { api } from "./axiosClient";
import type {
    FileListItemResponse,
    FileUploadResponse,
    AiTagResponse,
    RetryResponse
}
from "../types/file";

// 一覧取得
export async function getFiles(): Promise<FileListItemResponse[]>{
    const response = await api.get<FileListItemResponse[]>("files");
    return response.data;
}

// アップロード
export async function uploadFile(file:File): Promise<FileUploadResponse>{
    const formData = new FormData();
    formData.append("file", file);

    const response = await api.post<FileUploadResponse>("files/upload", formData,{
        headers: {"Content-Type": "multipart/form-data"}
    });

    return response.data;
}

// タグ生成
export async function generateTags(fileId: number):Promise<AiTagResponse>{
    const response = await api.post<AiTagResponse>(`files/${fileId}/generate-tags`);
    return response.data;
}

// Queue(再投入)
export async function retryQueue(fileId:number):Promise<RetryResponse>{
    const response = await api.post<RetryResponse>(`files/${fileId}/retry`);
    return response.data;
}