export interface AiTagItem{
    tag:string;
    bgColor:string;
}

export interface FileListItemResponse{
    fileId:number;
    originalFileName:string;
    status:number; // enum
    thumbnailUrl:string|null;
    tags:AiTagItem[];
}

export interface FileUploadResponse{
    fileId:number;
    originalFileName:string;
    fileStatus:number;
    thumbnailUrl: string|null;
}

export interface AiTagResponse{
    items:AiTagItem[];
}

export interface RetryResponse{
    message:string;
    fileId:number;
}