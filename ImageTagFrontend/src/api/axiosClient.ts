import axios from "axios";


const baseURL:string = import.meta.env.VITE_API_BASE_URL;

// axiosインスタンスの作成
export const api = axios.create({
    baseURL: baseURL, // ASP.NET Core APIのURL: // http://localhost:5095/api
    timeout: 10 * 1000 // タイムアウト10 * 1000ms
})

// リクエスト前に割り込んでJWTを自動で付与する
api.interceptors.request.use(
    (config)=>{
    const token = localStorage.getItem("token"); // local storageからJWTを取得
    if(token){
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
    }
    ,(error)=>{
        return Promise.reject(error);
    }
)

// 共通エラーハンドリング
api.interceptors.response.use(
    (response)=>response,
    (error)=>{
        if(error.response?.status == 401){
            localStorage.removeItem("token");
            window.location.href = "/index.html";
        }
        console.error("API Error;", error.response?.data || error.message);
        return Promise.reject(error);
    }
)