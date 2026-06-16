export function shortenFileName(name:string, maxLength=30):string{
    if(name.length <= maxLength){
        return name;
    }

    const half = Math.floor((maxLength - 3)/2);
    return name.slice(0, half) + "..." + name.slice(-half);
}

export function statusToString(status: number): string {
    switch (status) {
        case 0:
            return "画像処理中...";
        case 1:
            return "AIタグ生成可能";
        case 2:
            return "AIタグ生成完了";
        case 3:
            return "エラー";
        default:
            return "不明なステータス";
    }
}

export function statusColor(status: number): string {
    switch (status) {
        case 0:
            return "bg-yellow-500 text-white"; // Processing
        case 1:
            return "bg-blue-500 text-white"; // ReadyForTag
        case 2:
            return "bg-green-600 text-white"; // Completed
        case 3:
            return "bg-red-600 text-white"; // Error
        default:
            return "bg-gray-400 text-white";
    }
}